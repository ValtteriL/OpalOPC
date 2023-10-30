namespace Tests;
using Controller;
using Microsoft.Extensions.Logging;
using Model;
using View;
using Xunit;

public class ReportController_WriteReportShould
{
    [Fact]
    public void WriteReport_RunningBeforeGenerateReportCausesNullReferenceException()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<ReportController_WriteReportShould>();
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);
        ReportController reportController = new(logger, reporter);
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
        ILogger logger = loggerFactory.CreateLogger<ReportController_WriteReportShould>();
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);
        ReportController reportController = new(logger, reporter);
        string runStatus = "hello";
        string commandLine = string.Empty;

        reportController.GenerateReport(new List<Target>(), DateTime.Now, DateTime.Now, commandLine);
        reportController.WriteReport(runStatus);

        Assert.True(reportController.report!.RunStatus == runStatus);
    }
}
