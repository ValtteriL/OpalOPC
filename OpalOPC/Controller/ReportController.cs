using Microsoft.Extensions.Logging;
using Model;
using View;

namespace Controller
{
    public class ReportController
    {

        readonly ILogger _logger;
        readonly IReporter _reporter;
        public Report? report { get; private set; }

        public ReportController(ILogger logger, IReporter reporter)
        {
            _reporter = reporter;
            _logger = logger;
        }

        // Reporter and targets, generate report
        public void GenerateReport(ICollection<Target> targets, DateTime Start, DateTime End, string commandLine, string runStatus)
        {
            _logger.LogDebug("{Message}", "Generating report");
            report = new Report(targets, Start, End, commandLine, runStatus);
        }

        public void WriteReport()
        {
            if (report == null)
            {
                throw new NullReferenceException("Report is null");
            }
            _reporter.PrintXHTMLReport(report!);
        }

    }
}
