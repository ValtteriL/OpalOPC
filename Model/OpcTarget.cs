using System.Text.Json;
using Opc.Ua;
namespace Model
{
    public class OpcTarget
    {

        private ApplicationDescription _applicationDescription;
        public ICollection<Server> TargetServers { get; }

        public ApplicationType Type { get; }
        public string ApplicationName { get; }
        public string ApplicationUri { get; }
        public string ProductUri { get; }


        public OpcTarget(ApplicationDescription ad)
        {
            this._applicationDescription = ad;

            this.Type = ad.ApplicationType;
            this.ApplicationName = ad.ApplicationName.ToString();
            this.ApplicationUri = ad.ApplicationUri;
            this.ProductUri = ad.ProductUri;

            this.TargetServers = new List<Server>();
        }

        public void AddServer(string DiscoveryUrl, EndpointDescriptionCollection edc)
        {
            this.TargetServers.Add(new Server(DiscoveryUrl, edc));
        }

        public IEnumerable<Endpoint> GetEndpointsBySecurityMode(MessageSecurityMode messageSecurityMode)
        {
            return TargetServers.SelectMany(s => s.Endpoints.Where(e => e.SecurityMode == messageSecurityMode));
        }

        public IEnumerable<Endpoint> GetEndpointsByUserTokenType(UserTokenType userTokenType)
        {
            return TargetServers.SelectMany(s => s.Endpoints.Where(e => e.UserTokenTypes.Contains(userTokenType)));
        }

        public IEnumerable<Endpoint> GetEndpointsBySecurityPolicyUri(string securityPolicyUri)
        {
            return TargetServers.SelectMany(s => s.Endpoints.Where(e => e.SecurityPolicyUri == securityPolicyUri));
        }

        public override string ToString()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(this, options);
        }

        public class Server
        {
            public string DiscoveryUrl { get; }
            public ICollection<Endpoint> Endpoints { get; }

            public Server(string DiscoveryUrl, EndpointDescriptionCollection edc)
            {
                this.DiscoveryUrl = DiscoveryUrl;
                this.Endpoints = new List<Endpoint>();
                foreach (EndpointDescription e in edc)
                {
                    this.Endpoints.Add(new Endpoint(e));
                }
            }
        }

        public class Endpoint
        {
            public EndpointDescription EndpointDescription;

            public string EndpointUrl { get; }
            public string SecurityPolicyUri { get; }
            public MessageSecurityMode SecurityMode { get; }
            private byte[] ServerCertificate { get; }
            public ICollection<string> UserTokenPolicyIds { get; }
            public ICollection<UserTokenType> UserTokenTypes { get; }


            public Endpoint(EndpointDescription e)
            {
                this.EndpointDescription = e;

                this.EndpointUrl = e.EndpointUrl;
                this.SecurityPolicyUri = e.SecurityPolicyUri;
                this.SecurityMode = e.SecurityMode;
                this.ServerCertificate = e.ServerCertificate;

                this.UserTokenPolicyIds = new List<string>();
                this.UserTokenTypes = new List<UserTokenType>();
                foreach (UserTokenPolicy utp in e.UserIdentityTokens)
                {
                    this.UserTokenPolicyIds.Add(utp.PolicyId);
                    this.UserTokenTypes.Add(utp.TokenType);
                }
            }
        }

    }
}