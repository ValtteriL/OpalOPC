using Controller;
using Microsoft.Extensions.Logging;
using Model;
using View;

namespace OpcUaSecurityScanner
{
    public class Program
    {
        public static int Main(string[] args)
        {
            IBannerPrinter bannerPrinter = new BannerPrinter();
            bannerPrinter.printBanner();

            Options options = new Argparser(args).parseArgs();

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(options.logLevel)
                    .AddConsole();
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();

            IReporter reporter = new Reporter(options.xmlOutputStream!);

            DiscoveryController discoveryController = new DiscoveryController(logger);
            ICollection<Target> targets = discoveryController.DiscoverTargets(options.targets);

            SecurityTestController securityTestController = new SecurityTestController(logger);
            ICollection<Target> testedTargets = securityTestController.TestTargetSecurity(targets);

            ReportController reportController = new ReportController(logger, reporter);
            reportController.GenerateReport(testedTargets);

            return 0;
        }
    }
}
