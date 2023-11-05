
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Xunit;

namespace Tests;
public class SecurityPolicyNonePluginTest
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<AuditingDisabledPluginTest>();

        // act
        SecurityPolicyNonePlugin plugin = new(logger);

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
            SecurityPolicyUri = new Uri(SecurityPolicies.Aes128_Sha256_RsaOaep).ToString(),
        };
        Endpoint endpoint = new(endpointDescription);

        SecurityPolicyNonePlugin plugin = new(logger);


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
        ILogger logger = loggerFactory.CreateLogger<AuditingDisabledPluginTest>();
        EndpointDescription endpointDescription = new()
        {
            SecurityPolicyUri = new Uri(SecurityPolicies.None).ToString(),
        };
        Endpoint endpoint = new(endpointDescription);

        SecurityPolicyNonePlugin plugin = new(logger);

        // act
        (Issue? issue, ICollection<ISession> sessions) = plugin.Run(endpoint);

        // assert
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

}
