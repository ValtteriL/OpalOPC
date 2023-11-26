
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

namespace Tests;
public class CommonCredentialsPluginTest
{
    private readonly ILogger _logger;
    private readonly Mock<IConnectionUtil> _mockConnectionUtil;
    private readonly Mock<ISecurityTestSession> _mockSession;
    private readonly Mock<ISecurityTestSession> _mockSessionSuccess;

    public CommonCredentialsPluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<CommonCredentialsPluginTest>();
        _mockConnectionUtil = new Mock<IConnectionUtil>();

        _mockSession = new Mock<ISecurityTestSession>();
        _mockSession.Setup(session => session.Session.Connected).Returns(false);

        _mockSessionSuccess = new Mock<ISecurityTestSession>();
        _mockSessionSuccess.Setup(session => session.Session.Connected).Returns(true);
    }


    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
            SecurityMode = MessageSecurityMode.None
        };
        Endpoint endpoint = new(endpointDescription);

        // StartSession returns only closed sessions
        _mockSession.Setup(session => session.Session.Connected).Returns(false);
        _mockConnectionUtil.Setup(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()).Result).Returns(_mockSession.Object);

        CommonCredentialsPlugin plugin = new(_logger, _mockConnectionUtil.Object, new AuthenticationData());

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(endpoint);

        // assert
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Exactly(Util.Credentials.CommonCredentials.Count));
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Never);
        Assert.True(issue == null);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
            SecurityMode = MessageSecurityMode.None
        };
        Endpoint endpoint = new(endpointDescription);

        // StartSession should return a dummy session
        // StartSession returns single open session, then closed sessions
        _mockConnectionUtil.SetupSequence(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()).Result).Returns(_mockSessionSuccess.Object).Returns(_mockSession.Object);

        CommonCredentialsPlugin plugin = new(_logger, _mockConnectionUtil.Object, new AuthenticationData());

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(endpoint);

        // assert
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Exactly(Util.Credentials.CommonCredentials.Count));
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Never);
        Assert.True(issue != null);
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == 1);
    }

    [Fact]
    public void ApplicationCertificatesTriedIfNoIssuesWithout()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
            SecurityMode = MessageSecurityMode.None
        };
        Endpoint endpoint = new(endpointDescription);

        // StartSession should return a dummy session
        // StartSession returns single open session, then closed sessions
        _mockConnectionUtil.SetupSequence(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()).Result).Returns(_mockSession.Object).Returns(_mockSession.Object); // return closed session if no app certificate
        _mockConnectionUtil.SetupSequence(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()).Result).Returns(_mockSession.Object).Returns(_mockSessionSuccess.Object); // return successful session for app certificate

        var MockCertificateIdentifier = new Mock<CertificateIdentifier>();
        AuthenticationData authenticationData = new();
        authenticationData.applicationCertificates.Add(MockCertificateIdentifier.Object);

        CommonCredentialsPlugin plugin = new(_logger, _mockConnectionUtil.Object, authenticationData);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(endpoint);

        // assert
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Exactly(Util.Credentials.CommonCredentials.Count));
        _mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Exactly(Util.Credentials.CommonCredentials.Count));
        Assert.True(issue != null);
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == 1);
    }

}
