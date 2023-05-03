using Controller;
using Model;
using View;

namespace OpcUaSecurityScanner
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            IBannerPrinter bannerPrinter = new BannerPrinter();

            bannerPrinter.printBanner();

            Options options = new Argparser(args).parseArgs();

            IReporter reporter = new Reporter(options.xmlOutputStream!);

            ICollection<Target> targets = DiscoveryController.DiscoverTargets(options.targets);
            ICollection<Target> testedTargets = SecurityTestController.TestTargetSecurity(targets);

            ReportController.GenerateReport(reporter, testedTargets);

            return 0;
        }
    }
}
