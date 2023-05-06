using Controller;
using Microsoft.Extensions.Logging;
using Model;
using View;


class OpalOPC
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
                .AddSimpleConsole(options =>
                {
                    options.IncludeScopes = false;
                    options.TimestampFormat = "HH:mm:ss ";
                    options.SingleLine = true;
                });
        });

        ILogger logger = loggerFactory.CreateLogger<OpalOPC>();

        IReporter reporter = new Reporter(options.xmlOutputStream!);

        DiscoveryController discoveryController = new DiscoveryController(logger);
        ICollection<Target> targets = discoveryController.DiscoverTargets(options.targets);

        SecurityTestController securityTestController = new SecurityTestController(logger);
        ICollection<Target> testedTargets = securityTestController.TestTargetSecurity(targets);

        ReportController reportController = new ReportController(logger, reporter);
        reportController.GenerateReport(testedTargets);

        if (options.xmlOutputReportName != null)
        {
            logger.LogInformation($"Report saved to {options.xmlOutputReportName}");
        }

        return 0;
    }
}
