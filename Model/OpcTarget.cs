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

        public IEnumerable<Endpoint> GetEndpointsBySecurityPolicyUriNot(string securityPolicyUri)
        {
            return TargetServers.SelectMany(s => s.Endpoints.Where(e => e.SecurityPolicyUri != securityPolicyUri));
        }

        public override string ToString()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(this, options);
        }
    }
}