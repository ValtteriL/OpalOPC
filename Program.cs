using Controller;
using Microsoft.Extensions.Logging;
using Model;
using View;


class OpalOPC
{
    public static int Main(string[] args)
    {
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

        DateTime start = DateTime.Now;
        logger.LogInformation($"Starting OpalOPC {Util.VersionUtil.AppAssemblyVersion} ( https://opalopc.app )");

        if (options.targets.Count == 0)
        {
            logger.LogWarning("No targets were specified, so 0 applications will be scanned.");
        }

        IReporter reporter = new Reporter(options.xmlOutputStream!);

        DiscoveryController discoveryController = new DiscoveryController(logger);
        ICollection<Target> targets = discoveryController.DiscoverTargets(options.targets);

        SecurityTestController securityTestController = new SecurityTestController(logger);
        ICollection<Target> testedTargets = securityTestController.TestTargetSecurity(targets);

        ReportController reportController = new ReportController(logger, reporter);
        reportController.GenerateReport(testedTargets);

        DateTime end = DateTime.Now;
        TimeSpan ts = (end - start);
        string runStatus = $"OpalOPC done: {options.targets.Count} Discovery URLs ({reportController.report!.Targets.Count} applications found) scanned in {Math.Round(ts.TotalSeconds, 2)} seconds";
        logger.LogInformation(runStatus);

        reportController.WriteReport(runStatus);

        if (options.xmlOutputReportName != null)
        {
            logger.LogInformation($"Report saved to {options.xmlOutputReportName}");
        }

        return 0;
    }
}
