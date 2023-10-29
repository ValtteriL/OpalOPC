using Opc.Ua;
using Plugin;

namespace Model
{
    public class Target
    {

        private readonly ApplicationDescription? _applicationDescription;
        public List<Server> Servers { get; set; } = new List<Server>();

        public ApplicationType? Type { get; set; }
        public string? ApplicationName { get; set; }
        public string? ApplicationUri { get; set; }
        public string? ProductUri { get; set; }


        // parameterless constructor for XML serializer
        internal Target()
        {}

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
        }
    }
}