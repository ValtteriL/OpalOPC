using Opc.Ua;
namespace Model
{
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

        public void MergeEndpoints()
        {
            foreach (Endpoint endpoint in Endpoints)
            {
                // TODO: create a new object for every unique EndpointUrl and collect there:
                // - SecurityPolicyUris
                // - SecurityModes
                // - UserTokenPolicyIds
                // - UserTokenTypes
                // - ServerCertificate
                // - EndpointUrl

                // lsit of ^^ shall be visible in the JSON instead of targetServers
            }
        }

    }
}