namespace Tests;
using View;
using Model;
using Xunit;
using Microsoft.Extensions.Logging;
using Controller;

public class DiscoveryController_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        var loggerFactory = LoggerFactory.Create(builder => {});
        ILogger logger = loggerFactory.CreateLogger<DiscoveryController_Constructor>();
        DiscoveryController discoveryController = new DiscoveryController(logger);

        Assert.True(discoveryController != null);
    }

}