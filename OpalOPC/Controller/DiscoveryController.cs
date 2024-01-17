using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Controller
{
    public interface IDiscoveryController
    {
        ICollection<Target> DiscoverTargets(ICollection<Uri> discoveryUris);
    }

    public class DiscoveryController(ILogger<DiscoveryController> logger, IDiscoveryUtil discoveryUtil, ITaskUtil taskUtil) : IDiscoveryController
    {

        public ICollection<Target> DiscoverTargets(ICollection<Uri> discoveryUris)
        {
            logger.LogDebug("{Message}", $"Starting Discovery with {discoveryUris.Count} URIs");

            ICollection<Target> targets = new List<Target>();
            foreach (Uri uri in discoveryUris)
            {
                taskUtil.CheckForCancellation();
                targets = targets.Concat(DiscoverTargets(uri)).ToList();
            }

            return targets;
        }

        // Given discoveryUri, discover all applications
        private List<Target> DiscoverTargets(Uri discoveryUri)
        {

            // https://reference.opcfoundation.org/Core/Part4/v105/docs/

            // DISCOVER (find services, check supported modes, security policies, and user tokens)
            // https://reference.opcfoundation.org/GDS/v105/docs/4.3
            // 1. Use FindServers on LDS to get list of application descriptions
            // 2. Use GetEndpoint on each application's discoveryurls to get list of endpointdescriptions
            // Endpointdescriptions contain supported security policy, security mode, and all supported user tokens 
            // If the ApplicationType is Discovery server, it can be used to find other servers with FindServersOnNetwork and iterate through them

            // Application
            // - Endpoint..N
            // - Access privileges..M

            // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.2
            // ask the server for all servers it knows about

            List<Target> targets = [];


            logger.LogDebug("{Message}", $"Discovering applications in {discoveryUri}");

            if (discoveryUri.ToString().Contains("https://"))
            {
                string msg = $"Https is not supported: {discoveryUri}";
                logger.LogError("{Message}", msg);
                return targets;
            }

            Uri discoveryUriWithIP;

            try
            {
                discoveryUriWithIP = Utils.ParseUri(ConvertToIPBasedURI(discoveryUri.ToString()));
            }
            catch (Exception)
            {
                logger.LogWarning("{Message}", $"Skipping {discoveryUri} because of preceding errors");
                return targets;
            }

            ApplicationDescriptionCollection adc;

            try
            {
                adc = DiscoverApplications(discoveryUriWithIP);
            }
            catch (ServiceResultException)
            {
                logger.LogWarning("{Message}", $"Skipping {discoveryUri} because of preceding errors");
                return targets;
            }

            logger.LogDebug("{Message}", $"Discovered {adc.Count} applications");

            foreach (ApplicationDescription ad in adc)
            {
                taskUtil.CheckForCancellation();
                targets.Add(DiscoverEndpoints(ad));
            }

            return targets;
        }

        // Given uri as a string, replace the hostname with IP address
        private string ConvertToIPBasedURI(string uriString)
        {
            try
            {
                Uri uri = Utils.ParseUri(uriString) ?? throw new UriFormatException();
                IPAddress[] addresses = discoveryUtil.ResolveIPv4Addresses(uri.Host);
                string ip = addresses.First().ToString();
                return uri.OriginalString.Replace(uri.Host, ip);
            }
            catch (Exception ex)
            {
                string msg;
                if (ex is UriFormatException)
                {
                    msg = $"Invalid Uri format {uriString}";
                    logger.LogError("{Message}", msg);
                }
                else if (ex is SocketException || ex is ArgumentException)
                {
                    msg = $"Unable to resolve hostname {uriString}";
                    logger.LogError("{Message}", msg);
                }
                else if (ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
                {
                    msg = $"Hostname or address of {uriString} is invalid";
                    logger.LogError("{Message}", msg);
                }
                else
                {
                    throw;
                }

                throw new Exception(msg);
            }
        }

        private ApplicationDescriptionCollection DiscoverApplications(Uri uri)
        {
            ApplicationDescriptionCollection adc;

            try
            {
                adc = discoveryUtil.DiscoverApplications(uri);
            }
            catch (Opc.Ua.ServiceResultException e)
            {
                string msg = $"Cannot connect to discovery URI {uri}: {e}";
                logger.LogError("{Message}", msg);
                throw;
            }

            return adc;
        }

        private Target DiscoverEndpoints(ApplicationDescription applicationDescription)
        {
            Target target = new(applicationDescription);

            foreach (string s in applicationDescription.DiscoveryUrls)
            {

                // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.4
                // ask each discoveryUrl for endpoints
                logger.LogDebug("{Message}", $"Discovering endpoints for {applicationDescription.ApplicationName} ({applicationDescription.ProductUri})");
                logger.LogTrace("{Message}", $"Using DiscoveryUrl {s}");

                string s_by_ip;
                try
                {
                    s_by_ip = ConvertToIPBasedURI(s);
                }
                catch (Exception e)
                {
                    Server server = new(s, []);
                    server.AddError(new Error(e.Message));

                    target.AddServer(server);
                    continue;
                }

                if (s_by_ip.Contains("https://"))
                {
                    string msg = $"Https is not supported: {s_by_ip}";
                    logger.LogWarning("{Message}", msg);

                    Server server = new(s, []);
                    server.AddError(new Error(msg));

                    target.AddServer(server);
                    continue;
                }

                EndpointDescriptionCollection edc;

                try
                {
                    edc = discoveryUtil.DiscoverEndpoints(new Uri(s_by_ip));
                }
                catch (Opc.Ua.ServiceResultException e)
                {
                    string msg = $"Cannot connect to discovery URI {s_by_ip}: {e}";
                    logger.LogWarning("{Message}", msg);

                    Server server = new(s, []);
                    server.AddError(new Error(msg));
                    target.AddServer(server);

                    continue;
                }

                // remove all that contain https scheme
                edc.RemoveAll(e => e.EndpointUrl.Contains("https://"));

                logger.LogDebug("{Message}", $"Discovered {edc.Count} endpoints");

                target.AddServer(new Server(s, edc));
            }
            return target;
        }
    }
}
