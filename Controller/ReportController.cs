using Model;
using View;

namespace Controller
{
    public class ReportController
    {

        private IReporter reporter;

        public ReportController(IReporter reporter)
        {
            this.reporter = reporter;
        }

        // Reporter and targets, generate report
        public void GenerateReport(ICollection<Target> targets)
        {
            // Merge opctarget endpoints
            foreach(Target target in targets)
            {
                target.MergeEndpoints();
            }

            Report report = new Report(targets);

            reporter.printXMLReport(report);
        }

    }
}