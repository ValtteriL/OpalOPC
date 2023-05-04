using Microsoft.Extensions.Logging;
using Model;
using View;

namespace Controller
{
    public class ReportController
    {

        ILogger _logger;
        IReporter _reporter;

        public ReportController(ILogger logger, IReporter reporter)
        {
            _reporter = reporter;
            _logger = logger;
        }

        // Reporter and targets, generate report
        public void GenerateReport(ICollection<Target> targets)
        {
            _logger.LogInformation("Generating report");

            // Merge opctarget endpoints
            foreach(Target target in targets)
            {
                target.MergeEndpoints();
            }

            Report report = new Report(targets);

            _reporter.printXMLReport(report);
        }

    }
}