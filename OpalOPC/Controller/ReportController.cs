using Microsoft.Extensions.Logging;
using Model;
using View;

namespace Controller
{
    public interface IReportController
    {
        Report GenerateReport(ICollection<Target> targets, DateTime Start, DateTime End, string commandLine, string runStatus);
        void WriteReport(Report report, Stream outputStream);
    }

    public class ReportController(ILogger<ReportController> logger, IReporter reporter) : IReportController
    {
        public Report GenerateReport(ICollection<Target> targets, DateTime Start, DateTime End, string commandLine, string runStatus)
        {
            return new(targets, Start, End, commandLine, runStatus);
        }

        public void WriteReport(Report report, Stream outputStream)
        {
            logger.LogDebug("{Message}", "Generating report");
            reporter.PrintXHTMLReport(report, outputStream);
        }

    }
}
