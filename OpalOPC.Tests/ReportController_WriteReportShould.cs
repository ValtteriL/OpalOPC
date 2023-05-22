namespace Tests;
using View;
using Model;
using Xunit;
using Microsoft.Extensions.Logging;
using Controller;

public class ReportController_WriteReportShould
{
    [Fact]
    public void WriteReport_RunningBeforeGenerateReportCausesNullReferenceException()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryController_Constructor>();
        StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
        sw.AutoFlush = true;
        Reporter reporter = new Reporter(sw.BaseStream);
        ReportController reportController = new ReportController(logger, reporter);
        string runStatus = "hello";

        try
        {
            reportController.WriteReport(runStatus);
        }
        catch (System.NullReferenceException)
        {
            Assert.True(true);
            return;
        }

        Assert.True(false);
    }

    [Fact]
    public void WriteReport_AddsReportRunStatus()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<DiscoveryController_Constructor>();
        StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
        sw.AutoFlush = true;
        Reporter reporter = new Reporter(sw.BaseStream);
        ReportController reportController = new ReportController(logger, reporter);
        string runStatus = "hello";

        reportController.GenerateReport(new List<Target>(), DateTime.Now, DateTime.Now);
        reportController.WriteReport(runStatus);

        Assert.True(reportController.report!.RunStatus == runStatus);
    }
}