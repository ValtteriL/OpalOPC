using Controller;
using Microsoft.Extensions.Logging;
using Model;
using View;
using Xunit;

namespace Tests;
public class ReportController_generateReportShould
{
    [Fact]
    public void GenerateReport_AddsReportProperty()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<ReportController_generateReportShould>();
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);
        ReportController reportController = new(logger, reporter);
        string commandLine = string.Empty;

        Assert.True(reportController.report == null);

        reportController.GenerateReport(new List<Target>(), DateTime.Now, DateTime.Now, commandLine);

        Assert.True(reportController.report != null);
    }
}
