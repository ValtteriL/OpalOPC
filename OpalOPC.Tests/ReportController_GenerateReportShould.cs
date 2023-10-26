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
        StreamWriter sw = new(new MemoryStream());
        sw.AutoFlush = true;
        Reporter reporter = new(sw.BaseStream);
        ReportController reportController = new(logger, reporter);
        string commandLine = string.Empty;

        Assert.True(reportController.report == null);

        reportController.GenerateReport(new List<Target>(), DateTime.Now, DateTime.Now, commandLine);

        Assert.True(reportController.report != null);
    }
}