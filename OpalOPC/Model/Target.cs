using Opc.Ua;
using Plugin;

namespace Model
{
    public class Target
    {

        private ApplicationDescription? _applicationDescription;
        public List<Server> Servers { get; set; } = new List<Server>();
        public List<Error> Errors { get; set; } = new List<Error>();

        public ApplicationType? Type { get; set; }
        public string? ApplicationName { get; set; }
        public string? ApplicationUri { get; set; }
        public string? ProductUri { get; set; }


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

        public void AddError(string message)
        {
            this.Errors.Add(new Error(message));
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
                .Where(e => e.Issues.Any(i => i.PluginId == (int)PluginId.SecurityModeNone)
                    || e.Issues.Any(i => i.PluginId == (int)PluginId.SelfSignedCertificate));
        }

        // Get endpoints that can be logged into = anonymous or username + application authentication is disabled OR self-signed certificates accepted
        public IEnumerable<Endpoint> GetLoginSuccessfulEndpoints()
        {
            return this.GetEndpointsByUserTokenType(UserTokenType.UserName)
                .Concat(this.GetEndpointsByUserTokenType(UserTokenType.Anonymous))
                .Where(e => e.Issues.Any(i => i.PluginId == (int)PluginId.SecurityModeNone)
                    || e.Issues.Any(i => i.PluginId == (int)PluginId.SelfSignedCertificate));
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