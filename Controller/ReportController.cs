using Model;
using View;

namespace Controller
{
    public static class ReportController
    {

        // Reporter and targets, generate report
        public static void GenerateReport(IReporter reporter, ICollection<Target> targets)
        {
            // Merge opctarget endpoints
            foreach(Target target in targets)
            {
                target.MergeEndpoints();
            }

            Report report = new Report(targets);

            reporter.printJSONReport(report, "");
            reporter.printXMLReport(report, "");
        }

    }
}