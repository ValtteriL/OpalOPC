using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Controller
{
    public class DiscoveryController
    {
        ILogger _logger;

        public DiscoveryController(ILogger logger)
        {
            _logger = logger;
        }

        public ICollection<Target> DiscoverTargets(ICollection<Uri> discoveryUris)
        {
            _logger.LogDebug($"Starting Discovery with {discoveryUris.Count} URIs");

            ICollection<Target> targets = new List<Target>();
            foreach (Uri uri in discoveryUris)
            {
                targets = targets.Concat(DiscoverTargets(uri)).ToList();
            }

            return targets;
        }

        // Given discoveryUri, discover all applications
        private ICollection<Target> DiscoverTargets(Uri discoveryUri)
        {
            _logger.LogDebug($"Discovering applications in {discoveryUri}");

            ICollection<Target> targets = new List<Target>();

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
            DiscoveryClient asd = DiscoveryClient.Create(discoveryUri);
            ApplicationDescriptionCollection adc;

            try
            {
                adc = asd.FindServers(null);
            }
            catch (Opc.Ua.ServiceResultException e)
            {
                if (e.Message.Contains("BadRequestTimeout"))
                {
                    _logger.LogWarning($"Timeout connecting to discovery URI {discoveryUri}");
                    return targets;
                }
                throw;
            }

            _logger.LogDebug($"Discovered {adc.Count} applications");

            foreach (ApplicationDescription ad in adc)
            {

                Target target = new Target(ad);

                foreach (string s in ad.DiscoveryUrls)
                {

                    // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.4
                    // ask each discoveryUrl for endpoints
                    _logger.LogDebug($"Discovering endpoints for {ad.ApplicationName} ({ad.ProductUri})");
                    _logger.LogTrace($"Using DiscoveryUrl {s}");

                    DiscoveryClient sss = DiscoveryClient.Create(new Uri(s));
                    EndpointDescriptionCollection edc;

                    try
                    {
                        edc = sss.GetEndpoints(null);
                    }
                    catch (Opc.Ua.ServiceResultException e)
                    {
                        if (e.Message.Contains("BadNotConnected"))
                        {
                            string msg = $"Cannot connect to discovery URI {s}";
                            _logger.LogWarning(msg);
                            target.AddError(msg);
                            target.AddServer(s, new EndpointDescriptionCollection());
                            continue;
                        }
                        throw;
                    }

                    _logger.LogDebug($"Discovered {edc.Count} endpoints");

                    target.AddServer(s, edc);

                    if (ad.ApplicationType == ApplicationType.DiscoveryServer)
                    {
                        // TODO: NOTE this is not for GDS, only for LDS
                        // For GDS, must use GlobalDiscoveryServerClient and Query the registered applications
                        // Calling FindServersOnNetwork() on LDS will cause the LDS to do multicast probe for other LDS's
                        // and return their discoveryuris
                        // Thus this is useful for finding applications on local network
                        // https://reference.opcfoundation.org/GDS/v105/docs/4.3.4

                        _logger.LogCritical("Application is a discoveryserver");

                        // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.3
                        // ask the network servers this server knows about
                        // only works with discoveryservers

                        ServerOnNetworkCollection sonc = sss.FindServersOnNetwork(0, 0, null, out DateTime dt);
                        foreach (ServerOnNetwork son in sonc)
                        {
                            _logger.LogCritical($"SERVER ON NETWORK");
                            // GetEndpoints could be used to check the endpoint securities of the found servers
                        }
                    }
                }

                targets.Add(target);
            }

            return targets;
        }
    }
}