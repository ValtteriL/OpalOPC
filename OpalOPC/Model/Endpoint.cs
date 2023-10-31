
using Opc.Ua;

namespace Model
{
    public class Endpoint
    {
        public EndpointDescription EndpointDescription;

        public string EndpointUrl { get; }
        public string SecurityPolicyUri { get; }
        public MessageSecurityMode SecurityMode { get; }
        public byte[] ServerCertificate { get; }
        public ICollection<string> UserTokenPolicyIds { get; } = new List<string>();
        public ICollection<UserTokenType> UserTokenTypes { get; } = new List<UserTokenType>();

        public ICollection<Issue> Issues { get; set; } = new List<Issue>();

        public Endpoint(EndpointDescription e)
        {
            EndpointDescription = e;

            EndpointUrl = e.EndpointUrl;
            SecurityPolicyUri = e.SecurityPolicyUri;
            SecurityMode = e.SecurityMode;
            ServerCertificate = e.ServerCertificate;

            foreach (UserTokenPolicy utp in e.UserIdentityTokens)
            {
                UserTokenPolicyIds.Add(utp.PolicyId);
                UserTokenTypes.Add(utp.TokenType);
            }
        }
    }
}
