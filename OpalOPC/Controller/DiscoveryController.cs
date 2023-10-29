using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Controller
{
    public class DiscoveryController
    {
        private readonly ILogger _logger;
        private readonly CancellationToken? _token;
        private readonly IDiscoveryUtil _discoveryUtil;


        public DiscoveryController(ILogger logger, IDiscoveryUtil discoveryUtil, CancellationToken? token = null)
        {
            _logger = logger;
            _token = token;
            _discoveryUtil = discoveryUtil;
        }

        public ICollection<Target> DiscoverTargets(ICollection<Uri> discoveryUris)
        {
            _logger.LogDebug($"Starting Discovery with {discoveryUris.Count} URIs");

            ICollection<Target> targets = new List<Target>();
            foreach (Uri uri in discoveryUris)
            {
                TaskUtil.CheckForCancellation(_token);
                targets = targets.Concat(DiscoverTargets(uri)).ToList();
            }

            return targets;
        }

        // Given discoveryUri, discover all applications
        private ICollection<Target> DiscoverTargets(Uri discoveryUri)
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

            ICollection<Target> targets = new List<Target>();

            try
            {
                _logger.LogDebug($"Discovering applications in {discoveryUri}");

                if (discoveryUri.ToString().Contains("https://"))
                {
                    string msg = $"Https is not supported: {discoveryUri}";
                    _logger.LogError(msg);
                    return targets;
                }

                Uri discoveryUriWithIP = Utils.ParseUri(ConvertToIPBasedURI(discoveryUri.ToString()));

                ApplicationDescriptionCollection adc = DiscoverApplications(discoveryUriWithIP);

                _logger.LogDebug($"Discovered {adc.Count} applications");

                foreach (ApplicationDescription ad in adc)
                {
                    TaskUtil.CheckForCancellation(_token);
                    targets.Add(DiscoverEndpoints(ad));
                }
            }
            catch (Exception)
            {
            }

            return targets;
        }

        // Given uri as a string, replace the hostname with IP address
        private string ConvertToIPBasedURI(string uriString)
        {
            try
            {
                Uri uri = Utils.ParseUri(uriString) ?? throw new UriFormatException();
                IPAddress[] addresses = _discoveryUtil.ResolveIPv4Addresses(uri.Host);
                string ip = addresses.First().ToString();
                return uri.OriginalString.Replace(uri.Host, ip);
            }
            catch (UriFormatException)
            {
                string msg = $"Invalid Uri format {uriString}";
                _logger.LogError(msg);
                throw;
            }
            catch (Exception)
            {
                string msg = $"Unable to resolve hostname {uriString}";
                _logger.LogWarning(msg);
                throw;
            }
        }

        private ApplicationDescriptionCollection DiscoverApplications(Uri uri)
        {
            ApplicationDescriptionCollection adc;

            try
            {
                adc = _discoveryUtil.DiscoverApplications(uri);
            }
            catch (Opc.Ua.ServiceResultException e)
            {
                if (e.Message.Contains("BadRequestTimeout"))
                {
                    _logger.LogError($"Timeout connecting to discovery URI {uri}");
                }
                else if (e.Message.Contains("BadNotConnected") || e.Message.Contains("BadSecureChannelClosed"))
                {
                    _logger.LogError($"Error connecting to discovery URI {uri}");
                }
                else
                {
                    _logger.LogError($"Unknown exception connecting to discovery URI: {e}");
                }

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
                _logger.LogDebug($"Discovering endpoints for {applicationDescription.ApplicationName} ({applicationDescription.ProductUri})");
                _logger.LogTrace($"Using DiscoveryUrl {s}");

                string s_by_ip;
                try
                {
                    s_by_ip = ConvertToIPBasedURI(s);
                }
                catch (UriFormatException)
                {
                    continue;
                }
                catch (Exception e)
                {
                    string msg = $"{Utils.ParseUri(s).Host}: {e.Message}";
                    Server server = new(s, new EndpointDescriptionCollection());
                    server.AddError(new Error(msg));

                    target.AddServer(server);
                    continue;
                }

                if (s_by_ip.Contains("https://"))
                {
                    string msg = $"Https is not supported: {s_by_ip}";
                    _logger.LogWarning(msg);

                    Server server = new(s_by_ip, new EndpointDescriptionCollection());
                    server.AddError(new Error(msg));

                    target.AddServer(server);
                    continue;
                }

                EndpointDescriptionCollection edc;

                try
                {
                    edc = _discoveryUtil.DiscoverEndpoints(new Uri(s_by_ip));
                }
                catch (Opc.Ua.ServiceResultException e)
                {
                    string msg = string.Empty;

                    if (e.Message.Contains("BadNotConnected"))
                    {
                        msg = $"Cannot connect to discovery URI {s_by_ip}";
                    }
                    else
                    {
                        msg = $"Unknown exception connecting to discovery URI {s_by_ip}: {e}";
                    }
                    _logger.LogWarning(msg);

                    Server server = new(s_by_ip, new EndpointDescriptionCollection());
                    server.AddError(new Error(msg));
                    target.AddServer(server);

                    continue;
                }

                // remove all that contain https scheme
                edc.RemoveAll(e => e.EndpointUrl.Contains("https://"));

                _logger.LogDebug($"Discovered {edc.Count} endpoints");

                target.AddServer(new Server(s_by_ip, edc));
            }
            return target;
        }
    }
}