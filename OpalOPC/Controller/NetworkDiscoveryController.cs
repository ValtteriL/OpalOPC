using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Util;

namespace Controller
{
    public interface INetworkDiscoveryController
    {
        public List<Uri> MulticastDiscoverTargets();
    }

    public class NetworkDiscoveryController(ILogger logger, IDiscoveryUtil discoveryUtil, IMDNSUtil mDNSUtil) : INetworkDiscoveryController
    {
        public List<Uri> MulticastDiscoverTargets()
        {

            ConcurrentBag<Uri> targetUris = [];

            // run both discovery methods in parallel
            Parallel.Invoke(
                () => new LDSDiscoverer(discoveryUtil, logger).DiscoverTargets().ForEach(targetUris.Add),
                () => new DNSSDDiscoverer(mDNSUtil, logger).DiscoverTargets().ForEach(targetUris.Add)
                );

            // return list of unique targetUris
            return targetUris.Distinct().ToList();
        }

        private class DNSSDDiscoverer(IMDNSUtil mDNSUtil, ILogger logger)
        {
            // discover targets through DNS-SD

            // https://reference.opcfoundation.org/GDS/v105/docs/C
            private readonly List<(string, string)> _dnsSdServiceNamesAndSchemes = [
                ("_opcua-tcp", "opc.tcp"),
                ("_opcua-tls", "opc.wss"),
                ("_opcua-https", "opc.https")
                ];
            private readonly string _protocol = "_tcp";

            public List<Uri> DiscoverTargets()
            {
                ConcurrentBag<Uri> targetUris = [];

                try
                {
                    logger.LogTrace("{Message}", "Discovering targets through DNS-SD");

                    // run parallel discovery for all service names
                    Parallel.ForEach(_dnsSdServiceNamesAndSchemes, (serviceNameAndScheme) =>
                    {
                        (string serviceName, string scheme) = serviceNameAndScheme;
                        // execute discovertargets and trigger cancellation after 5 seconds
                        mDNSUtil.DiscoverTargets($"{serviceName}.{_protocol}.", scheme, new CancellationTokenSource(5000).Token).ForEach(targetUris.Add);
                    });

                    // return list of targetUris
                    return targetUris.Distinct().ToList();
                }
                catch (Exception e)
                {
                    logger.LogDebug("{Message}", $"Error discovering targets through DNS-SD: {e.Message}");
                    return [];
                }
            }
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
