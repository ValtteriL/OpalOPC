using Controller;
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Plugin;
using Xunit;

namespace Tests
{
    public class SecurityTestControllerTest
    {
        private readonly Mock<ILogger> _loggerMock;
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


        public SecurityTestControllerTest()
        {
            _loggerMock = new Mock<ILogger>();
            _server = new("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription });
        }


        [Fact]
        public void TestTargetsRemainUntouchedIfNoPlugins()
        {
            // arrange
            Target target = new(_applicationDescription);
            target.AddServer(_server);

            var opcTargets = new List<Target> { target };
            SecurityTestController securityTestController = new(_loggerMock.Object, new List<IPlugin>());

            // act
            ICollection<Target> newTargets = securityTestController.TestTargetSecurity(opcTargets);

            // assert
            Assert.Equal(opcTargets, newTargets);
        }

        [Fact]
        public void TestTargetsRemainUntouchedIfOnlyPostAuthPlugins()
        {
            // arrange
            Target target = new(_applicationDescription);
            target.AddServer(_server);

            var opcTargets = new List<Target> { target };

            var mockPostAuthPlugin = new Mock<IPostAuthPlugin>();
            mockPostAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PostAuthPlugin);

            SecurityTestController securityTestController = new(_loggerMock.Object, new List<IPlugin> { mockPostAuthPlugin.Object });

            // act
            securityTestController.TestTargetSecurity(opcTargets);

            // assert
            mockPostAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<ISession>()), Times.Never());
            Assert.Equal(opcTargets, opcTargets);
        }

        [Fact]
        public void TestEndpointSecurityCallsPluginForEachEndpoint()
        {
            // arrange
            Target target = new(_applicationDescription);
            target.AddServer(_server);

            var opcTargets = new List<Target> { target, target, target };

            var mockPreAuthPlugin = new Mock<IPreAuthPlugin>();
            mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<Endpoint>())).Returns((new Issue(1, "test", 2), new List<ISession>()));
            mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);
            SecurityTestController securityTestController = new(_loggerMock.Object, new List<IPlugin> { mockPreAuthPlugin.Object });

            // act
            securityTestController.TestTargetSecurity(opcTargets);

            // assert
            mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<Endpoint>()), Times.Exactly(opcTargets.Count));
        }

        [Fact]
        public void TestNoPluginsRunIfNoTargets()
        {
            // arrange
            var mockPreAuthPlugin = new Mock<IPreAuthPlugin>();
            mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<Endpoint>())).Returns((new Issue(1, "test", 2), new List<ISession>()));
            mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);
            SecurityTestController securityTestController = new(_loggerMock.Object, new List<IPlugin> { mockPreAuthPlugin.Object });

            // act
            securityTestController.TestTargetSecurity(new List<Target>());

            // assert
            mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<Endpoint>()), Times.Never);
        }

        [Fact]
        public void TestIssuesAreAddedToTarget()
        {
            // arrange
            Target target = new(_applicationDescription);
            target.AddServer(_server);

            var opcTargets = new List<Target> { target };

            var sessionMock = new Mock<ISession>();

            var mockPreAuthPlugin = new Mock<IPreAuthPlugin>();
            mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<Endpoint>())).Returns((new Issue(1, "test", 2), new List<ISession>() { sessionMock.Object }));
            mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);

            var mockPostAuthPlugin = new Mock<IPostAuthPlugin>();
            mockPostAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<ISession>())).Returns((new Issue(2, "test", 2)));
            mockPostAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PostAuthPlugin);

            SecurityTestController securityTestController = new(_loggerMock.Object, new List<IPlugin> { mockPreAuthPlugin.Object, mockPostAuthPlugin.Object });

            // act
            securityTestController.TestTargetSecurity(opcTargets);

            // assert
            mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<Endpoint>()), Times.Once);
            mockPostAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<ISession>()), Times.Once);
            Assert.True(opcTargets.First().Servers.First().SeparatedEndpoints.First().Issues.Count == 2);
        }

        [Fact]
        public void TestPostAuthNotRunIfNoSessions()
        {
            // arrange
            Target target = new(_applicationDescription);
            target.AddServer(_server);

            var opcTargets = new List<Target> { target };

            var sessionMock = new Mock<ISession>();

            var mockPreAuthPlugin = new Mock<IPreAuthPlugin>();
            mockPreAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<Endpoint>())).Returns((new Issue(1, "test", 2), new List<ISession>()));
            mockPreAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PreAuthPlugin);

            var mockPostAuthPlugin = new Mock<IPostAuthPlugin>();
            mockPostAuthPlugin.Setup(plugin => plugin.Run(It.IsAny<ISession>())).Returns((new Issue(2, "test", 2)));
            mockPostAuthPlugin.Setup(plugin => plugin.Type).Returns(Plugintype.PostAuthPlugin);

            SecurityTestController securityTestController = new(_loggerMock.Object, new List<IPlugin> { mockPreAuthPlugin.Object, mockPostAuthPlugin.Object });

            // act
            securityTestController.TestTargetSecurity(opcTargets);

            // assert
            mockPreAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<Endpoint>()), Times.Once);
            mockPostAuthPlugin.Verify(plugin => plugin.Run(It.IsAny<ISession>()), Times.Never);
            Assert.True(opcTargets.First().Servers.First().SeparatedEndpoints.First().Issues.Count == 1);
        }
    }
}
