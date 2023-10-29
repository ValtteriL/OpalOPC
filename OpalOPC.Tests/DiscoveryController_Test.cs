namespace Tests;

using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Client;
using Controller;
using Util;
using Xunit;
using System.Net.Sockets;
using System.Net;

public class DiscoveryControllerTest
{
    private ApplicationDescriptionCollection _validApplicationDescriptionCollection = new() { new ApplicationDescription()
    {
        ApplicationName = "test",
        ApplicationType = ApplicationType.Server,
        ApplicationUri = "asd",
        ProductUri = "asd",
        DiscoveryUrls = new StringCollection()
            {
                "opc.tcp://example.com:53530"
            }
    } };
    private EndpointDescriptionCollection _validEndpointDescriptionCollection = new()
    {
        new EndpointDescription()
        {
            EndpointUrl = "asd"
        }
    };

    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        DiscoveryUtil discoveryUtil = new();

        // act
        DiscoveryController controller = new(logger, discoveryUtil);

        // assert
        Assert.True(controller != null);
    }

    [Fact]
    public void ReturnsCorrectNumberOfTargetsAndEndpoints()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new IPAddress(new byte[] { 127, 0, 0, 1 }) });
        mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);
        mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(logger, mockDiscoveryUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.NotEmpty(targets);
        Assert.True(targets.Count == 1);
        Assert.True(targets.First().ApplicationName == _validApplicationDescriptionCollection.First().ApplicationName);
        Assert.True(targets.First().ApplicationUri == _validApplicationDescriptionCollection.First().ApplicationUri);
        Assert.True(targets.First().ProductUri == _validApplicationDescriptionCollection.First().ProductUri);
    }

    [Fact]
    public void EmptyDiscoveryUrisReturnEmptyTargets()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        DiscoveryUtil discoveryUtil = new();
        DiscoveryController controller = new(logger, discoveryUtil);
        List<Uri> discoveryUris = new();

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void HttpsDiscoveryUriReturnsEmptyTargets()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        DiscoveryUtil discoveryUtil = new();
        DiscoveryController controller = new(logger, discoveryUtil);
        List<Uri> discoveryUris = new() {
            new Uri("https://test.com")
        };

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void SocketExceptionReturnsEmptyTargets()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Throws<SocketException>();
        mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);
        mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(logger, mockDiscoveryUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void ExceptionReturnsEmptyTargets()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Throws<Exception>();
        mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);
        mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(logger, mockDiscoveryUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void ResolvesToZeroAddresses()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { });
        mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);
        mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(logger, mockDiscoveryUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void DiscoverApplicationsServiceResultExceptionReturnsEmptyTargets()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new IPAddress(new byte[] { 127, 0, 0, 1 }) });
        mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Throws<ServiceResultException>();
        mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(logger, mockDiscoveryUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void DiscoverApplicationsExceptionReturnsEmptyTargets()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new IPAddress(new byte[] { 127, 0, 0, 1 }) });
        mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Throws<Exception>();
        mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(logger, mockDiscoveryUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void DiscoverEndpointsServiceResultExceptionReturnsSingleTargetNoEndpoints()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new IPAddress(new byte[] { 127, 0, 0, 1 }) });
        mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Throws<ServiceResultException>();
        mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);

        DiscoveryController controller = new(logger, mockDiscoveryUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.True(targets.Count == 1);
        Assert.Empty(targets.First().Servers.First().Endpoints);
    }

    [Fact]
    public void DiscoverEndpointsExceptionReturnsEmptyTargets()
    {
        // arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new IPAddress(new byte[] { 127, 0, 0, 1 }) });
        mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Throws<Exception>();
        mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);

        DiscoveryController controller = new(logger, mockDiscoveryUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.Empty(targets);
    }

}
