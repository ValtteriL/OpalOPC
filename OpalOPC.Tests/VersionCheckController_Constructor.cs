namespace Tests;
using Controller;
using Microsoft.Extensions.Logging;
using Model;
using View;
using Xunit;

public class VersionCheckController_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<VersionCheckController_Constructor>();
        VersionCheckController versionCheckController = new(logger);

        Assert.True(versionCheckController != null);
    }

}
