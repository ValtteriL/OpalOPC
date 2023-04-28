using Opc.Ua;
namespace Model
{
    public class Target
    {

        private ApplicationDescription _applicationDescription;
        public List<Server> Servers { get; set; }

        public ApplicationType Type { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationUri { get; set; }
        public string ProductUri { get; set; }


        // parameterless constructor for XML serializer
        internal Target()
        {}

        public Target(ApplicationDescription ad)
        {
            this._applicationDescription = ad;

            this.Type = ad.ApplicationType;
            this.ApplicationName = ad.ApplicationName.ToString();
            this.ApplicationUri = ad.ApplicationUri;
            this.ProductUri = ad.ProductUri;

            this.Servers = new List<Server>();
        }

        public void AddServer(string DiscoveryUrl, EndpointDescriptionCollection edc)
        {
            this.Servers.Add(new Server(DiscoveryUrl, edc));
        }

        public IEnumerable<Endpoint> GetEndpointsBySecurityMode(MessageSecurityMode messageSecurityMode)
        {
            return Servers.SelectMany(s => s.SeparatedEndpoints.Where(e => e.SecurityMode == messageSecurityMode));
        }

        public IEnumerable<Endpoint> GetEndpointsByUserTokenType(UserTokenType userTokenType)
        {
            return Servers.SelectMany(s => s.SeparatedEndpoints.Where(e => e.UserTokenTypes.Contains(userTokenType)));
        }

        public IEnumerable<Endpoint> GetEndpointsBySecurityPolicyUri(string securityPolicyUri)
        {
            return Servers.SelectMany(s => s.SeparatedEndpoints.Where(e => e.SecurityPolicyUri == securityPolicyUri));
        }

        public IEnumerable<Endpoint> GetEndpointsBySecurityPolicyUriNot(string securityPolicyUri)
        {
            return Servers.SelectMany(s => s.SeparatedEndpoints.Where(e => e.SecurityPolicyUri != securityPolicyUri));
        }

        // Get bruteable endpoints = username + application authentication is disabled OR self-signed certificates accepted
        public IEnumerable<Endpoint> GetBruteableEndpoints()
        {
            return this.GetEndpointsByUserTokenType(UserTokenType.UserName)
                .Where(e => e.Issues.Contains(Issues.SecurityModeNone)
                    || e.Issues.Contains(Issues.SelfSignedCertificateAccepted));
        }

        // Get endpoints that can be logged into = anonymous or username + application authentication is disabled OR self-signed certificates accepted
        public IEnumerable<Endpoint> GetLoginSuccessfulEndpoints()
        {
            return this.GetEndpointsByUserTokenType(UserTokenType.UserName)
                .Concat(this.GetEndpointsByUserTokenType(UserTokenType.Anonymous))
                .Where(e => e.Issues.Contains(Issues.SecurityModeNone)
                    || e.Issues.Contains(Issues.SelfSignedCertificateAccepted));
        }

        // Merge endpoints with identical URI, add up their findings
        public void MergeEndpoints()
        {
            foreach (Server server in Servers)
            {
                server.MergeEndpoints();
            }
        }
    }
}