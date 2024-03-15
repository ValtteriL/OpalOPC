
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Xunit;

namespace Tests;
public class AuditingDisabledPluginTest
{

    private readonly ILogger _logger;
    private readonly AuditingDisabledPlugin _plugin;
    private readonly Mock<ISession> _mockSession;

    public AuditingDisabledPluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<AuditingDisabledPluginTest>();
        _plugin = new AuditingDisabledPlugin(_logger);
        _mockSession = new Mock<ISession>();
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection([new(UserTokenType.Certificate)]),
            EndpointUrl = "opc.tcp://localhost:4840",
        };
        Endpoint endpoint = new(endpointDescription);

        // session should return true to session.ReadValue(Util.WellKnownNodes.Server_Auditing)
        _mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_Auditing)).Returns(new DataValue(new Variant(true)));
        _mockSession.Setup(session => session.Endpoint).Returns(endpointDescription);

        // act
        Issue? issue = _plugin.Run([_mockSession.Object]);

        // assert
        Assert.True(issue == null);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection([new(UserTokenType.Anonymous)]),
            EndpointUrl = "opc.tcp://localhost:4840",
        };
        Endpoint endpoint = new(endpointDescription);

        // session should return false to session.ReadValue(Util.WellKnownNodes.Server_Auditing)
        _mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_Auditing)).Returns(new DataValue(new Variant(false)));
        _mockSession.Setup(session => session.Endpoint).Returns(endpointDescription);

        // act
        Issue? issue = _plugin.Run([_mockSession.Object]);

        // assert
        Assert.True(issue != null);
    }

}
