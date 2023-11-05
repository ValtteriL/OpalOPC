using Controller;
using Microsoft.Extensions.Logging;
using View;
using Xunit;

namespace Tests;
public class ReportController_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
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
