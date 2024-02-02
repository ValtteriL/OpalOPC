
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Plugin;
using Xunit;

namespace Tests;
public class ServerCertificateInvalidPluginTest
{
    private readonly ILogger _logger;
    private readonly ServerCertificateInvalidPlugin _plugin;
    private readonly string _discoveryUrl = "opc.tcp://localhost:4840";
    private readonly EndpointDescriptionCollection _endpointDescriptions = [];

    public ServerCertificateInvalidPluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<ServerCertificateInvalidPluginTest>();
        _plugin = new ServerCertificateInvalidPlugin(_logger);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange

        EndpointDescription endpointDescription = new()
        {
            ServerCertificate = new 
        };
        _endpointDescriptions.Add(endpointDescription);


        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue == null);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            ServerCertificate = new
        };
        _endpointDescriptions.Add(endpointDescription);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

}
