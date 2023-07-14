using Controller;
using Microsoft.Extensions.Logging;
using Model;
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

        ScanController scanController = new ScanController(logger, options.targets, options.xmlOutputStream!);
        scanController.Scan();

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
