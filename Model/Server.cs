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

    }
}