using Controller;
using Microsoft.Extensions.Logging;
using Model;
using Util;
using View;


class OpalOPC
{
    public static int Main(string[] args)
    {
        TelemetryUtil.TrackEvent("CLI started");
        Options options = new Argparser(args).parseArgs();

        if (options.exitCode.HasValue)
        {
            Environment.Exit((int)options.exitCode);
        }

        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
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

        VersionCheckController versionCheckController = new(logger);
        versionCheckController.CheckVersion();

        ScanController scanController = new(logger, options.targets, options.xmlOutputStream!, Environment.CommandLine, options.authenticationData);
        scanController.Scan();

        if (options.xmlOutputReportName != null)
        {
            logger.LogInformation("{Message}", $"Report saved to {options.xmlOutputReportName} (Use browser to view it)");
        }

#if DEBUG
        logger.LogInformation("{Message}", $"Access report directly: http://localhost:8000/{options.xmlOutputReportName}");
#endif

        return 0;
    }
}
