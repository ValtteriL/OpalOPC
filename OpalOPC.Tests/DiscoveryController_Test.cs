
using System.Net;
using System.Net.Sockets;
using Controller;
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Util;
using Xunit;

namespace Tests;
public class DiscoveryControllerTest
{
    private readonly ApplicationDescriptionCollection _validApplicationDescriptionCollection = new() { new ApplicationDescription()
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
    private readonly EndpointDescriptionCollection _validEndpointDescriptionCollection = new()
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
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
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
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
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
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
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
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
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
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
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
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
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
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(Array.Empty<IPAddress>());
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
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
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
    public void DiscoverApplicationsExceptionReturnsInExceptionThrown()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new IPAddress(new byte[] { 127, 0, 0, 1 }) });
        mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Throws<Exception>();
        mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(logger, mockDiscoveryUtil.Object);

        // act & assert
        Assert.Throws<Exception>(() => controller.DiscoverTargets(discoveryUris));
    }

    [Fact]
    public void DiscoverEndpointsServiceResultExceptionReturnsSingleTargetNoEndpoints()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
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
    public void DiscoverEndpointsExceptionResultsInExceptionThrown()
    {
        // arrange
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryControllerTest>();
        List<Uri> discoveryUris = new() {
            new Uri("opc.tcp://asd.com"),
        };

        var mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new IPAddress(new byte[] { 127, 0, 0, 1 }) });
        mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Throws<Exception>();
        mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);

        DiscoveryController controller = new(logger, mockDiscoveryUtil.Object);

        // act & assert
        Assert.Throws<Exception>(() => controller.DiscoverTargets(discoveryUris));
    }

}
