
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Security.Certificates;
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
    public void ReportsIssuesNotTrusted()
    {
        // arrange

        EndpointDescription endpointDescription = new()
        {
            ServerCertificate = CertificateBuilder.Create("CN=Root CA").CreateForRSA().RawData
    };
        _endpointDescriptions.Add(endpointDescription);


        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue != null);
        Assert.Contains("not trusted", issue.Name);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssuesExpired()
    {
        // arrange
        X509Certificate2 cert = CertificateBuilder.Create("CN=Root CA").SetNotAfter(DateTime.Now - TimeSpan.FromDays(1)).CreateForRSA();

        EndpointDescription endpointDescription = new()
        {
            ServerCertificate = cert.RawData
        };
        _endpointDescriptions.Add(endpointDescription);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue != null);
        Assert.Contains("expired", issue.Name);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssuesNotYetValid()
    {
        // arrange
        X509Certificate2 cert = CertificateBuilder.Create("CN=Root CA").SetNotBefore(DateTime.Now + TimeSpan.FromDays(1)).CreateForRSA();

        EndpointDescription endpointDescription = new()
        {
            ServerCertificate = cert.RawData
        };
        _endpointDescriptions.Add(endpointDescription);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue != null);
        Assert.Contains("not yet valid", issue.Name);
        Assert.Empty(sessions);
    }

}
