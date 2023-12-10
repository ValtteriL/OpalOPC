
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

namespace Tests;
public class AnonymousAuthenticationPluginTest
{
    private readonly ILogger _logger;
    private readonly Mock<IConnectionUtil> _mockConnectionUtil;
    private readonly Mock<ISecurityTestSession> _mockSession;
    private readonly string _discoveryUrl = "opc.tcp://localhost:4840";
    private readonly EndpointDescriptionCollection _endpointDescriptions = new();

    public AnonymousAuthenticationPluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<AnonymousAuthenticationPluginTest>();
        _mockConnectionUtil = new Mock<IConnectionUtil>();
        _mockSession = new Mock<ISecurityTestSession>();
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange

        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Certificate) })
        };
        Endpoint endpoint = new(endpointDescription);

        AnonymousAuthenticationPlugin plugin = new(_logger, _mockConnectionUtil.Object, new AuthenticationData());
        _endpointDescriptions.Add(endpointDescription);


        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Never());
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Never());
        Assert.True(issue == null);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Anonymous) }),
            SecurityPolicyUri = SecurityPolicies.None
        };
        Endpoint endpoint = new(endpointDescription);
        _endpointDescriptions.Add(endpointDescription);

        // StartSession should return a dummy session
        _mockConnectionUtil.Setup(conn => conn.StartSession(endpointDescription, It.IsAny<UserIdentity>()).Result).Returns(_mockSession.Object);

        AnonymousAuthenticationPlugin plugin = new(_logger, _mockConnectionUtil.Object, new AuthenticationData());

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue != null);
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Once);
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Never());
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == 1);
    }

    [Fact]
    public void FailureToOpenSessionFailsGracefully()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Anonymous) }),
            SecurityPolicyUri = SecurityPolicies.None
        };
        _endpointDescriptions.Add(endpointDescription);

        _mockConnectionUtil.Setup(conn => conn.StartSession(endpointDescription, It.IsAny<UserIdentity>()).Result).Throws(new Exception());

        AnonymousAuthenticationPlugin plugin = new(_logger, _mockConnectionUtil.Object, new AuthenticationData());

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Once);
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ApplicationCertificatesAreTried()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Anonymous) }),
            SecurityPolicyUri = SecurityPolicies.Basic128Rsa15
        };
        _endpointDescriptions.Add(endpointDescription);

        // StartSession should return a dummy session
        _mockConnectionUtil.Setup(conn => conn.StartSession(endpointDescription, It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()).Result).Returns(_mockSession.Object);

        AuthenticationData authenticationData = new();
        var mockCertIdentifier = new Mock<CertificateIdentifier>();
        authenticationData.applicationCertificates.Add(mockCertIdentifier.Object);

        AnonymousAuthenticationPlugin plugin = new(_logger, _mockConnectionUtil.Object, authenticationData);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Never);
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Once);
        Assert.True(issue != null);
        Assert.True(sessions.Count == 1);
    }

}
