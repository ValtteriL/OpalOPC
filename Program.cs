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

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Example log message");

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
