using Controller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model;
using Plugin;
using Util;
using View;

namespace ScannerApplication
{
    public class AppConfigurer
    {
        public static IHost ConfigureApplication(Options options, ILoggerProvider? loggerProvider = null)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<IWorker, Worker>();
                    services.AddSingleton<IScanController, ScanController>();
                    services.AddSingleton<IReporter, Reporter>();
                    services.AddSingleton<IReportController, ReportController>();
                    services.AddSingleton<IDiscoveryController, DiscoveryController>();
                    services.AddSingleton<IDiscoveryUtil, DiscoveryUtil>();
                    services.AddSingleton<ISecurityTestController, SecurityTestController>();
                    services.AddSingleton<ITaskUtil, TaskUtil>();
                    services.AddSingleton<IPluginRepository, PluginRepository>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(options.logLevel);

                    if (loggerProvider != null)
                        logging.AddProvider(loggerProvider);
                    else
                        logging.AddSimpleConsole(options =>
                        {
                            options.IncludeScopes = false;
                            options.TimestampFormat = "HH:mm:ss ";
                            options.SingleLine = true;
                        });
                })
                .Build();
        }
    }
}
