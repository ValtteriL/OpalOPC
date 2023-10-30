
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

namespace Tests;
public class SecurityPolicyBasic256PluginTest
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityPolicyBasic256PluginTest>();

        // act
        SecurityPolicyBasic256Plugin plugin = new(logger);

        // assert
        Assert.True(plugin != null);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<SecurityPolicyBasic256PluginTest>();

        EndpointDescription endpointDescription = new()
        {
            SecurityPolicyUri = new Uri(SecurityPolicies.Aes128_Sha256_RsaOaep).ToString(),
        };
        Endpoint endpoint = new(endpointDescription);

        SecurityPolicyBasic256Plugin plugin = new(logger);


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
        ILogger logger = loggerFactory.CreateLogger<SecurityPolicyBasic256PluginTest>();
        EndpointDescription endpointDescription = new()
        {
            SecurityPolicyUri = new Uri(SecurityPolicies.Basic256).ToString(),
        };
        Endpoint endpoint = new(endpointDescription);

        SecurityPolicyBasic256Plugin plugin = new(logger);

        // act
        (Issue? issue, ICollection<ISession> sessions) = plugin.Run(endpoint);

        // assert
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

}
