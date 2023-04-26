
using Opc.Ua;

namespace Model
{
    public class Endpoint
    {
        public EndpointDescription EndpointDescription;

        public string EndpointUrl { get; }
        public string SecurityPolicyUri { get; }
        public MessageSecurityMode SecurityMode { get; }
        private byte[] ServerCertificate { get; }
        public ICollection<string> UserTokenPolicyIds { get; } = new List<string>();
        public ICollection<UserTokenType> UserTokenTypes { get; } = new List<UserTokenType>();

        public ICollection<Issue> Issues { get; set; } = new List<Issue>();

        public Endpoint(EndpointDescription e)
        {
            this.EndpointDescription = e;

            this.EndpointUrl = e.EndpointUrl;
            this.SecurityPolicyUri = e.SecurityPolicyUri;
            this.SecurityMode = e.SecurityMode;
            this.ServerCertificate = e.ServerCertificate;

            foreach (UserTokenPolicy utp in e.UserIdentityTokens)
            {
                this.UserTokenPolicyIds.Add(utp.PolicyId);
                this.UserTokenTypes.Add(utp.TokenType);
            }
        }
    }
}