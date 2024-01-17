using Controller;
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using View;
using Xunit;

namespace Tests;
public class ReportController_WriteReportShould
{
    [Fact]
    public void WriteReport_AddsReportRunStatus()
    {
        Mock<ILogger<IReportController>> _loggerMock = new();

        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<ReportController_WriteReportShould>();
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new();
        string runStatus = "hello";
        string commandLine = "commandline";
        ReportController reportController = new(_loggerMock.Object, reporter);
        Report report = reportController.GenerateReport(new List<Target>(), DateTime.Now, DateTime.Now, runStatus, commandLine);
        reportController.WriteReport(report, new MemoryStream());

        Assert.True(report.RunStatus == runStatus);
        Assert.True(report.Command == commandLine);
    }
}
