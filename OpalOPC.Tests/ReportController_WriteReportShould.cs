using Controller;
using Microsoft.Extensions.Logging;
using Model;
using View;
using Xunit;

namespace Tests;
public class ReportController_WriteReportShould
{
    [Fact]
    public void WriteReport_RunningBeforeGenerateReportCausesNullReferenceException()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<ReportController_WriteReportShould>();
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);
        ReportController reportController = new(logger, reporter);

        try
        {
            reportController.WriteReport();
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
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<ReportController_WriteReportShould>();
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);
        ReportController reportController = new(logger, reporter);
        string runStatus = "hello";
        string commandLine = "commandline";

        reportController.GenerateReport(new List<Target>(), DateTime.Now, DateTime.Now, commandLine, runStatus);
        reportController.WriteReport();

        Assert.True(reportController.report!.RunStatus == runStatus);
        Assert.True(reportController.report!.Command == commandLine);
    }
}
