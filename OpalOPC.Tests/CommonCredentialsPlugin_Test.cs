
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
    private readonly string _discoveryUrl = "opc.tcp://localhost:4840";
    private readonly EndpointDescriptionCollection _endpointDescriptions = new();

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
            SecurityPolicyUri = SecurityPolicies.None
        };
        Endpoint endpoint = new(endpointDescription);
        _endpointDescriptions.Add(endpointDescription);

        // StartSession returns only closed sessions
        _mockSession.Setup(session => session.Session.Connected).Returns(false);
        _mockConnectionUtil.Setup(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>())).Returns(_mockSession.Object);

        CommonCredentialsPlugin plugin = new(_logger, _mockConnectionUtil.Object, new AuthenticationData());

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>()), Times.Exactly(Util.Credentials.CommonCredentials.Count));
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Never);
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
            SecurityPolicyUri = SecurityPolicies.None
        };
        Endpoint endpoint = new(endpointDescription);
        _endpointDescriptions.Add(endpointDescription);

        // StartSession should return a dummy session
        // StartSession returns single open session, then closed sessions
        _mockConnectionUtil.SetupSequence(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>())).Returns(_mockSessionSuccess.Object).Returns(_mockSession.Object);

        CommonCredentialsPlugin plugin = new(_logger, _mockConnectionUtil.Object, new AuthenticationData());

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>()), Times.Exactly(Util.Credentials.CommonCredentials.Count));
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Never);
        Assert.True(issue != null);
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == 1);
    }

}
