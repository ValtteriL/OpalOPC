
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Xunit;

namespace Tests;
public class ServerStatusPluginTest
{

    private readonly ILogger _logger;
    private readonly Mock<ISession> _mockSession;
    private readonly ServerStatusPlugin _plugin;

    public ServerStatusPluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<ServerStatusPluginTest>();
        _mockSession = new Mock<ISession>();
        _plugin = new ServerStatusPlugin(_logger);
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

        // session should return XXXXXXXX on session.ReadValue(Util.WellKnownNodes.Server_ServerStatus)
        _mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_ServerStatus)).Returns(new DataValue(new Variant(new string[] { Util.WellKnownProfiles.Security_User_Access_Control_Full })));
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

        // session should return YYYYYYYY on session.ReadValue(Util.WellKnownNodes.Server_ServerStatus)
        _mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_ServerStatus)).Returns(new DataValue());
        _mockSession.Setup(session => session.Endpoint).Returns(endpointDescription);

        // act
        Issue? issue = _plugin.Run(_mockSession.Object);

        // assert
        Assert.True(issue != null);
    }

}
