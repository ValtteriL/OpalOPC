using System.Xml.Serialization;
using Opc.Ua;

namespace Model
{
    public class EndpointSummary
    {
        public string EndpointUrl { get; set; }
        public byte[] ServerCertificate { get; set; }
        [XmlArrayItem("SecurityPolicyUri")]
        public HashSet<string> SecurityPolicyUris { get; private set; } = new HashSet<string>();
        public HashSet<MessageSecurityMode> MessageSecurityModes { get; private set; } = new HashSet<MessageSecurityMode>();
        [XmlArrayItem("UserTokenPolicy")]
        public HashSet<string> UserTokenPolicyIds { get; private set; } = new HashSet<string>();
        public HashSet<UserTokenType> UserTokenTypes { get; private set; } = new HashSet<UserTokenType>();
        public HashSet<Issue> Issues { get; private set; } = new HashSet<Issue>();

        // parameterless constructor for XML serializer
        internal EndpointSummary()
        {}

        public EndpointSummary(Endpoint endpoint)
        {
            this.EndpointUrl = endpoint.EndpointUrl;
            this.ServerCertificate = endpoint.ServerCertificate;

            this.MergeEndpoint(endpoint);
        }

        public EndpointSummary MergeEndpoint(Endpoint endpoint)
        {
            this.SecurityPolicyUris.Add(endpoint.SecurityPolicyUri);
            this.MessageSecurityModes.Add(endpoint.SecurityMode);

            this.UserTokenPolicyIds = this.UserTokenPolicyIds.Union(endpoint.UserTokenPolicyIds).ToHashSet();
            this.UserTokenTypes = this.UserTokenTypes.Union(endpoint.UserTokenTypes).ToHashSet();
            this.Issues = this.Issues.Union(endpoint.Issues).ToHashSet();

            return this;
        }
    }
}