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
        EndpointDescriptionCollection edc = new();
        Server server = new(discoveryUrl, edc);
        Error error = new("");

        server.AddError(error);

        Assert.True(server.Errors.Count == 1);
        Assert.Contains(error, server.Errors);
    }
}
