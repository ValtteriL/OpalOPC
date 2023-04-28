
using Opc.Ua;

namespace Model
{
    public class EndpointSummary
    {
        public string EndpointUrl { get; }
        public byte[] ServerCertificate { get; }
        public ICollection<string> SecurityPolicyUris { get; } = new HashSet<string>();
        public ICollection<MessageSecurityMode> SecurityModes { get; } = new HashSet<MessageSecurityMode>();
        public IEnumerable<string> UserTokenPolicyIds { get; private set; } = new HashSet<string>();
        public IEnumerable<UserTokenType> UserTokenTypes { get; private set; } = new HashSet<UserTokenType>();
        public IEnumerable<Issue> Issues { get; private set; } = new HashSet<Issue>();

        public EndpointSummary(Endpoint endpoint)
        {
            this.EndpointUrl = endpoint.EndpointUrl;
            this.ServerCertificate = endpoint.ServerCertificate;

            this.MergeEndpoint(endpoint);
        }

        public EndpointSummary MergeEndpoint(Endpoint endpoint)
        {
            this.SecurityPolicyUris.Add(endpoint.SecurityPolicyUri);
            this.SecurityModes.Add(endpoint.SecurityMode);

            this.UserTokenPolicyIds = this.UserTokenPolicyIds.Union(endpoint.UserTokenPolicyIds);
            this.UserTokenTypes = this.UserTokenTypes.Union(endpoint.UserTokenTypes);
            this.Issues = this.Issues.Union(endpoint.Issues);

            return this;
        }
    }
}