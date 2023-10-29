namespace Tests;
using Model;
using Xunit;

public class Endpoint_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        Opc.Ua.EndpointDescription description = new();
        Endpoint endpoint = new(description);

        Assert.True(endpoint != null);
    }

    [Fact]
    public void constructor_SetsProperties()
    {
        Opc.Ua.EndpointDescription description = new();
        string endpointUrl = "endpointUrl";
        string securityPolicyUrl = "securityPolicyUrl";
        byte[] serverCertificate = new byte[] { 1 };
        description.EndpointUrl = endpointUrl;
        description.SecurityPolicyUri = securityPolicyUrl;
        description.SecurityMode = Opc.Ua.MessageSecurityMode.None;
        description.ServerCertificate = serverCertificate;

        Endpoint endpoint = new(description);

        Assert.True(endpoint.EndpointDescription == description);
        Assert.True(endpoint.EndpointUrl == endpointUrl);
        Assert.True(endpoint.SecurityPolicyUri == securityPolicyUrl);
        Assert.True(endpoint.SecurityMode == Opc.Ua.MessageSecurityMode.None);
        Assert.True(endpoint.ServerCertificate == serverCertificate);

    }
}