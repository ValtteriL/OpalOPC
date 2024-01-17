using Model;
using Opc.Ua;
using Xunit;

namespace Tests;
public class Server_Constructor
{

    [Fact]
    public void constructor_SetsProperties()
    {
        string discoveryUrl = "a";
        EndpointDescriptionCollection edc = [];
        Server server = new(discoveryUrl, edc);

        Assert.True(server.DiscoveryUrl == discoveryUrl);
        Assert.True(server.EndpointDescriptions == edc);
        Assert.True(server.Errors != null);
        Assert.True(server.Errors.Count == 0);
        Assert.True(server.Issues != null);
        Assert.True(server.Issues.Count == 0);
    }
}
