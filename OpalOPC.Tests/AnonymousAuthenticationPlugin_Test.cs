
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

namespace Tests;
public class AnonymousAuthenticationPluginTest
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<AuditingDisabledPluginTest>();

        // act
        AnonymousAuthenticationPlugin plugin = new(logger);

        // assert
        Assert.True(plugin != null);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<AuditingDisabledPluginTest>();

        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Certificate) })
        };
        Endpoint endpoint = new(endpointDescription);

        // StartSession should not be called at all
        var mockConnectionUtil = new Mock<IConnectionUtil>();

        AnonymousAuthenticationPlugin plugin = new(logger, mockConnectionUtil.Object);

        // act
        (Issue? issue, ICollection<ISession> sessions) = plugin.Run(endpoint);

        // assert
        mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Never());
        Assert.True(issue == null);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<AnonymousAuthenticationPluginTest>();
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Anonymous) })
        };
        Endpoint endpoint = new(endpointDescription);

        // StartSession should return a dummy session
        var mockConnectionUtil = new Mock<IConnectionUtil>();
        var mockSession = new Mock<ISession>();
        mockConnectionUtil.Setup(conn => conn.StartSession(endpointDescription, new UserIdentity()).Result).Returns(mockSession.Object);

        AnonymousAuthenticationPlugin plugin = new(logger, mockConnectionUtil.Object);

        // act
        (Issue? issue, ICollection<ISession> sessions) = plugin.Run(endpoint);

        // assert
        Assert.True(issue != null);
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == 1);
    }

}
