
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
    private readonly ApplicationDescriptionCollection _validApplicationDescriptionCollection = [new ApplicationDescription()
    {
        ApplicationName = "test",
        ApplicationType = ApplicationType.Server,
        ApplicationUri = "asd",
        ProductUri = "asd",
        DiscoveryUrls =
            [
                "opc.tcp://example.com:53530"
            ]
    }];
    private readonly EndpointDescriptionCollection _validEndpointDescriptionCollection =
    [
        new EndpointDescription()
        {
            EndpointUrl = "asd"
        }
    ];

    private readonly List<Uri> _discoveryUris =
    [
        new Uri("opc.tcp://asd.com"),
    ];
    private readonly Mock<IDiscoveryUtil> _mockDiscoveryUtil;
    private readonly Mock<ITaskUtil> _mockTaskUtil;
    private readonly Mock<ILogger<DiscoveryController>> _loggerMock;

    public DiscoveryControllerTest()
    {
        _loggerMock = new Mock<ILogger<DiscoveryController>>();
        _mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        _mockTaskUtil = new Mock<ITaskUtil>();
    }


    [Fact]
    public void ReturnsCorrectNumberOfTargetsAndEndpoints()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new(new byte[] { 127, 0, 0, 1 }) });
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);
        _mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockTaskUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(_discoveryUris);

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
        DiscoveryUtil discoveryUtil = new();
        DiscoveryController controller = new(_loggerMock.Object, discoveryUtil, _mockTaskUtil.Object);
        List<Uri> discoveryUris = [];

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void HttpsDiscoveryUriReturnsEmptyTargets()
    {
        // arrange
        DiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockTaskUtil.Object);
        List<Uri> discoveryUris = [
            new Uri("https://test.com")
        ];

        // act
        ICollection<Target> targets = controller.DiscoverTargets(discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void SocketExceptionReturnsEmptyTargets()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Throws<SocketException>();
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);
        _mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockTaskUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(_discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void ExceptionReturnsEmptyTargets()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Throws<Exception>();
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);
        _mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockTaskUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(_discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void ResolvesToZeroAddresses()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns([]);
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);
        _mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockTaskUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(_discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void DiscoverApplicationsServiceResultExceptionReturnsEmptyTargets()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new(new byte[] { 127, 0, 0, 1 }) });
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Throws<ServiceResultException>();
        _mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockTaskUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(_discoveryUris);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void DiscoverApplicationsExceptionReturnsInExceptionThrown()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new(new byte[] { 127, 0, 0, 1 }) });
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Throws<Exception>();
        _mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Returns(_validEndpointDescriptionCollection);

        DiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockTaskUtil.Object);

        // act & assert
        Assert.Throws<Exception>(() => controller.DiscoverTargets(_discoveryUris));
    }

    [Fact]
    public void DiscoverEndpointsServiceResultExceptionReturnsSingleTargetNoEndpoints()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new(new byte[] { 127, 0, 0, 1 }) });
        _mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Throws<ServiceResultException>();
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);

        DiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockTaskUtil.Object);

        // act
        ICollection<Target> targets = controller.DiscoverTargets(_discoveryUris);

        // assert
        Assert.True(targets.Count == 1);
        Assert.Empty(targets.First().Servers.First().EndpointDescriptions);
    }

    [Fact]
    public void DiscoverEndpointsExceptionResultsInExceptionThrown()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.ResolveIPv4Addresses(It.IsAny<string>())).Returns(new IPAddress[] { new(new byte[] { 127, 0, 0, 1 }) });
        _mockDiscoveryUtil.Setup(util => util.DiscoverEndpoints(It.IsAny<Uri>())).Throws<Exception>();
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns(_validApplicationDescriptionCollection);

        DiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockTaskUtil.Object);

        // act & assert
        Assert.Throws<Exception>(() => controller.DiscoverTargets(_discoveryUris));
    }

}
