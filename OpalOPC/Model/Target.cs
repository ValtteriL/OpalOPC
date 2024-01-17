using Opc.Ua;

namespace Model
{
    public class Target(ApplicationDescription ad)
    {

        private readonly ApplicationDescription _applicationDescription = ad;
        public List<Server> Servers { get; private set; } = [];

        public ApplicationType Type { get; private set; } = ad.ApplicationType;
        public string ApplicationName { get; private set; } = ad.ApplicationName.ToString();
        public string ApplicationUri { get; private set; } = ad.ApplicationUri;
        public string ProductUri { get; private set; } = ad.ProductUri;

        public int IssuesCount => Servers.Sum(s => s.Issues.Count);
        public int ErrorsCount => Servers.Sum(s => s.Errors.Count);

        public void AddServer(Server server)
        {
            Servers.Add(server);
        }

        public void SortServersByIssueSeverity()
        {
            // sort servers within target by issue severity even if issues is empty
            Servers = [.. Servers.OrderByDescending(s => s.Issues.Count != 0 ? s.Issues.Max(i => i.Severity) : 0)];
        }
    }
}
