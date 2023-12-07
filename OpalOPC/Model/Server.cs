using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Opc.Ua;
namespace Model
{
    public class Server
    {

        public string DiscoveryUrl { get; private set; } = string.Empty;
        public List<Error> Errors { get; private set; } = new List<Error>();
        public ICollection<Issue> Issues { get; private set; } = new List<Issue>();

        [JsonIgnore]
        public ICollection<Endpoint> SeparatedEndpoints { get; } = new List<Endpoint>();
        [JsonIgnore]
        public ICollection<ISecurityTestSession> securityTestSessions { get; private set; } = new List<ISecurityTestSession>();

        [JsonIgnore]
        public EndpointDescriptionCollection EndpointDescriptions { get; private set; } = new EndpointDescriptionCollection();

        [XmlArrayItem("Endpoint")]
        public List<EndpointSummary> Endpoints { get; private set; } = new List<EndpointSummary>();

        // parameterless constructor for XML serializer
        internal Server()
        { }

        public Server(string DiscoveryUrl, EndpointDescriptionCollection edc)
        {
            this.DiscoveryUrl = DiscoveryUrl;
            EndpointDescriptions = edc;
            foreach (EndpointDescription e in edc)
            {
                SeparatedEndpoints.Add(new Endpoint(e));
            }
        }

        public void AddError(Error error)
        {
            Errors.Add(error);
        }

        public void AddIssue(Issue issue)
        {
            Issues.Add(issue);
        }

        // Merge SeparatedEndpoints into Endpointsummaries by endpointUrls
        public void MergeEndpoints()
        {
            Dictionary<string, EndpointSummary> endpointDictionary = new();

            foreach (Endpoint endpoint in SeparatedEndpoints)
            {
                if (endpointDictionary.TryGetValue(endpoint.EndpointUrl, out EndpointSummary? value))
                {
                    value.MergeEndpoint(endpoint);
                    continue;
                }

                endpointDictionary.Add(endpoint.EndpointUrl, new EndpointSummary(endpoint));
            }

            // sort endpoints within server by issue severity
            Endpoints = endpointDictionary.Values.OrderByDescending(e => e.Issues.Max(i => i.Severity)).ToList();
        }

        public void AddSecurityTestSession(ISecurityTestSession securityTestSession)
        {
            securityTestSessions.Add(securityTestSession);
        }

    }
}
