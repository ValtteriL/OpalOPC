namespace Tests;

using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

public class SelfSignedCertificatePluginTest
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<AuditingDisabledPluginTest>();

        // act
        SelfSignedCertificatePlugin plugin = new(logger);

        // assert
        Assert.True(plugin != null);
    }

    [Fact]
    public void DoesNotReportFalsePositive()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<AuditingDisabledPluginTest>();

        EndpointDescription endpointDescription = new()
        {
            SecurityPolicyUri = new Uri(SecurityPolicies.Aes128_Sha256_RsaOaep).ToString(),
        };
        Endpoint endpoint = new(endpointDescription);

        var mockConnectionUtil = new Mock<IConnectionUtil>();
        var mockSession = new Mock<ISession>();
        mockConnectionUtil.Setup(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()).Result).Throws<ServiceResultException>();

        SelfSignedCertificatePlugin plugin = new(logger, mockConnectionUtil.Object);


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
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<AuditingDisabledPluginTest>();
        EndpointDescription endpointDescription = new()
        {
            SecurityPolicyUri = new Uri(SecurityPolicies.None).ToString(),
        };
        Endpoint endpoint = new(endpointDescription);

        var mockConnectionUtil = new Mock<IConnectionUtil>();
        var mockSession = new Mock<ISession>();
        mockConnectionUtil.Setup(conn => conn.StartSession(It.IsAny<EndpointDescription>(), It.IsAny<UserIdentity>()).Result).Returns(mockSession.Object);

        SelfSignedCertificatePlugin plugin = new(logger, mockConnectionUtil.Object);

        // act
        (Issue? issue, ICollection<ISession> sessions) = plugin.Run(endpoint);

        // assert
        Assert.True(issue != null);
        Assert.Empty(sessions);
    }

}