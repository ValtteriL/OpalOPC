using System.Globalization;
using Controller;
using Model;
using Opc.Ua;
using View;

namespace OpcUaSecurityScanner
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IReporter reporter = new Reporter();

            reporter.printBanner();
            reporter.printLibraryVersion();


            // TODO: parse args

            ICollection<Target> targets = DiscoveryController.DiscoverTargets(new Uri("opc.tcp://echo:53530"));
            ICollection<Target> testedTargets = SecurityTestController.TestTargetSecurity(targets);

            ReportController.GenerateReport(reporter, testedTargets);
        }
    }
}
