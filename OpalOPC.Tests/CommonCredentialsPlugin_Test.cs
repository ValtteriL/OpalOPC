namespace Tests;

using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

public class CommonCredentialsPluginTest
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<CommonCredentialsPluginTest>();

        // act
        CommonCredentialsPlugin plugin = new(logger);

        // assert
        Assert.True(plugin != null);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityModeInvalidPluginTest>();

        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
            SecurityMode = MessageSecurityMode.None
        };
        Endpoint endpoint = new(endpointDescription);

        // StartSession returns only closed sessions
        var mockConnectionUtil = new Mock<IConnectionUtil>();
        var mockSession = new Mock<ISession>();
        mockSession.Setup(session => session.Connected).Returns(false);
        mockConnectionUtil.Setup(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()).Result).Returns(mockSession.Object);

        CommonCredentialsPlugin plugin = new(logger, mockConnectionUtil.Object);

        // act
        (Issue? issue, ICollection<ISession> sessions) = plugin.Run(endpoint);

        // assert
        mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Exactly(Util.Credentials.CommonCredentials.Count));
        Assert.True(issue == null);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityModeInvalidPluginTest>();
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
            SecurityMode = MessageSecurityMode.None
        };
        Endpoint endpoint = new(endpointDescription);

        // StartSession should return a dummy session
        // StartSession returns single open session, then closed sessions
        var mockConnectionUtil = new Mock<IConnectionUtil>();
        var mockSessionSuccess = new Mock<ISession>();
        mockSessionSuccess.Setup(session => session.Connected).Returns(true);
        var mockSession = new Mock<ISession>();
        mockSession.Setup(session => session.Connected).Returns(false);
        mockConnectionUtil.SetupSequence(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()).Result).Returns(mockSessionSuccess.Object).Returns(mockSession.Object);

        CommonCredentialsPlugin plugin = new(logger, mockConnectionUtil.Object);

        // act
        (Issue? issue, ICollection<ISession> sessions) = plugin.Run(endpoint);

        // assert
        mockConnectionUtil.Verify(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()), Times.Exactly(Util.Credentials.CommonCredentials.Count));
        Assert.True(issue != null);
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Count == 1);
    }

}