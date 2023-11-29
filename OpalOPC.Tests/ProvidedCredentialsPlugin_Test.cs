
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Security.Certificates;
using Plugin;
using Util;
using Xunit;

namespace Tests;
public class ProvidedCredentialsPluginTest
{
    private readonly ILogger _logger;
    private readonly Mock<IConnectionUtil> _mockConnectionUtil;
    private readonly Mock<ISecurityTestSession> _mockSession;
    private readonly Mock<ISecurityTestSession> _mockSessionSuccess;
    private readonly Endpoint _endpoint;
    private readonly int _expectedConnectionAttemptsWithAppCert;
    private readonly int _expectedConnectionAttempts;
    private readonly AuthenticationData _authenticationData = new()
    {
        loginCredentials = new List<(string, string)> {
                ("username", "password"),
                ("username2", "password2")
            },
        applicationCertificates = new List<CertificateIdentifier>
        {
                new(CertificateBuilder.Create("CN=Test").CreateForRSA()),
                new(CertificateBuilder.Create("CN=Test").CreateForRSA())
        },
        userCertificates = new List<CertificateIdentifier>
        {
                new(CertificateBuilder.Create("CN=Test").CreateForRSA()),
                new(CertificateBuilder.Create("CN=Test").CreateForRSA())
        }
    };

    public ProvidedCredentialsPluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<ProvidedCredentialsPluginTest>();
        _mockConnectionUtil = new Mock<IConnectionUtil>();

        _mockSession = new Mock<ISecurityTestSession>();
        _mockSession.Setup(session => session.Session.Connected).Returns(false);

        _mockSessionSuccess = new Mock<ISecurityTestSession>();
        _mockSessionSuccess.Setup(session => session.Session.Connected).Returns(true);

        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
            SecurityMode = MessageSecurityMode.None
        };
        _endpoint = new(endpointDescription);

        _expectedConnectionAttempts = _authenticationData.loginCredentials.Count + _authenticationData.userCertificates.Count;
        _expectedConnectionAttemptsWithAppCert = (_authenticationData.loginCredentials.Count + _authenticationData.userCertificates.Count) * _authenticationData.applicationCertificates.Count;
    }


    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        _mockConnectionUtil.Setup(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>())).Returns(_mockSession.Object);

        ProvidedCredentialsPlugin plugin = new(_logger, _mockConnectionUtil.Object, _authenticationData);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_endpoint);

        // assert
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>()), Times.Exactly(_expectedConnectionAttempts));
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(
            It.IsAny<Endpoint>(),
            It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()),
            Times.Exactly(_expectedConnectionAttemptsWithAppCert));
        Assert.True(issue == null);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange

        // StartSession should return a dummy session
        // StartSession returns single open session, then closed sessions
        _mockConnectionUtil.Setup(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>())).Returns(_mockSessionSuccess.Object);

        ProvidedCredentialsPlugin plugin = new(_logger, _mockConnectionUtil.Object, _authenticationData);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_endpoint);

        // assert
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>()), Times.Exactly(_expectedConnectionAttempts));
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Never); // not tried as success seen without
        Assert.True(issue != null);
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == _expectedConnectionAttempts);
    }

    [Fact]
    public void AppCertsTriedOnlyIfNoSuccessWithout()
    {
        // arrange

        // StartSession should return a dummy session
        // StartSession returns single open session, then closed sessions
        _mockConnectionUtil.Setup(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>())).Returns(_mockSession.Object);
        _mockConnectionUtil.Setup(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>())).Returns(_mockSessionSuccess.Object);

        ProvidedCredentialsPlugin plugin = new(_logger, _mockConnectionUtil.Object, _authenticationData);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_endpoint);

        // assert
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>()), Times.Exactly(_expectedConnectionAttempts));
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Exactly(_expectedConnectionAttemptsWithAppCert));
        Assert.True(issue != null);
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == _expectedConnectionAttemptsWithAppCert);
    }

}
