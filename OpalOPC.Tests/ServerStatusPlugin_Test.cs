
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

        // session should return null on session.ReadValue(Util.WellKnownNodes.Server_ServerStatus)
        _mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_ServerStatus, typeof(ServerStatusDataType))).Returns(null);
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

        // session should return plain ServerStatusDataType on session.ReadValue(Util.WellKnownNodes.Server_ServerStatus)
        _mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_ServerStatus, typeof(ServerStatusDataType))).Returns(
            new ServerStatusDataType() {
                SecondsTillShutdown = 0,
                State = ServerState.Running,
                BuildInfo = new BuildInfo()
                {
                    BuildDate = DateTime.Now,
                    BuildNumber = "1",
                    ManufacturerName = "Test",
                    ProductName = "Test",
                    ProductUri = "Test",
                    SoftwareVersion = "1.0.0"
                },
                CurrentTime = DateTime.Now,
                StartTime = DateTime.Now,
                ShutdownReason = new LocalizedText("Test")
            });
        _mockSession.Setup(session => session.Endpoint).Returns(endpointDescription);

        // act
        Issue? issue = _plugin.Run(_mockSession.Object);

        // assert
        Assert.True(issue != null);
    }

    [Fact]
    public void ReportsIssuesEvenIfShutdownReadonIsNull()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Anonymous) }),
            EndpointUrl = "opc.tcp://localhost:4840",
        };
        Endpoint endpoint = new(endpointDescription);

        // session should return ServerStatusDataType with shutdownreason null on session.ReadValue(Util.WellKnownNodes.Server_ServerStatus)
        _mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_ServerStatus, typeof(ServerStatusDataType))).Returns(
            new ServerStatusDataType());
        _mockSession.Setup(session => session.Endpoint).Returns(endpointDescription);

        // act
        Issue? issue = _plugin.Run(_mockSession.Object);

        // assert
        Assert.True(issue != null);
    }

}
