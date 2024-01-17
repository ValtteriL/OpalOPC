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
            UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
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
        }


        [Fact]
        public void TestTargetsRemainUntouchedIfNoPlugins()
        {
            // arrange
            var opcTargets = new List<Target> { _target };
            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object);

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

            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object);

            // act
            securityTestController.TestTargetSecurity(opcTargets, _authenticationData);

            // assert
            mockPostAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<ISession>()), Times.Never());
            Assert.Equal(opcTargets, opcTargets);
        }

        [Fact]
        public void TestEndpointSecurityCallsPluginForEachEndpoint()
        {
            // arrange
            var opcTargets = new List<Target> { _target, _target, _target };

            var mockPreAuthPlugin = new Mock<IPreAuthPlugin>();
            mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>())).Returns((new Issue(1, "test", 2), new List<ISecurityTestSession>()));
            mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);
            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object);

            // act
            securityTestController.TestTargetSecurity(opcTargets, _authenticationData);

            // assert
            mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>()), Times.Exactly(opcTargets.Count));
        }

        [Fact]
        public void TestNoPluginsRunIfNoTargets()
        {
            // arrange
            _mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>())).Returns((new Issue(1, "test", 2), new List<ISecurityTestSession>()));
            _mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);
            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object);

            // act
            securityTestController.TestTargetSecurity(new List<Target>(), _authenticationData);

            // assert
            _mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>()), Times.Never);
        }

        [Fact]
        public void TestIssuesAreAddedToTarget()
        {
            // arrange
            var opcTargets = new List<Target> { _target };

            _mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>())).Returns((new Issue(1, "test", 2), new List<ISecurityTestSession>() { _mockSecurityTestSession.Object }));
            _mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);

            _mockPostAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<ISession>())).Returns((new Issue(2, "test", 2)));
            _mockPostAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PostAuthPlugin);

            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object);

            // act
            securityTestController.TestTargetSecurity(opcTargets, _authenticationData);

            // assert
            _mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>()), Times.Once);
            _mockPostAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<ISession>()), Times.Once);
            Assert.True(opcTargets.First().Servers.First().Issues.Count == 2);
        }

        [Fact]
        public void TestPostAuthNotRunIfNoSessions()
        {
            // arrange
            var opcTargets = new List<Target> { _target };

            _mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>())).Returns((new Issue(1, "test", 2), new List<ISecurityTestSession>()));
            _mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);

            _mockPostAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<ISession>())).Returns((new Issue(2, "test", 2)));
            _mockPostAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PostAuthPlugin);

            SecurityTestController securityTestController = new(_loggerMock.Object, _taskUtilMock.Object);

            // act
            securityTestController.TestTargetSecurity(opcTargets, _authenticationData);

            // assert
            _mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<string>(), It.IsAny<EndpointDescriptionCollection>()), Times.Once);
            _mockPostAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<ISession>()), Times.Never);
            Assert.True(opcTargets.First().Servers.First().Issues.Count == 1);
        }
    }
}
