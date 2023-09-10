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
        ILogger _logger;
        readonly CancellationToken? _token;

        public DiscoveryController(ILogger logger, CancellationToken? token = null)
        {
            _logger = logger;
            _token = token;
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
            _logger.LogDebug($"Discovering applications in {discoveryUri}");

            ICollection<Target> targets = new List<Target>();

            if (discoveryUri.ToString().Contains("https://"))
            {
                string msg = $"Https is not supported: {discoveryUri}";
                _logger.LogError(msg);
                return targets;
            }


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

            Uri discoveryUriWithIP;
            try
            {
                discoveryUriWithIP = Utils.ParseUri(ConvertToIPBasedURI(discoveryUri.ToString()));
            }
            catch (SocketException)
            {
                string msg = $"Unable to resolve hostname {discoveryUri.ToString()}";
                _logger.LogError(msg);
                return targets;
            }

            DiscoveryClient asd = DiscoveryClient.Create(discoveryUriWithIP);
            ApplicationDescriptionCollection adc;

            try
            {
                adc = asd.FindServers(null);
            }
            catch (Opc.Ua.ServiceResultException e)
            {
                if (e.Message.Contains("BadRequestTimeout"))
                {
                    _logger.LogError($"Timeout connecting to discovery URI {discoveryUri}");
                }
                else if (e.Message.Contains("BadNotConnected") || e.Message.Contains("BadSecureChannelClosed"))
                {
                    _logger.LogError($"Error connecting to discovery URI {discoveryUri}");
                }
                else
                {
                    _logger.LogError($"Unknown exception connecting to discovery URI: {e}");
                }

                return targets;
            }

            _logger.LogDebug($"Discovered {adc.Count} applications");

            foreach (ApplicationDescription ad in adc)
            {

                TaskUtil.CheckForCancellation(_token);

                Target target = new Target(ad);

                foreach (string s in ad.DiscoveryUrls)
                {

                    // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.4
                    // ask each discoveryUrl for endpoints
                    _logger.LogDebug($"Discovering endpoints for {ad.ApplicationName} ({ad.ProductUri})");
                    _logger.LogTrace($"Using DiscoveryUrl {s}");

                    string s_by_ip;
                    try
                    {
                        s_by_ip = ConvertToIPBasedURI(s);
                    }
                    catch (UriFormatException)
                    {
                        _logger.LogError($"Invalid Uri: {s}, skipping");
                        continue;
                    }
                    catch (Exception)
                    {
                        string msg = $"Unable to resolve hostname {Utils.ParseUri(s).Host}";
                        _logger.LogWarning(msg);

                        Server server = new Server(s, new EndpointDescriptionCollection());
                        server.AddError(new Error(msg));

                        target.AddServer(server);
                        continue;
                    }

                    if (s_by_ip.Contains("https://"))
                    {
                        string msg = $"Https is not supported: {s_by_ip}";
                        _logger.LogWarning(msg);

                        Server server = new Server(s_by_ip, new EndpointDescriptionCollection());
                        server.AddError(new Error(msg));

                        target.AddServer(server);
                        continue;
                    }

                    DiscoveryClient sss = DiscoveryClient.Create(new Uri(s_by_ip));
                    EndpointDescriptionCollection edc;

                    try
                    {
                        edc = sss.GetEndpoints(null);
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

                        Server server = new Server(s_by_ip, new EndpointDescriptionCollection());
                        server.AddError(new Error(msg));
                        target.AddServer(server);

                        continue;
                    }

                    // remove all that contain https scheme
                    edc.RemoveAll(e => e.EndpointUrl.Contains("https://"));

                    _logger.LogDebug($"Discovered {edc.Count} endpoints");

                    target.AddServer(new Server(s_by_ip, edc));
                }

                targets.Add(target);
            }

            return targets;
        }

        // Given uri as a string, replace the hostname with IP address
        private static string ConvertToIPBasedURI(String uriString)
        {
            Uri uri = Utils.ParseUri(uriString);
            if (uri == null)
            {
                throw new UriFormatException();
            }

            string ip = Dns.GetHostAddresses(uri!.Host)[0].ToString();

            return uri.OriginalString.Replace(uri!.Host, ip);
        }
    }
}