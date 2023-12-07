using Opc.Ua;
using Org.BouncyCastle.Asn1.X509;

namespace Model
{
    public class Target
    {

        private readonly ApplicationDescription _applicationDescription = new();
        public List<Server> Servers { get; private set; } = new();

        public ApplicationType? Type { get; private set; }
        public string? ApplicationName { get; private set; }
        public string? ApplicationUri { get; private set; }
        public string? ProductUri { get; private set; }


        // parameterless constructor for XML serializer
        internal Target()
        { }

        public Target(ApplicationDescription ad)
        {
            _applicationDescription = ad;

            Type = ad.ApplicationType;
            ApplicationName = ad.ApplicationName.ToString();
            ApplicationUri = ad.ApplicationUri;
            ProductUri = ad.ProductUri;

            Servers = new List<Server>();
        }

        public ICollection<Endpoint> GetEndpoints()
        {
            List<Endpoint> endpoints = new();

            foreach (ICollection<Endpoint> serverEndpointList in Servers.Select(s => s.SeparatedEndpoints))
            {
                endpoints.AddRange(serverEndpointList);
            }

            return endpoints;
        }

        public void AddServer(Server server)
        {
            Servers.Add(server);
        }

        // Merge endpoints with identical URI, add up their findings
        public void MergeEndpoints()
        {
            foreach (Server server in Servers)
            {
                server.MergeEndpoints();
            }

            // sort servers within target by issue severity
            Servers = Servers.OrderByDescending(s => s.Endpoints.Max(e => e.Issues.Max(i => i.Severity))).ToList();
        }
    }
}
