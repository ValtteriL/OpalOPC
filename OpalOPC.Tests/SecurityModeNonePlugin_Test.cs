
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Xunit;

namespace Tests;
public class SecurityModeNonePluginTest
{

    private readonly ILogger _logger;
    private readonly SecurityModeNonePlugin _plugin;
    private readonly string _discoveryUrl = "opc.tcp://localhost:4840";
    private readonly EndpointDescriptionCollection _endpointDescriptions = new();
    public SecurityModeNonePluginTest()
    {
        _logger = LoggerFactory.Create(builder => { }).CreateLogger<SecurityModeNonePluginTest>();
        _plugin = new SecurityModeNonePlugin(_logger);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange

        EndpointDescription endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Certificate) }),
            SecurityMode = MessageSecurityMode.SignAndEncrypt
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
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.Anonymous) }),
            SecurityMode = MessageSecurityMode.None
        };
        _endpointDescriptions.Add(endpointDescription);

        // act
        (Issue? issue, ICollection<ISecurityTestSession> sessions) = _plugin.Run(_discoveryUrl, _endpointDescriptions);

        // assert
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

}
