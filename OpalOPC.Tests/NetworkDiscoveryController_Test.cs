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

    private ApplicationDescription applicationDescription(Uri uri)
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
    private static ServerOnNetwork serverOnNetwork(Uri uri)
    {
        return new ServerOnNetwork()
        {
            DiscoveryUrl = uri.OriginalString
        };
    }

    [Fact]
    public void ReturnsCorrectNumberOfUri()
    {
        // arrange
        Uri appUri1 = new("opc.tcp://127.0.0.1:4840");
        Uri appUri2 = new("opc.tcp://127.0.0.1:53530");
        Uri dnsAppURI = new("opc.tcp://127.0.0.1:53531");
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(appUri1)).Returns([applicationDescription(appUri1)]);
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(appUri2)).Returns([applicationDescription(appUri2)]);
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsOnNetwork(appUri1)).Returns([serverOnNetwork(appUri2)]);
        _mockMDNSUtil.Setup(m => m.DiscoverTargets(It.IsAny<ConcurrentBag<Uri>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<ConcurrentBag<Uri>, string, string, CancellationToken>((bag, query, scheme, token) =>
        {
            bag.Add(dnsAppURI);
        });

        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = controller.MulticastDiscoverTargets(_timeoutSeconds);

        // assert
        Assert.NotEmpty(targets);
        Assert.True(targets.Count == 3);
        Assert.Contains(appUri1, targets);
        Assert.Contains(appUri2, targets);
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplications(It.IsAny<Uri>()), Times.Exactly(4));
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsOnNetwork(It.IsAny<Uri>()), Times.Exactly(3));
        _mockMDNSUtil.Verify(m => m.DiscoverTargets(It.IsAny<ConcurrentBag<Uri>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public void ReturnsCorrectNumberOfUrisWhenFindServersOnNetworkReturnsEmptyList()
    {
        // arrange
        Uri appUri1 = new("opc.tcp://127.0.0.1:4840");
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns([applicationDescription(appUri1)]);
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsOnNetwork(It.IsAny<Uri>())).Returns([]);

        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = controller.MulticastDiscoverTargets(_timeoutSeconds);

        // assert
        Assert.NotEmpty(targets);
        Assert.True(targets.Count == 1);
        Assert.Contains(appUri1, targets);
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplications(It.IsAny<Uri>()), Times.Exactly(3));
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsOnNetwork(It.IsAny<Uri>()), Times.Exactly(3));
    }

    [Fact]
    public void ReturnsCorrectNumberOfUrisWhenFindServersOnNetworkReturnsSameServer()
    {
        // arrange
        Uri uri = new("opc.tcp://asdadsasd");
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns([applicationDescription(uri)]);
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsOnNetwork(It.IsAny<Uri>())).Returns([serverOnNetwork(uri)]);

        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = controller.MulticastDiscoverTargets(_timeoutSeconds);

        // assert
        Assert.NotEmpty(targets);
        Assert.True(targets.Count == 1);
        Assert.Contains(uri, targets);
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplications(It.IsAny<Uri>()), Times.Exactly(6)); // 3 times by default, 3 times more for each time theres a serveronnetwork found
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsOnNetwork(It.IsAny<Uri>()), Times.Exactly(3));
    }

    [Fact]
    public void DiscoverApplicationsAndDiscoverApplicationsOnNetworkExceptionResultsInEmptyTargets()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Throws<Exception>();
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplicationsOnNetwork(It.IsAny<Uri>())).Throws<Exception>();

        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = controller.MulticastDiscoverTargets(_timeoutSeconds);

        // act & assert
        Assert.Empty(targets);
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplications(It.IsAny<Uri>()), Times.Exactly(3));
        _mockDiscoveryUtil.Verify(util => util.DiscoverApplicationsOnNetwork(It.IsAny<Uri>()), Times.Exactly(3));
    }

    [Fact]
    public void ExceptionInMDNSDiscovererResultsInEmptyTargets()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns([]);
        _mockMDNSUtil.Setup(m => m.DiscoverTargets(It.IsAny<ConcurrentBag<Uri>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws<Exception>();

        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = controller.MulticastDiscoverTargets(_timeoutSeconds);

        // assert
        Assert.Empty(targets);
    }

    [Fact]
    public void DiscoverApplicationsReturnsEmptyResultsInEmptyTargets()
    {
        // arrange
        _mockDiscoveryUtil.Setup(util => util.DiscoverApplications(It.IsAny<Uri>())).Returns([]);
        //_mockMDNSUtil.Setup(m => m.DiscoverTargets(It.IsAny<ConcurrentBag<Uri>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns([]);


        NetworkDiscoveryController controller = new(_loggerMock.Object, _mockDiscoveryUtil.Object, _mockMDNSUtil.Object);

        // act
        IList<Uri> targets = controller.MulticastDiscoverTargets(_timeoutSeconds);

        // assert
        Assert.Empty(targets);
    }
}
