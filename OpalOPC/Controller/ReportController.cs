using Microsoft.Extensions.Logging;
using Model;
using View;

namespace Controller
{
    public class ReportController
    {

        readonly ILogger _logger;
        readonly IReporter _reporter;
        public Report? report { get; set; }

        public ReportController(ILogger logger, IReporter reporter)
        {
            _reporter = reporter;
            _logger = logger;
        }

        // Reporter and targets, generate report
        public void GenerateReport(ICollection<Target> targets, DateTime Start, DateTime End, string commandLine)
        {
            _logger.LogDebug("Generating report");

            // Merge opctarget endpoints
            foreach(Target target in targets)
            {
                target.MergeEndpoints();
            }

            report = new Report(targets, Start, End, commandLine);
        }

        public void WriteReport(string runStatus)
        {
            report!.RunStatus = runStatus;
            _reporter.PrintXHTMLReport(report!);
        }

    }
}