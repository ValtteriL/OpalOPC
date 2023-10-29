namespace Tests;
using View;
using Model;
using Xunit;
using Microsoft.Extensions.Logging;
using Controller;

public class ReportController_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        var loggerFactory = LoggerFactory.Create(builder => {});
        ILogger logger = loggerFactory.CreateLogger<DiscoveryController_Constructor>();
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);
        ReportController reportController = new(logger, reporter);

        Assert.True(reportController != null);
    }

}