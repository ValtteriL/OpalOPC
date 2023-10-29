namespace Tests;
using View;
using Model;
using Xunit;
using Microsoft.Extensions.Logging;
using Controller;

public class VersionCheckController_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        var loggerFactory = LoggerFactory.Create(builder => {});
        ILogger logger = loggerFactory.CreateLogger<VersionCheckController_Constructor>();
        VersionCheckController versionCheckController = new(logger);

        Assert.True(versionCheckController != null);
    }

}