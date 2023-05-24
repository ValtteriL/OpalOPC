using Controller;
using Microsoft.Extensions.Logging;
using Model;
using Plugin;
using View;


class OpalOPC
{
    public static int Main(string[] args)
    {

        Options options = new Argparser(args).parseArgs();

        if (options.exitCode.HasValue)
        {
            Environment.Exit((int)options.exitCode);
        }

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

        VersionCheckController versionCheckController = new VersionCheckController(logger);
        versionCheckController.CheckVersion();

        DateTime start = DateTime.Now;
        logger.LogInformation($"Starting OpalOPC {Util.VersionUtil.AppAssemblyVersion} ( https://opalopc.com )");

        if (options.targets.Count == 0)
        {
            logger.LogWarning("No targets were specified, so 0 applications will be scanned.");
        }

        IReporter reporter = new Reporter(options.xmlOutputStream!);

        DiscoveryController discoveryController = new DiscoveryController(logger);
        ICollection<Target> targets = discoveryController.DiscoverTargets(options.targets);

        // Initialize security testing plugins
        ICollection<IPlugin> securityTestPlugins = new List<IPlugin> {
            new SecurityModeInvalidPlugin(logger),
            new SecurityModeNonePlugin(logger),

            new SecurityPolicyBasic128Rsa15Plugin(logger),
            new SecurityPolicyBasic256Plugin(logger),
            new SecurityPolicyNonePlugin(logger),

            new AnonymousAuthenticationPlugin(logger),
            new SelfSignedCertificatePlugin(logger),

            new CommonCredentialsPlugin(logger),
            new RBACNotSupportedPlugin(logger),
            new AuditingDisabledPlugin(logger),
        };

        SecurityTestController securityTestController = new SecurityTestController(logger, securityTestPlugins);
        ICollection<Target> testedTargets = securityTestController.TestTargetSecurity(targets);

        ReportController reportController = new ReportController(logger, reporter);

        DateTime end = DateTime.Now;
        reportController.GenerateReport(testedTargets, start, end);

        TimeSpan ts = (end - start);
        string runStatus = $"OpalOPC done: {options.targets.Count} Discovery URLs ({reportController.report!.Targets.Count} applications found) scanned in {Math.Round(ts.TotalSeconds, 2)} seconds";
        logger.LogInformation(runStatus);

        reportController.WriteReport(runStatus);

        if (options.xmlOutputReportName != null)
        {
            logger.LogInformation($"Report saved to {options.xmlOutputReportName} (Use browser to view it)");
        }

#if DEBUG
        logger.LogInformation($"Access report directly: http://localhost:8000/{options.xmlOutputReportName}");
#endif

        return 0;
    }
}
