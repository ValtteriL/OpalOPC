using Application;
using Controller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model;
using Util;
using View;


class OpalOPC
{
    public static int Main(string[] args)
    {
        TelemetryUtil.TrackEvent("CLI started");

        try
        {
            using Options options = new Argparser(args).parseArgs();

            if (options.exitCode.HasValue)
            {
                Environment.Exit((int)options.exitCode);
            }


            IHost _host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<IWorker, Worker>();
                    services.AddSingleton<IVersionCheckController, VersionCheckController>();
                    services.AddSingleton<IScanController, ScanController>();
                    services.AddSingleton<IReporter, Reporter>();
                    services.AddSingleton<IReportController, ReportController>();
                    services.AddSingleton<IDiscoveryController, DiscoveryController>();
                    services.AddSingleton<IDiscoveryUtil, DiscoveryUtil>();
                    services.AddSingleton<ISecurityTestController, SecurityTestController>();
                    services.AddSingleton<ITaskUtil, TaskUtil>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(options.logLevel);
                    logging.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = false;
                        options.TimestampFormat = "HH:mm:ss ";
                        options.SingleLine = true;
                    });
                })
                .Build();

            IWorker worker = _host.Services.GetRequiredService<IWorker>();
            worker.Run(options);
            return (int)ExitCodes.Success;
        }
        catch (Exception ex)
        {
            TelemetryUtil.TrackException(ex);
            Console.Error.WriteLine(ex.Message);
            return (int)ExitCodes.Error;
        }
    }
}
