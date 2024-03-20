
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Plugin;
using Util;
using Xunit;

namespace Tests;
public class SelfSignedUserCertificatePluginTest
{
    private readonly Mock<ILogger> _mockLogger = new();
    private readonly Mock<ISelfSignedCertificateUtil> _mockSelfSignedCertificateUtil = new();
    private readonly Mock<IConnectionUtil> _mockConnectionUtil = new();
    private readonly EndpointDescriptionCollection _endpointDescriptions = [];
    private readonly Mock<ISecurityTestSession> _mockSession = new();
    private readonly string _discoveryUrl = "opc.tcp://localhost:4840";

    public SelfSignedUserCertificatePluginTest()
    {
        _mockSelfSignedCertificateUtil.Setup(c => c.GetCertificate()).Returns(new CertificateIdentifier());
    }


    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection([new(UserTokenType.UserName)]) // not UserTokenType.Certificate
        };
        _endpointDescriptions.Add(endpointDescription);

        SelfSignedUserCertificatePlugin plugin = new(_mockLogger.Object, _mockSelfSignedCertificateUtil.Object, _mockConnectionUtil.Object, new AuthenticationData());


        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Never);
        Assert.True(issue == null);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection([new(UserTokenType.Certificate)]),
            SecurityPolicyUri = SecurityPolicies.None
        };
        Endpoint endpoint = new(endpointDescription);
        _endpointDescriptions.Add(endpointDescription);

        _mockConnectionUtil.Setup(conn => conn.StartSession(endpointDescription, It.IsAny<UserIdentity>()).Result).Returns(_mockSession.Object);

        SelfSignedUserCertificatePlugin plugin = new(_mockLogger.Object, _mockSelfSignedCertificateUtil.Object, _mockConnectionUtil.Object, new AuthenticationData());

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue != null);
        _mockConnectionUtil.Verify(conn => conn.StartSession(endpointDescription, It.IsAny<UserIdentity>()), Times.Once);
        _mockConnectionUtil.Verify(conn => conn.StartSession(endpointDescription, It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Never());
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == 1);
    }

    [Fact]
    public void FailureToOpenSessionFailsGracefully()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection([new(UserTokenType.Certificate)]),
            SecurityPolicyUri = SecurityPolicies.None
        };
        _endpointDescriptions.Add(endpointDescription);

        _mockConnectionUtil.Setup(conn => conn.StartSession(endpointDescription, It.IsAny<UserIdentity>()).Result).Throws(new Exception());

        SelfSignedUserCertificatePlugin plugin = new(_mockLogger.Object, _mockSelfSignedCertificateUtil.Object, _mockConnectionUtil.Object, new AuthenticationData());

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Once);
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

}
