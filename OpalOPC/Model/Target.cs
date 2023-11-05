using Opc.Ua;

namespace Model
{
    public class Target
    {

        private readonly ApplicationDescription? _applicationDescription;
        public List<Server> Servers { get; set; } = new();

        public ApplicationType? Type { get; set; }
        public string? ApplicationName { get; set; }
        public string? ApplicationUri { get; set; }
        public string? ProductUri { get; set; }


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
