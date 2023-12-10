using Opc.Ua;

namespace Model
{
    public class Target
    {

        private readonly ApplicationDescription _applicationDescription = new();
        public List<Server> Servers { get; private set; } = new();

        public ApplicationType Type { get; private set; }
        public string ApplicationName { get; private set; }
        public string ApplicationUri { get; private set; }
        public string ProductUri { get; private set; }


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

        public void SortServersByIssueSeverity()
        {
            // sort servers within target by issue severity even if issues is empty
            Servers = Servers.OrderByDescending(s => s.Issues.Any() ? s.Issues.Max(i => i.Severity) : 0).ToList();
        }
    }
}
