
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Xunit;

namespace Tests;
public class SecurityPolicyBasic128Rsa15PluginTest
{
    private readonly ILogger _logger;
    private readonly SecurityPolicyBasic128Rsa15Plugin _plugin;
    private readonly string _discoveryUrl = "opc.tcp://localhost:4840";
    private readonly EndpointDescriptionCollection _endpointDescriptions = new();

    public SecurityPolicyBasic128Rsa15PluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<SecurityPolicyBasic128Rsa15PluginTest>();
        _plugin = new SecurityPolicyBasic128Rsa15Plugin(_logger);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange

        EndpointDescription endpointDescription = new()
        {
            SecurityPolicyUri = new Uri(SecurityPolicies.Basic256).ToString(),
        };
        _endpointDescriptions.Add(endpointDescription);


        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue == null);
        Assert.Empty(sessions);
    }

    [Fact]
    public void ReportsIssues()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            SecurityPolicyUri = new Uri(SecurityPolicies.Basic128Rsa15).ToString(),
        };
        _endpointDescriptions.Add(endpointDescription);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

}
