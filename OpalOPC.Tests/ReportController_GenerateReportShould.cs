namespace Tests;
using View;
using Model;
using Xunit;
using Microsoft.Extensions.Logging;
using Controller;

public class ReportController_generateReportShould
{
    [Fact]
    public void GenerateReport_AddsReportProperty()
    {
        var loggerFactory = LoggerFactory.Create(builder => {});
        ILogger logger = loggerFactory.CreateLogger<DiscoveryController_Constructor>();
        StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
        sw.AutoFlush = true;
        Reporter reporter = new Reporter(sw.BaseStream);
        ReportController reportController = new ReportController(logger, reporter);

        Assert.True(reportController.report == null);

        reportController.GenerateReport(new List<Target>(), DateTime.Now, DateTime.Now);

        Assert.True(reportController.report != null);
    }
}