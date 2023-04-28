using System.Collections.Generic;
using System.Text.Json.Serialization;
using Opc.Ua;
namespace Model
{
    public class Server
    {

        public string DiscoveryUrl { get; }

        [JsonIgnore]
        public ICollection<Endpoint> SeparatedEndpoints { get; } = new List<Endpoint>();
        public ICollection<EndpointSummary> Endpoints { get; private set; } = new List<EndpointSummary>();

        public Server(string DiscoveryUrl, EndpointDescriptionCollection edc)
        {
            this.DiscoveryUrl = DiscoveryUrl;
            foreach (EndpointDescription e in edc)
            {
                this.SeparatedEndpoints.Add(new Endpoint(e));
            }
        }

        // Merge SeparatedEndpoints into Endpointsummaries by endpointUrls
        public void MergeEndpoints()
        {
            Dictionary<string, EndpointSummary> endpointDictionary = new Dictionary<string, EndpointSummary>();

            foreach (Endpoint endpoint in SeparatedEndpoints)
            {
                if(endpointDictionary.ContainsKey(endpoint.EndpointUrl))
                {
                    endpointDictionary[endpoint.EndpointUrl].MergeEndpoint(endpoint);
                    continue;
                }

                endpointDictionary.Add(endpoint.EndpointUrl, new EndpointSummary(endpoint));
            }

            this.Endpoints = endpointDictionary.Values;
        }

    }
}