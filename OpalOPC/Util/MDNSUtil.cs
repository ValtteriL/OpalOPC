using Makaretu.Dns;

namespace Util
{
    public interface IMDNSUtil
    {
        public List<Uri> DiscoverTargets(string query, string scheme, CancellationToken cancellationToken);
    }

    public class MDNSUtil : IMDNSUtil
    {
        private readonly List<Uri> _targetUris = [];

        public List<Uri> DiscoverTargets(string query, string scheme, CancellationToken cancellationToken)
        {
            MulticastService multicastService = new();
            ServiceDiscovery serviceDiscovery = new(multicastService);

            multicastService.NetworkInterfaceDiscovered += (s, e) =>
            {
                // query for services in all network interfaces
                serviceDiscovery.QueryServiceInstances(query);
            };

            serviceDiscovery.ServiceInstanceDiscovered += (s, e) =>
            {
                // when service instance is discovered, query for its SRV record
                multicastService.SendQuery(e.ServiceInstanceName, type: DnsType.SRV);
            };

            multicastService.AnswerReceived += (s, e) =>
            {
                // when SRV record is received, parse it and add the target URI to the list
                List<SRVRecord> srvRecords = e.Message.Answers.OfType<SRVRecord>().ToList();
                foreach (SRVRecord srvRecord in srvRecords)
                {
                    string targetUri = $"{scheme}://{srvRecord.Target}:{srvRecord.Port}";
                    _targetUris.Add(new Uri(targetUri));
                }
            };

            try
            {
                multicastService.Start();
                // wait until the cancellation token is triggered
                cancellationToken.WaitHandle.WaitOne();
            }
            finally
            {
                serviceDiscovery.Dispose();
                multicastService.Stop();
            }

            return _targetUris;
        }
    }
}
