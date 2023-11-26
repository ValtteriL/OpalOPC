
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

namespace Tests;
public class SelfSignedCertificatePluginTest
{
    private readonly ILogger _logger;
    private readonly Mock<ISecurityTestSession> _mockSecurityTestSession;
    private readonly Mock<CertificateIdentifier> _mockCertificateIdentifier;
    private readonly SelfSignedCertificatePlugin _plugin;

    public SelfSignedCertificatePluginTest()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        _logger = loggerFactory.CreateLogger<SelfSignedCertificatePluginTest>();
        _mockSecurityTestSession = new Mock<ISecurityTestSession>();
        _mockCertificateIdentifier = new Mock<CertificateIdentifier>();
        _plugin = new SelfSignedCertificatePlugin(_logger);
    }


    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        SessionCredential sessionCredential = new(new UserIdentity(), _mockCertificateIdentifier.Object);
        _mockSecurityTestSession.Setup(session => session.Credential).Returns(sessionCredential);
        _mockSecurityTestSession.Setup(session => session.EndpointUrl).Returns("test");


        // act
        Issue? issue = _plugin.Run(new List<ISecurityTestSession> { _mockSecurityTestSession.Object });

        // assert
        Assert.True(issue == null);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        SessionCredential sessionCredential = new(new UserIdentity(), _mockCertificateIdentifier.Object, true);
        _mockSecurityTestSession.Setup(session => session.Credential).Returns(sessionCredential);
        _mockSecurityTestSession.Setup(session => session.EndpointUrl).Returns("test");

        // act
        Issue? issue = _plugin.Run(new List<ISecurityTestSession> { _mockSecurityTestSession.Object });

        // assert
        Assert.True(issue != null);
    }

}
