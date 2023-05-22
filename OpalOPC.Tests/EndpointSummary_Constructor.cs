namespace Tests;
using Model;
using Xunit;

public class EndpointSummary_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        Opc.Ua.EndpointDescription description = new Opc.Ua.EndpointDescription();
        Endpoint endpoint = new Endpoint(new Opc.Ua.EndpointDescription());
        EndpointSummary endpointSummary = new EndpointSummary(endpoint);

        Assert.True(endpoint != null);
    }

    [Fact]
    public void constructor_NullInputReturnsNullReferenceException()
    {
        Endpoint? endpoint = null;

        try
        {
            EndpointSummary endpointSummary = new EndpointSummary(endpoint!);
        }
        catch (System.NullReferenceException)
        {
            Assert.True(true);
            return;   
        }

        Assert.True(false);
    }

    [Fact]
    public void constructor_SetsProperties()
    {
        Opc.Ua.EndpointDescription description = new Opc.Ua.EndpointDescription();
        string endpointUrl = "endpointUrl";
        string securityPolicyUrl = "securityPolicyUrl";
        byte[] serverCertificate = new byte[] { 1 };
        description.EndpointUrl = endpointUrl;
        description.SecurityPolicyUri = securityPolicyUrl;
        description.SecurityMode = Opc.Ua.MessageSecurityMode.None;
        description.ServerCertificate = serverCertificate;
        Endpoint endpoint = new Endpoint(description);

        EndpointSummary endpointSummary = new EndpointSummary(endpoint);

        Assert.True(endpointSummary.EndpointUrl == endpointUrl);
        Assert.Contains(securityPolicyUrl, endpointSummary.SecurityPolicyUris);
        Assert.Contains(Opc.Ua.MessageSecurityMode.None, endpointSummary.MessageSecurityModes);
        Assert.True(endpointSummary.ServerCertificate == serverCertificate);
        Assert.Empty(endpointSummary.Issues);
        Assert.Empty(endpointSummary.UserTokenTypes);
        Assert.Empty(endpointSummary.UserTokenPolicyIds);

    }
}