using Controller;
using Model;
using View;

namespace OpcUaSecurityScanner
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IReporter reporter = new Reporter();

            reporter.printBanner();

            Options options = new Argparser(args).parseArgs();

            ICollection<Target> targets = DiscoveryController.DiscoverTargets(options.targets);
            ICollection<Target> testedTargets = SecurityTestController.TestTargetSecurity(targets);

            ReportController.GenerateReport(reporter, testedTargets);
        }
    }
}
