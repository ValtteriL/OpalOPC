namespace Tests;
using Model;
using Opc.Ua;
using Xunit;

public class Server_AddError
{
    [Fact]
    public void AddError_InputErrorIsAddedToErrors()
    {
        string discoveryUrl = "a";
        EndpointDescriptionCollection edc = new EndpointDescriptionCollection();
        Server server = new Server(discoveryUrl, edc);
        Error error = new Error("");

        server.AddError(error);

        Assert.True(server.Errors.Count == 1);
        Assert.Contains(error, server.Errors);
    }
}
