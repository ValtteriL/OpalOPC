using System.Net;
using System.Net.Sockets;
using Opc.Ua;

namespace Util
{
    public interface IDiscoveryUtil
    {
        IPAddress[] ResolveIPv4Addresses(string host);
        ApplicationDescriptionCollection DiscoverApplications(Uri uri);
        Task<ApplicationDescriptionCollection> DiscoverApplicationsAsync(Uri uri);
        ServerOnNetworkCollection DiscoverApplicationsOnNetwork(Uri uri);
        Task<FindServersOnNetworkResponse> DiscoverApplicationsOnNetworkAsync(Uri uri, CancellationToken cancellationToken);
        EndpointDescriptionCollection DiscoverEndpoints(Uri uri);
    }

    public class DiscoveryUtil : IDiscoveryUtil
    {
        public ServerOnNetworkCollection DiscoverApplicationsOnNetwork(Uri uri)
        {
            uint startingRecordId = (uint)0;
            uint maxRecordsToReturn = (uint)0;
            StringCollection serverCapabilityFilter = [];
            return DiscoveryClient.Create(uri).FindServersOnNetwork(startingRecordId, maxRecordsToReturn, serverCapabilityFilter, out _);
        }

        public Task<FindServersOnNetworkResponse> DiscoverApplicationsOnNetworkAsync(Uri uri, CancellationToken cancellationToken)
        {
            uint startingRecordId = (uint)0;
            uint maxRecordsToReturn = (uint)0;
            StringCollection serverCapabilityFilter = [];
            RequestHeader requestHeader = new();
            return DiscoveryClient.Create(uri).FindServersOnNetworkAsync(requestHeader, startingRecordId, maxRecordsToReturn, serverCapabilityFilter, cancellationToken);
        }

        public ApplicationDescriptionCollection DiscoverApplications(Uri uri)
        {
            return DiscoveryClient.Create(uri).FindServers(null);
        }

        public Task<ApplicationDescriptionCollection> DiscoverApplicationsAsync(Uri uri)
        {
            return DiscoveryClient.Create(uri).FindServersAsync(null);
        }

        public EndpointDescriptionCollection DiscoverEndpoints(Uri uri)
        {
            return DiscoveryClient.Create(uri).GetEndpoints(null);
        }

        public IPAddress[] ResolveIPv4Addresses(string host)
        {
            return Dns.GetHostAddresses(host, AddressFamily.InterNetwork);
        }
    }
}
