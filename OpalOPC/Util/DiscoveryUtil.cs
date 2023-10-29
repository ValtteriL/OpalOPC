using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;

namespace Util
{
    public interface IDiscoveryUtil
    {
        IPAddress[] ResolveIPv4Addresses(string host);
        ApplicationDescriptionCollection DiscoverApplications(Uri uri);
        EndpointDescriptionCollection DiscoverEndpoints(Uri uri);
    }

    public class DiscoveryUtil : IDiscoveryUtil
    {
        public ApplicationDescriptionCollection DiscoverApplications(Uri uri)
        {
            return DiscoveryClient.Create(uri).FindServers(null);
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
