namespace Tests;
using Model;
using Opc.Ua;
using Xunit;

public class Server_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        string discoveryUrl = "a";
        EndpointDescriptionCollection edc = new();
        Server server = new(discoveryUrl, edc);

        Assert.True(server != null);
    }

    [Fact]
    public void constructor_SetsProperties()
    {
        string discoveryUrl = "a";
        EndpointDescriptionCollection edc = new();
        Server server = new(discoveryUrl, edc);

        Assert.True(server.DiscoveryUrl == discoveryUrl);
        foreach (EndpointDescription ed in edc)
        {
            Assert.Contains(new Endpoint(ed), server.SeparatedEndpoints);
        }
        Assert.True(server.Endpoints != null);
        Assert.True(server.Errors != null);
        Assert.True(server.Errors.Count == 0);
    }
}
