using Model;
using View;

namespace Controller
{
    public static class ReportController
    {

        // Reporter and targets, generate report
        public static void GenerateReport(IReporter reporter, ICollection<OpcTarget> targets)
        {
            // Merge opctarget endpoints
            foreach(OpcTarget target in targets)
            {
                target.MergeEndpoints();
            }

            reporter.printJSONReport(targets, "");

            reporter.printXMLReport(targets, "");
        }

    }
}