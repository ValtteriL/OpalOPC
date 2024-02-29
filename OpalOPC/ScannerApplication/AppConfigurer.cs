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
        public static IHost ConfigureApplication(Options options, ILoggerProvider loggerProvider)
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
                    services.AddSingleton<INetworkDiscoveryController, NetworkDiscoveryController>();
                    services.AddSingleton<IMDNSUtil, MDNSUtil>();
                    services.AddHttpClient<IKnownVulnerabilityApiRequestUtil, KnownVulnerabilityApiRequestUtil>("KnownVulnerabilityPlugin", client =>
                    {
                        client.BaseAddress = new Uri(options.apiUri);
                        client.Timeout = TimeSpan.FromSeconds(300);
                    });
                    //services.RemoveAll<IHttpMessageHandlerBuilderFilter>(); // disable httpclient default logging
                    services.AddSingleton<IKnownVulnerabilityApiRequestUtil, KnownVulnerabilityApiRequestUtil>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(options.logLevel);
                    logging.AddProvider(loggerProvider);
                })
                .Build();
        }
    }
}
