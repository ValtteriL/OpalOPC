using Microsoft.Extensions.Logging;
using Model;
using View;

namespace Controller
{
    public interface IReportController
    {
        Report GenerateReport(ICollection<Target> targets, DateTime Start, DateTime End, string commandLine, string runStatus);
        void WriteReports(Report report, Stream htmlOutputStream, Stream sarifOutputStream);
    }

    public class ReportController(ILogger<IReportController> logger, IHtmlReporter htmlReporter, ISarifReporter sarifReporter) : IReportController
    {
        public Report GenerateReport(ICollection<Target> targets, DateTime Start, DateTime End, string commandLine, string runStatus)
        {
            return new(targets, Start, End, commandLine, runStatus);
        }

        public void WriteReports(Report report, Stream htmlOutputStream, Stream sarifOutputStream)
        {
            logger.LogDebug("{Message}", "Generating report");
            htmlReporter.WriteReportToStream(report, htmlOutputStream);
            sarifReporter.WriteReportToStream(report, sarifOutputStream);
        }

    }
}
