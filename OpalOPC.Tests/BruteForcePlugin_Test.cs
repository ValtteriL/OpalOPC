
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
public class BruteForcePluginTest
{
    private readonly ILogger _logger;
    private readonly Mock<IConnectionUtil> _mockConnectionUtil;
    private readonly Mock<ISecurityTestSession> _mockSession;
    private readonly Mock<ISecurityTestSession> _mockSessionSuccess;
    private readonly int _expectedConnectionAttempts;
    private readonly int _expectedConnectionAttemptsWithAppCerts;
    private readonly string _discoveryUrl = "opc.tcp://localhost:4840";
    private readonly EndpointDescriptionCollection _endpointDescriptions = new();
    private readonly AuthenticationData _authenticationData = new()
    {
        bruteForceCredentials = new List<(string, string)> {
                ("username", "password"),
                ("username2", "password2")
            },
        applicationCertificates = new List<CertificateIdentifier>
        {
                new CertificateIdentifier(CertificateBuilder.Create("CN=Test").CreateForRSA()),
                new CertificateIdentifier(CertificateBuilder.Create("CN=Test").CreateForRSA())
        },
        userCertificates = new List<CertificateIdentifier>
        {
                new CertificateIdentifier(CertificateBuilder.Create("CN=Test").CreateForRSA()),
                new CertificateIdentifier(CertificateBuilder.Create("CN=Test").CreateForRSA())
        }
    };

    public BruteForcePluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<BruteForcePluginTest>();
        _mockConnectionUtil = new Mock<IConnectionUtil>();

        _mockSession = new Mock<ISecurityTestSession>();
        _mockSession.Setup(session => session.Session.Connected).Returns(false);

        _mockSessionSuccess = new Mock<ISecurityTestSession>();
        _mockSessionSuccess.Setup(session => session.Session.Connected).Returns(true);

        _expectedConnectionAttempts = _authenticationData.bruteForceCredentials.Count;
        _expectedConnectionAttemptsWithAppCerts = _expectedConnectionAttempts * _authenticationData.applicationCertificates.Count;
    }


    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        _endpointDescriptions.Add(new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
            SecurityPolicyUri = SecurityPolicies.None,
        });
        _endpointDescriptions.Add(new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
            SecurityPolicyUri = SecurityPolicies.Basic128Rsa15
        });

        // StartSession returns only closed sessions
        _mockSession.Setup(session => session.Session.Connected).Returns(false);
        _mockConnectionUtil.Setup(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>())).Returns(_mockSession.Object);

        BruteForcePlugin plugin = new(_logger, _mockConnectionUtil.Object, _authenticationData);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>()), Times.Exactly(_expectedConnectionAttempts));
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Exactly(_expectedConnectionAttemptsWithAppCerts));
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
            SecurityPolicyUri = SecurityPolicies.None,
        };
        Endpoint endpoint = new(endpointDescription);
        _endpointDescriptions.Add(endpointDescription);

        // StartSession should return a dummy session
        // StartSession returns single open session, then closed sessions
        _mockConnectionUtil.SetupSequence(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>())).Returns(_mockSessionSuccess.Object).Returns(_mockSession.Object);

        BruteForcePlugin plugin = new(_logger, _mockConnectionUtil.Object, _authenticationData);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>()), Times.Exactly(_expectedConnectionAttempts));
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Never);
        Assert.True(issue != null);
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == 1);
    }

    [Fact]
    public void ApplicationCertificatesTriedIfNoSessionsWithout()
    {
        // arrange
        _endpointDescriptions.Add(new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
            SecurityPolicyUri = SecurityPolicies.None,
        });
        _endpointDescriptions.Add(new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
            SecurityPolicyUri = SecurityPolicies.Basic128Rsa15,
        });

        // StartSession should return a dummy session
        // StartSession returns single open session, then closed sessions
        _mockConnectionUtil.SetupSequence(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>())).Returns(_mockSession.Object).Returns(_mockSession.Object); // return closed session if no app certificate
        _mockConnectionUtil.SetupSequence(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>())).Returns(_mockSession.Object).Returns(_mockSessionSuccess.Object); // return successful session for app certificate

        var MockCertificateIdentifier = new Mock<CertificateIdentifier>();

        BruteForcePlugin plugin = new(_logger, _mockConnectionUtil.Object, _authenticationData);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>()), Times.Exactly(_expectedConnectionAttempts));
        _mockConnectionUtil.Verify(conn => conn.AttemptLogin(It.IsAny<Endpoint>(), It.IsAny<UserIdentity>(), It.IsAny<CertificateIdentifier>()), Times.Exactly(_expectedConnectionAttemptsWithAppCerts));
        Assert.True(issue != null);
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == 1);
    }

}
