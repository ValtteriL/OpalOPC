namespace Tests;

using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

public class AuditingDisabledPluginTest
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryController_Constructor>();

        // act
        AuditingDisabledPlugin plugin = new(logger);

        // assert
        Assert.True(plugin != null);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryController_Constructor>();

        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Certificate) })
        };
        Endpoint endpoint = new(endpointDescription);

        AuditingDisabledPlugin plugin = new(logger);

        // session should return true to session.ReadValue(Util.WellKnownNodes.Server_Auditing)
        var mockSession = new Mock<ISession>();
        mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_Auditing)).Returns(new DataValue(new Variant(true)));

        // act
        Issue? issue = plugin.Run(mockSession.Object);

        // assert
        Assert.True(issue == null);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryController_Constructor>();
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Anonymous) })
        };
        Endpoint endpoint = new(endpointDescription);

        // session should return false to session.ReadValue(Util.WellKnownNodes.Server_Auditing)
        var mockSession = new Mock<ISession>();
        mockSession.Setup(session => session.ReadValue(Util.WellKnownNodes.Server_Auditing)).Returns(new DataValue(new Variant(false)));

        AuditingDisabledPlugin plugin = new(logger);

        // act
        Issue? issue = plugin.Run(mockSession.Object);

        // assert
        Assert.True(issue != null);
    }

}