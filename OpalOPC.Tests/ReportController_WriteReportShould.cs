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
        Mock<ISarifReporter> _sarifReporterMock = new();

        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<ReportController_WriteReportShould>();

        HtmlReporter htmlReporter = new();

        string runStatus = "hello";
        string commandLine = "commandline";
        ReportController reportController = new(_loggerMock.Object, htmlReporter, _sarifReporterMock.Object);
        Report report = reportController.GenerateReport([], DateTime.Now, DateTime.Now, commandLine, runStatus);
        reportController.WriteReports(report, new MemoryStream(), new MemoryStream());

        Assert.True(report.RunStatus == runStatus);
        Assert.True(report.Command == commandLine);
    }
}
