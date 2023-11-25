
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Xunit;

namespace Tests;
public class SecurityPolicyBasic128Rsa15PluginTest
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityPolicyBasic128Rsa15PluginTest>();

        // act
        SecurityPolicyBasic128Rsa15Plugin plugin = new(logger);

        // assert
        Assert.True(plugin != null);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityPolicyBasic128Rsa15PluginTest>();

        EndpointDescription endpointDescription = new()
        {
            SecurityPolicyUri = new Uri(SecurityPolicies.Basic256).ToString(),
        };
        Endpoint endpoint = new(endpointDescription);

        SecurityPolicyBasic128Rsa15Plugin plugin = new(logger);


        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(endpoint);

        // assert
        Assert.True(issue == null);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityPolicyBasic128Rsa15PluginTest>();
        EndpointDescription endpointDescription = new()
        {
            SecurityPolicyUri = new Uri(SecurityPolicies.Basic128Rsa15).ToString(),
        };
        Endpoint endpoint = new(endpointDescription);

        SecurityPolicyBasic128Rsa15Plugin plugin = new(logger);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = plugin.Run(endpoint);

        // assert
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

}
