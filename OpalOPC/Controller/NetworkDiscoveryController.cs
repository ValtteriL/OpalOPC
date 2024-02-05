using Microsoft.Extensions.Logging;
using Opc.Ua;
using Util;

namespace Controller
{
    public interface INetworkDiscoveryController
    {
        public List<Uri> MulticastDiscoverTargets();
    }

    public class NetworkDiscoveryController(ILogger logger, IDiscoveryUtil discoveryUtil) : INetworkDiscoveryController
    {
        public List<Uri> MulticastDiscoverTargets()
        {
            List<Uri> discoveryUrls = new LDSDiscoverer(discoveryUtil, logger).DiscoverTargets();
            return discoveryUrls;
        }

        private class LDSDiscoverer(IDiscoveryUtil discoveryUtil, ILogger logger)
        {
            // discover targets through LDS, try to find the LDS on localhost

            // 4840 is the default port
            // 4843 is the default port for HTTPS: https://help.commonvisionblox.com/OpcUa/server.html
            // 53530 is the default for Prosys OPC UA Simulation Server
            private readonly List<int> _ldsPortNumbers = [4840, 4843, 53530];
            private readonly string _discoveryUriBase = "opc.tcp://127.0.0.1";
            private readonly IDiscoveryUtil _discoveryUtil = discoveryUtil;

            public List<Uri> DiscoverTargets()
            {
                ApplicationDescriptionCollection applications = [];

                // try to discover applications and servers on each port
                // add applications to list
                // if server is found, discover applications on it as well and add to list
                foreach (int port in _ldsPortNumbers)
                {
                    Uri discoveryUri = new($"{_discoveryUriBase}:{port}");

                    applications.AddRange(DiscoverApplications(discoveryUri) ?? []);

                    foreach (ServerOnNetwork server in DiscoverApplicationsOnNetwork(discoveryUri) ?? [])
                    {
                        applications.AddRange(DiscoverApplications(new Uri(server.DiscoveryUrl)) ?? []);
                    }
                }

                // get flat list of unique discoveryUrls from all applications
                List<Uri> discoveryUrls = applications.SelectMany(app => app.DiscoveryUrls).Distinct().Select(s => new Uri(s)).ToList();

                return discoveryUrls;
            }

            private ApplicationDescriptionCollection DiscoverApplications(Uri discoveryUri)
            {
                try
                {
                    logger.LogTrace("{Message}", $"Discovering applications on {discoveryUri}");
                    return _discoveryUtil.DiscoverApplications(discoveryUri);
                }
                catch (Exception e)
                {
                    logger.LogDebug("{Message}", $"Error discovering applications on {discoveryUri}: {e.Message}");
                    return [];
                }
            }

            private ServerOnNetworkCollection DiscoverApplicationsOnNetwork(Uri discoveryUri)
            {
                try
                {
                    logger.LogTrace("{Message}", $"Discovering servers on network on {discoveryUri}");
                    return _discoveryUtil.DiscoverApplicationsOnNetwork(discoveryUri);
                }
                catch (Exception e)
                {
                    logger.LogDebug("{Message}", $"Error discovering servers on network on {discoveryUri}: {e.Message}");
                    return [];
                }
            }
        }

    }
}
