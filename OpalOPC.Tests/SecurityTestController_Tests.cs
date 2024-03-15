using Controller;
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Util;
using Xunit;

namespace Tests
{
    public class SecurityTestControllerTest
    {
        private readonly Mock<ILogger<ISecurityTestController>> _loggerMock;
        private readonly Mock<ITaskUtil> _taskUtilMock;
        private readonly EndpointDescription _endpointDescription = new()
        {
            UserIdentityTokens = new UserTokenPolicyCollection([new(UserTokenType.UserName)]),
            SecurityMode = MessageSecurityMode.None,
            EndpointUrl = "opc.tcp://localhost:4840",
        };
        private readonly Server _server;
        private readonly ApplicationDescription _applicationDescription = new()
        {
            ApplicationType = ApplicationType.Server,
            ApplicationName = "test",
            ApplicationUri = "test",
            ProductUri = "test",
        };
        private readonly Target _target;
        private readonly Mock<ISecurityTestSession> _mockSecurityTestSession;
        private readonly Mock<IPreAuthPlugin> _mockPreAuthPlugin;
        private readonly Mock<IPostAuthPlugin> _mockPostAuthPlugin;
        private readonly AuthenticationData _authenticationData = new();
        private readonly Mock<IPluginRepository> _pluginRepositoryMock;


        public SecurityTestControllerTest()
        {
            _loggerMock = new Mock<ILogger<ISecurityTestController>>();
            _server = new("opc.tcp://discoveryuri", [_endpointDescription]);
            _target = new(_applicationDescription);
            _target.AddServer(_server);
            _mockSecurityTestSession = new Mock<ISecurityTestSession>();
            _mockPreAuthPlugin = new Mock<IPreAuthPlugin>();
            _mockPostAuthPlugin = new Mock<IPostAuthPlugin>();
            _taskUtilMock = new Mock<ITaskUtil>();
            _pluginRepositoryMock = new Mock<IPluginRepository>();
        }


        [Fact]
        public void TestTargetsRemainUntouchedIfNoPlugins()
        {
            // arrange
            var opcTargets = new List<Target> { _target };
            _pluginRepositoryMock.Setup(repository => repository.BuildAll(It.IsAny<AuthenticationData>())).Returns([]);
            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object, _pluginRepositoryMock.Object);

            // act
            ICollection<Target> newTargets = securityTestController.TestTargetSecurity(opcTargets, _authenticationData);

            // assert
            Assert.Equal(opcTargets, newTargets);
        }

        [Fact]
        public void TestTargetsRemainUntouchedIfOnlyPostAuthPlugins()
        {
            // arrange
            var opcTargets = new List<Target> { _target };

            var mockPostAuthPlugin = new Mock<IPostAuthPlugin>();
            mockPostAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PostAuthPlugin);

            _pluginRepositoryMock.Setup(repository => repository.BuildAll(It.IsAny<AuthenticationData>())).Returns([mockPostAuthPlugin.Object]);

            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object, _pluginRepositoryMock.Object);

            // act
            securityTestController.TestTargetSecurity(opcTargets, _authenticationData);

            // assert
            mockPostAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<IList<ISession>>()), Times.Never());
            Assert.Equal(opcTargets, opcTargets);
        }

        [Fact]
        public void TestEndpointSecurityCallsPluginForEachEndpoint()
        {
            // arrange
            var opcTargets = new List<Target> { _target, _target, _target };

            var mockPreAuthPlugin = new Mock<IPreAuthPlugin>();
            mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>())).Returns((new Issue(PluginId.BruteForce, "description", 2), new List<ISecurityTestSession>()));
            mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);

            _pluginRepositoryMock.Setup(repository => repository.BuildAll(It.IsAny<AuthenticationData>())).Returns([mockPreAuthPlugin.Object]);

            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object, _pluginRepositoryMock.Object);

            // act
            securityTestController.TestTargetSecurity(opcTargets, _authenticationData);

            // assert
            mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>()), Times.Exactly(opcTargets.Count));
        }

        [Fact]
        public void TestNoPluginsRunIfNoTargets()
        {
            // arrange
            _mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>())).Returns((new Issue(PluginId.BruteForce, "description", 2), new List<ISecurityTestSession>()));
            _mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);
            _pluginRepositoryMock.Setup(repository => repository.BuildAll(It.IsAny<AuthenticationData>())).Returns([_mockPreAuthPlugin.Object]);
            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object, _pluginRepositoryMock.Object);

            // act
            securityTestController.TestTargetSecurity([], _authenticationData);

            // assert
            _mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>()), Times.Never);
        }

        [Fact]
        public void TestIssuesAreAddedToTarget()
        {
            // arrange
            var opcTargets = new List<Target> { _target };

            _mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>())).Returns((new Issue(PluginId.BruteForce, "description", 2), new List<ISecurityTestSession>() { _mockSecurityTestSession.Object }));
            _mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);

            _mockPostAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<IList<ISession>>())).Returns((new Issue(PluginId.KnownVulnerability, "test", 2)));
            _mockPostAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PostAuthPlugin);

            _pluginRepositoryMock.Setup(repository => repository.BuildAll(It.IsAny<AuthenticationData>())).Returns([_mockPreAuthPlugin.Object, _mockPostAuthPlugin.Object]);

            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object, _pluginRepositoryMock.Object);

            // act
            securityTestController.TestTargetSecurity(opcTargets, _authenticationData);

            // assert
            _mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>()), Times.Once);
            _mockPostAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<IList<ISession>>()), Times.Once);
            Assert.True(opcTargets.First().Servers.First().Issues.Count == 2);
        }

        [Fact]
        public void TestPostAuthNotRunIfNoSessions()
        {
            // arrange
            var opcTargets = new List<Target> { _target };

            _mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>())).Returns((new Issue(PluginId.BruteForce, "description", 2), new List<ISecurityTestSession>()));
            _mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);

            _mockPostAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<IList<ISession>>())).Returns((new Issue(PluginId.AuditingDisabled, "test", 2)));
            _mockPostAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PostAuthPlugin);

            _pluginRepositoryMock.Setup(repository => repository.BuildAll(It.IsAny<AuthenticationData>())).Returns([_mockPreAuthPlugin.Object, _mockPostAuthPlugin.Object]);

            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object, _pluginRepositoryMock.Object);

            // act
            securityTestController.TestTargetSecurity(opcTargets, _authenticationData);

            // assert
            _mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>()), Times.Once);
            _mockPostAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<IList<ISession>>()), Times.Never);
            Assert.True(opcTargets.First().Servers.First().Issues.Count == 1);
        }
    }
}
