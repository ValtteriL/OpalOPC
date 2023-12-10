
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Plugin;
using Xunit;

namespace Tests;
public class SecurityPolicyNonePluginTest
{
    private readonly ILogger _logger;
    private readonly SecurityPolicyNonePlugin _plugin;
    private readonly string _discoveryUrl = "opc.tcp://localhost:4840";
    private readonly EndpointDescriptionCollection _endpointDescriptions = new();

    public SecurityPolicyNonePluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<SecurityPolicyNonePluginTest>();
        _plugin = new SecurityPolicyNonePlugin(_logger);
    }


    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        EndpointDescription endpointDescription = new()
        {
            SecurityPolicyUri = new Uri(SecurityPolicies.Aes128_Sha256_RsaOaep).ToString(),
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
            SecurityPolicyUri = new Uri(SecurityPolicies.None).ToString(),
        };
        _endpointDescriptions.Add(endpointDescription);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

}
