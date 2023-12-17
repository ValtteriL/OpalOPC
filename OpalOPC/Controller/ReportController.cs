using Microsoft.Extensions.Logging;
using Model;
using View;

namespace Controller
{
    public class ReportController
    {

        readonly ILogger _logger;
        readonly IReporter _reporter;
        public Report report { get; private set; }

        public ReportController(ILogger logger, IReporter reporter, ICollection<Target> targets, DateTime Start, DateTime End, string commandLine, string runStatus)
        {
            _reporter = reporter;
            _logger = logger;
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
