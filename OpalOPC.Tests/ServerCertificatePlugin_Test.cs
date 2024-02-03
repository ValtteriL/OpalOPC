
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Security.Certificates;
using Plugin;
using Xunit;

namespace Tests;
public class ServerCertificatePluginTest
{
    private readonly ILogger _logger;
    private readonly ServerCertificatePlugin _plugin;
    private readonly string _discoveryUrl = "opc.tcp://localhost:4840";
    private readonly EndpointDescriptionCollection _endpointDescriptions = [];
    private readonly string datetimeformat = "yyyy-MM-ddTHH";

    public ServerCertificatePluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<ServerCertificatePluginTest>();
        _plugin = new ServerCertificatePlugin(_logger);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        X509Certificate2 cert = CertificateBuilder.Create("CN=Root CA").CreateForRSA();

        EndpointDescription endpointDescription = new()
        {
            ServerCertificate = cert.RawData
        };
        _endpointDescriptions.Add(endpointDescription);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue != null);
        Assert.Contains(cert.Issuer, issue.Name);
        Assert.Contains(cert.Subject, issue.Name);
        Assert.Contains(cert.NotBefore.ToString(datetimeformat), issue.Name);
        Assert.Contains(cert.NotAfter.ToString(datetimeformat), issue.Name);
        Assert.Contains(cert.SerialNumber, issue.Name);
        Assert.Empty(sessions);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        EndpointDescription endpointDescription = new();
        _endpointDescriptions.Add(endpointDescription);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue == null);
        Assert.Empty(sessions);
    }
}
