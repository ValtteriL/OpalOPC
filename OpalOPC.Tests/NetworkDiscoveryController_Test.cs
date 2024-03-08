using System.Collections.Concurrent;
using Controller;
using Microsoft.Extensions.Logging;
using Moq;
using Opc.Ua;
using Util;
using Xunit;

namespace Tests;
public class NetworkDiscoveryControllerTest
{
    private readonly Mock<IDiscoveryUtil> _mockDiscoveryUtil;
    private readonly Mock<IMDNSUtil> _mockMDNSUtil;
    private readonly Mock<ILogger<NetworkDiscoveryController>> _loggerMock;
    private readonly int _timeoutSeconds = 5;

    public NetworkDiscoveryControllerTest()
    {
        _loggerMock = new Mock<ILogger<NetworkDiscoveryController>>();
        _mockDiscoveryUtil = new Mock<IDiscoveryUtil>();
        _mockMDNSUtil = new Mock<IMDNSUtil>();
    }

    private static ApplicationDescription applicationDescription(Uri uri)
    {
        return new ApplicationDescription()
        {
            ApplicationName = "test",
            ApplicationType = ApplicationType.Server,
            ApplicationUri = "asd",
            ProductUri = "asd",
            DiscoveryUrls =
            [
                uri.OriginalString,
            ]
        };
    }
    private static FindServersOnNetworkResponse serversOnNetworkResponse(Uri uri)
    {
        return new FindServersOnNetworkResponse()
        {
            Servers =
            [
                new ServerOnNetwork()
                {
                    DiscoveryUrl = uri.OriginalString,
                }
            ]
        };
    }

    [Fact]
    public async Task ReturnsCorrectNumberOfUri()
    {
        // arrange
        Uri appUri1 = new("opc.tcp://127.0.0.1:4840");
        Uri appUri2 = new("opc.tcp://127.0.0.1:53530");
        Uri dnsAppURI = new("opc.tcp://127.0.0.1:53531");
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsAsync(appUri1)).ReturnsAsync([applicationDescription(appUri1)]);
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsAsync(appUri2)).ReturnsAsync([applicationDescription(appUri2)]);
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsOnNetworkAsync(appUri1, It.IsAny<CancellationToken>())).ReturnsAsync(serversOnNetworkResponse(appUri2));
        _mockMDNSUtil.Setup(m => m.DiscoverTargets(It.IsAny<ConcurrentBag<Uri>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<ConcurrentBag<Uri>, string, string, CancellationToken>((bag, query, scheme, token) =>
        {
            bag.Add(dnsAppURI);
        });

        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = await controller.MulticastDiscoverTargets(_timeoutSeconds);

        // assert
        Assert.NotEmpty(targets);
        Assert.True(targets.Count == 3);
        Assert.Contains(appUri1, targets);
        Assert.Contains(appUri2, targets);
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsAsync(It.IsAny<Uri>()), Times.Exactly(4));
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsOnNetworkAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _mockMDNSUtil.Verify(m => m.DiscoverTargets(It.IsAny<ConcurrentBag<Uri>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task ReturnsCorrectNumberOfUrisWhenFindServersOnNetworkReturnsEmptyList()
    {
        // arrange
        Uri appUri1 = new("opc.tcp://127.0.0.1:4840");
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsAsync(It.IsAny<Uri>())).ReturnsAsync([applicationDescription(appUri1)]);
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsOnNetworkAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FindServersOnNetworkResponse());

        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = await controller.MulticastDiscoverTargets(_timeoutSeconds);

        // assert
        Assert.NotEmpty(targets);
        Assert.True(targets.Count == 1);
        Assert.Contains(appUri1, targets);
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsAsync(It.IsAny<Uri>()), Times.Exactly(3));
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsOnNetworkAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task ReturnsCorrectNumberOfUrisWhenFindServersOnNetworkReturnsSameServer()
    {
        // arrange
        Uri uri = new("opc.tcp://asdadsasd");
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsAsync(It.IsAny<Uri>())).ReturnsAsync([applicationDescription(uri)]);
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsOnNetworkAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).ReturnsAsync(serversOnNetworkResponse(uri));

        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = await controller.MulticastDiscoverTargets(_timeoutSeconds);

        // assert
        Assert.NotEmpty(targets);
        Assert.True(targets.Count == 1);
        Assert.Contains(uri, targets);
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsAsync(It.IsAny<Uri>()), Times.Exactly(6)); // 3 times by default, 3 times more for each time theres a serveronnetwork found
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsOnNetworkAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task DiscoverApplicationsAsyncAndDiscoverApplicationsOnNetworkAsyncExceptionResultsInEmptyTargets()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsAsync(It.IsAny<Uri>())).Throws<Exception>();
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsOnNetworkAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Throws<Exception>();

        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = await controller.MulticastDiscoverTargets(_timeoutSeconds);

        // act & assert
        Assert.Empty(targets);
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsAsync(It.IsAny<Uri>()), Times.Exactly(3));
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsOnNetworkAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task ExceptionInMDNSDiscovererResultsInEmptyTargets()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsAsync(It.IsAny<Uri>())).ReturnsAsync([]);
        _mockMDNSUtil.Setup(m => m.DiscoverTargets(It.IsAny<ConcurrentBag<Uri>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws<Exception>();

        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = await controller.MulticastDiscoverTargets(_timeoutSeconds);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public async Task DiscoverApplicationsAsyncReturnsEmptyResultsInEmptyTargets()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsAsync(It.IsAny<Uri>())).ReturnsAsync([]);
        //_mockMDNSUtil.Setup(m => m.DiscoverTargets(It.IsAny<ConcurrentBag<Uri>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns([]);


        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = await controller.MulticastDiscoverTargets(_timeoutSeconds);

        // assert
        Assert.Empty(targets);
    }
}
