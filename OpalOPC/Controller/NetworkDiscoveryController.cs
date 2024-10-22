﻿using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Util;

namespace Controller
{
    public interface INetworkDiscoveryController
    {
        public Task<IList<Uri>> MulticastDiscoverTargets(int timeoutSeconds);
    }

    public class NetworkDiscoveryController(ILogger<NetworkDiscoveryController> logger, IDiscoveryUtil discoveryUtil, IMDNSUtil mDNSUtil) : INetworkDiscoveryController
    {
        public async Task<IList<Uri>> MulticastDiscoverTargets(int timeoutSeconds)
        {

            ConcurrentBag<Uri> targetUris = [];

            // run both discovery methods in parallel until timeout
            // if timeout is reached, stop discovery and return list of unique targetUris
            using var cts = new CancellationTokenSource();

            // cancel after timeout
            cts.CancelAfter(timeoutSeconds * 1000);

            List<Task> tasks = [];
            tasks.Add(new LDSDiscoverer(discoveryUtil, logger).DiscoverTargets(targetUris, cts.Token));
            tasks.Add(new DNSSDDiscoverer(mDNSUtil, logger).DiscoverTargets(targetUris, cts.Token));
            await Task.WhenAll(tasks);

            IList<Uri> discoveredUris = targetUris.Distinct().ToList();

            // return list of unique targetUris
            return discoveredUris;
        }

        private class DNSSDDiscoverer(IMDNSUtil mDNSUtil, ILogger<NetworkDiscoveryController> logger)
        {
            // discover targets through DNS-SD

            // https://reference.opcfoundation.org/GDS/v105/docs/C
            private readonly List<(string, string)> _dnsSdServiceNamesAndSchemes = [
                ("_opcua-tcp", "opc.tcp"),
                ("_opcua-tls", "opc.wss"),
                ("_opcua-https", "opc.https")
                ];
            private readonly string _protocol = "_tcp";

            public Task DiscoverTargets(ConcurrentBag<Uri> targetUris, CancellationToken cancellationToken)
            {
                logger.LogTrace("{Message}", "Discovering targets through DNS-SD");

                List<Task> tasks = [];

                foreach ((string serviceName, string scheme) in _dnsSdServiceNamesAndSchemes)
                {
                    // execute discovertargets and trigger cancellation after 5 seconds
                    tasks.Add(DiscoverTargets(targetUris, $"{serviceName}.{_protocol}.", scheme, cancellationToken));
                }

                return Task.WhenAll(tasks);
            }

            private async Task DiscoverTargets(ConcurrentBag<Uri> targetUris, string query, string scheme, CancellationToken cancellationToken)
            {
                try
                {
                    await mDNSUtil.DiscoverTargets(targetUris, query, scheme, cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogDebug("{Message}", $"Error discovering targets through DNS-SD: {e.Message}");
                }
            }
        }

        private class LDSDiscoverer(IDiscoveryUtil discoveryUtil, ILogger<NetworkDiscoveryController> logger)
        {
            // discover targets through LDS, try to find the LDS on localhost

            // 4840 is the default port
            // 4843 is the default port for HTTPS: https://help.commonvisionblox.com/OpcUa/server.html
            // 53530 is the default for Prosys OPC UA Simulation Server
            private readonly List<int> _ldsPortNumbers = [4843, 26543, 48010, 48020, 48031, 48050, 4840, 4841, 4855, 4885, 4897, 49320, 53520, 53530, 62541];
            private readonly string _discoveryUriBase = "opc.tcp://127.0.0.1";
            private readonly IDiscoveryUtil _discoveryUtil = discoveryUtil;

            public async Task DiscoverTargets(ConcurrentBag<Uri> targetUris, CancellationToken cancellationToken)
            {

                // try to discover applications and servers on each port
                // add applications to list
                // if server is found, discover applications on it as well and add to list

                List<Task> tasks = [];

                foreach (int port in _ldsPortNumbers)
                {
                    Uri discoveryUri = new($"{_discoveryUriBase}:{port}");

                    tasks.Add(Task.Run(() => DiscoverApplications(discoveryUri, targetUris), cancellationToken));
                    tasks.Add(DiscoverApplicationsOnNetwork(discoveryUri, targetUris, cancellationToken));
                }

                // wait until tasks ready or until cancellation
                await Task.WhenAll(tasks);

                return;
            }

            private async Task DiscoverApplicationsOnNetwork(Uri discoveryUri, ConcurrentBag<Uri> targetUris, CancellationToken cancellationToken)
            {
                // get flat list of unique discoveryUrls from servers on network and add them to targetUris

                FindServersOnNetworkResponse serversOnNetwork = await DiscoverApplicationsOnNetwork(discoveryUri, cancellationToken);

                List<Task> tasks = [];

                foreach (ServerOnNetwork server in serversOnNetwork.Servers)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        ApplicationDescriptionCollection applicationsOnNetwork = await DiscoverApplications(new Uri(server.DiscoveryUrl)) ?? [];
                        applicationsOnNetwork.SelectMany(app => app.DiscoveryUrls).Distinct().Select(s => new Uri(s)).ToList().ForEach(targetUris.Add);
                    }, cancellationToken));
                }

                await Task.WhenAll(tasks);

                return;
            }

            private async Task DiscoverApplications(Uri discoveryUri, ConcurrentBag<Uri> targetUris)
            {

                ApplicationDescriptionCollection applications = await DiscoverApplications(discoveryUri) ?? [];

                // get flat list of unique discoveryUrls and add them to targetUris
                applications.SelectMany(app => app.DiscoveryUrls).Distinct().Select(s => new Uri(s)).ToList().ForEach(targetUris.Add);
            }

            private async Task<ApplicationDescriptionCollection> DiscoverApplications(Uri discoveryUri)
            {
                try
                {
                    logger.LogTrace("{Message}", $"Discovering applications on {discoveryUri}");
                    return await _discoveryUtil.DiscoverApplicationsAsync(discoveryUri);
                }
                catch (Exception e)
                {
                    logger.LogDebug("{Message}", $"Error discovering applications on {discoveryUri}: {e.Message}");
                    return [];
                }
            }

            private async Task<FindServersOnNetworkResponse> DiscoverApplicationsOnNetwork(Uri discoveryUri, CancellationToken cancellationToken)
            {
                try
                {
                    logger.LogTrace("{Message}", $"Discovering servers on network on {discoveryUri}");
                    return await _discoveryUtil.DiscoverApplicationsOnNetworkAsync(discoveryUri, cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogDebug("{Message}", $"Error discovering servers on network on {discoveryUri}: {e.Message}");
                    return new FindServersOnNetworkResponse();
                }
            }
        }

    }
}
