
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

    public SelfSignedCertificatePluginTest()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        _logger = loggerFactory.CreateLogger<SelfSignedCertificatePluginTest>();
    }


    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange

        // act
        SelfSignedCertificatePlugin plugin = new(_logger);

        // assert
        Assert.True(plugin != null);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        var mockSecurityTestSession = new Mock<ISecurityTestSession>();
        var mockCertificateIdentifier = new Mock<CertificateIdentifier>();
        SessionCredential sessionCredential = new(new UserIdentity(), mockCertificateIdentifier.Object);
        mockSecurityTestSession.Setup(session => session.Credential).Returns(sessionCredential);
        mockSecurityTestSession.Setup(session => session.EndpointUrl).Returns("test");

        SelfSignedCertificatePlugin plugin = new(_logger);


        // act
        Issue? issue = plugin.Run(new List<ISecurityTestSession> { mockSecurityTestSession.Object });

        // assert
        Assert.True(issue == null);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        SelfSignedCertificatePlugin plugin = new(_logger);

        var mockSecurityTestSession = new Mock<ISecurityTestSession>();
        var mockCertificateIdentifier = new Mock<CertificateIdentifier>();
        SessionCredential sessionCredential = new(new UserIdentity(), mockCertificateIdentifier.Object, true);
        mockSecurityTestSession.Setup(session => session.Credential).Returns(sessionCredential);
        mockSecurityTestSession.Setup(session => session.EndpointUrl).Returns("test");

        // act
        Issue? issue = plugin.Run(new List<ISecurityTestSession> { mockSecurityTestSession.Object });

        // assert
        Assert.True(issue != null);
    }

}
