using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Opc.Ua;
namespace Model
{
    public class Server
    {

        public string? DiscoveryUrl { get; set; }
        public List<Error> Errors { get; set; } = new List<Error>();

        [JsonIgnore]
        public ICollection<Endpoint> SeparatedEndpoints { get; } = new List<Endpoint>();

        [XmlArrayItem("Endpoint")]
        public List<EndpointSummary> Endpoints { get; private set; } = new List<EndpointSummary>();

        // parameterless constructor for XML serializer
        internal Server()
        { }

        public Server(string DiscoveryUrl, EndpointDescriptionCollection edc)
        {
            this.DiscoveryUrl = DiscoveryUrl;
            foreach (EndpointDescription e in edc)
            {
                this.SeparatedEndpoints.Add(new Endpoint(e));
            }
        }

        public void AddError(Error error)
        {
            this.Errors.Add(error);
        }

        // Merge SeparatedEndpoints into Endpointsummaries by endpointUrls
        public void MergeEndpoints()
        {
            Dictionary<string, EndpointSummary> endpointDictionary = new();

            foreach (Endpoint endpoint in SeparatedEndpoints)
            {
                if (endpointDictionary.ContainsKey(endpoint.EndpointUrl))
                {
                    endpointDictionary[endpoint.EndpointUrl].MergeEndpoint(endpoint);
                    continue;
                }

                endpointDictionary.Add(endpoint.EndpointUrl, new EndpointSummary(endpoint));
            }

            this.Endpoints = endpointDictionary.Values.ToList();
        }

    }
}
