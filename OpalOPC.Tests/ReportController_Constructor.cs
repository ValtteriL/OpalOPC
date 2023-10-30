namespace Tests;
using Controller;
using Microsoft.Extensions.Logging;
using Model;
using View;
using Xunit;

public class ReportController_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<ReportController_Constructor>();
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);
        ReportController reportController = new(logger, reporter);

        Assert.True(reportController != null);
    }

}
