
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Xunit;

namespace Tests;
public class RBACNotSupportedPluginTest
{

    private readonly ILogger _logger;
    private readonly Mock<ISession> _mockSession;
    private readonly RBACNotSupportedPlugin _plugin;

    public RBACNotSupportedPluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<RBACNotSupportedPluginTest>();
        _mockSession = new Mock<ISession>();
        _plugin = new RBACNotSupportedPlugin(_logger);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Certificate) }),
            EndpointUrl = "opc.tcp://localhost:4840",
        };
        Endpoint endpoint = new(endpointDescription);

        // session should return any of the well-known RBAC profiles on session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray)
        _mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray)).Returns(new DataValue(new Variant(new string[] { Util.WellKnownProfiles.Security_User_Access_Control_Full })));
        _mockSession.Setup(session => session.Endpoint).Returns(endpointDescription);

        // act
        Issue? issue = _plugin.Run(_mockSession.Object);

        // assert
        Assert.True(issue == null);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Anonymous) }),
            EndpointUrl = "opc.tcp://localhost:4840",
        };
        Endpoint endpoint = new(endpointDescription);

        // session should return none of the well-known RBAC profiles on session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray)
        _mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray)).Returns(new DataValue());
        _mockSession.Setup(session => session.Endpoint).Returns(endpointDescription);

        // act
        Issue? issue = _plugin.Run(_mockSession.Object);

        // assert
        Assert.True(issue != null);
    }

}
