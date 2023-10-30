
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

namespace Tests;
public class RBACNotSupportedPluginTest
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<RBACNotSupportedPluginTest>();

        // act
        RBACNotSupportedPlugin plugin = new(logger);

        // assert
        Assert.True(plugin != null);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityModeInvalidPluginTest>();

        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Certificate) })
        };
        Endpoint endpoint = new(endpointDescription);

        RBACNotSupportedPlugin plugin = new(logger);

        // session should return any of the well-known RBAC profiles on session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray)
        var mockSession = new Mock<ISession>();
        mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray)).Returns(new DataValue(new Variant(new string[] { Util.WellKnownProfiles.Security_User_Access_Control_Full })));

        // act
        Issue? issue = plugin.Run(mockSession.Object);

        // assert
        Assert.True(issue == null);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityModeInvalidPluginTest>();
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Anonymous) })
        };
        Endpoint endpoint = new(endpointDescription);

        // session should return none of the well-known RBAC profiles on session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray)
        var mockSession = new Mock<ISession>();
        mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray)).Returns(new DataValue());

        RBACNotSupportedPlugin plugin = new(logger);

        // act
        Issue? issue = plugin.Run(mockSession.Object);

        // assert
        Assert.True(issue != null);
    }

}
