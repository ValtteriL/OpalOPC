using Model;
using Opc.Ua;
using View;

namespace Controller
{
    public static class ReportController
    {

        // Given discoveryUri, discover all applications
        public static void GenerateReport(IReporter reporter, IEnumerable<OpcTarget> targets)
        {
            // TODO: merge opctarget endpoints and convert the collection into report
            foreach(OpcTarget target in targets)
            {
                MergeEndpoints(target);
            }

            reporter.printPdfReport()
        }

        private static OpcTarget MergeEndpoints(OpcTarget target)
        {
            // TODO
            return target;
        }

    }
}