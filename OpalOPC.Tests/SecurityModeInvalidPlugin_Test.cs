
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

namespace Tests;
public class SecurityModeInvalidPluginTest
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityModeInvalidPluginTest>();

        // act
        SecurityModeInvalidPlugin plugin = new(logger);

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
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Certificate) }),
            SecurityMode = MessageSecurityMode.None
        };
        Endpoint endpoint = new(endpointDescription);

        SecurityModeInvalidPlugin plugin = new(logger);


        // act
        (Issue? issue, ICollection<ISession> sessions) = plugin.Run(endpoint);

        // assert
        Assert.True(issue == null);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityModeInvalidPluginTest>();
        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Anonymous) }),
            SecurityMode = MessageSecurityMode.Invalid
        };
        Endpoint endpoint = new(endpointDescription);

        SecurityModeInvalidPlugin plugin = new(logger);

        // act
        (Issue? issue, ICollection<ISession> sessions) = plugin.Run(endpoint);

        // assert
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

}
