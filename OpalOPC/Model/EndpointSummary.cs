using System.Xml.Serialization;
using Opc.Ua;

namespace Model
{
    public class EndpointSummary
    {
        public string? EndpointUrl { get; set; }
        public byte[]? ServerCertificate { get; set; }
        [XmlArrayItem("SecurityPolicyUri")]
        public HashSet<string> SecurityPolicyUris { get; private set; } = new HashSet<string>();
        public HashSet<MessageSecurityMode> MessageSecurityModes { get; private set; } = new HashSet<MessageSecurityMode>();
        [XmlArrayItem("UserTokenPolicy")]
        public HashSet<string> UserTokenPolicyIds { get; private set; } = new HashSet<string>();
        public HashSet<UserTokenType> UserTokenTypes { get; private set; } = new HashSet<UserTokenType>();
        public HashSet<Issue> Issues { get; private set; } = new HashSet<Issue>();

        // parameterless constructor for XML serializer
        internal EndpointSummary()
        { }

        public EndpointSummary(Endpoint endpoint)
        {
            EndpointUrl = endpoint.EndpointUrl;
            ServerCertificate = endpoint.ServerCertificate;

            MergeEndpoint(endpoint);
        }

        public EndpointSummary MergeEndpoint(Endpoint endpoint)
        {
            SecurityPolicyUris.Add(endpoint.SecurityPolicyUri);
            MessageSecurityModes.Add(endpoint.SecurityMode);

            UserTokenPolicyIds = UserTokenPolicyIds.Union(endpoint.UserTokenPolicyIds).ToHashSet();
            UserTokenTypes = UserTokenTypes.Union(endpoint.UserTokenTypes).ToHashSet();
            Issues = Issues.Union(endpoint.Issues).DistinctBy(i => i.Name).OrderByDescending(i => i.Severity).ToHashSet();

            return this;
        }
    }
}
