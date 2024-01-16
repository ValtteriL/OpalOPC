using Opc.Ua;
namespace Model
{
    public class Server
    {

        public string DiscoveryUrl { get; private set; } = string.Empty;
        public List<Error> Errors { get; private set; } = [];
        public ICollection<Issue> Issues { get; private set; } = new List<Issue>();
        public ICollection<ISecurityTestSession> securityTestSessions { get; private set; } = new List<ISecurityTestSession>();
        public EndpointDescriptionCollection EndpointDescriptions { get; private set; } = [];


        public Server(string DiscoveryUrl, EndpointDescriptionCollection edc)
        {
            this.DiscoveryUrl = DiscoveryUrl;
            EndpointDescriptions = edc;
        }

        public void AddError(Error error)
        {
            Errors.Add(error);
        }

        public void AddIssue(Issue issue)
        {
            Issues.Add(issue);
            Issues = Issues.OrderByDescending(i => i.Severity).ToList();
        }

        public void AddSecurityTestSession(ISecurityTestSession securityTestSession)
        {
            securityTestSessions.Add(securityTestSession);
        }

    }
}
