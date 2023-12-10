
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

        // Check if endpoint is bruteable = username + application authentication is disabled OR self-signed certificates accepted
        // we can only test if username authentication is enabled - we can't test if self-signed certificates are accepted
        // this means that we may try to brute a non-bruteable endpoint, but we will not miss any bruteable endpoints
        public bool IsBruteable()
        {
            return UserTokenTypes.Contains(UserTokenType.UserName);
        }
    }
}
