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

            DiscoveryController discoveryController = new DiscoveryController();
            ICollection<Target> targets = discoveryController.DiscoverTargets(options.targets);

            SecurityTestController securityTestController = new SecurityTestController();
            ICollection<Target> testedTargets = securityTestController.TestTargetSecurity(targets);

            ReportController reportController = new ReportController(reporter);
            reportController.GenerateReport(testedTargets);

            return 0;
        }
    }
}
