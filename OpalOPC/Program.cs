using Controller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;
using ScannerApplication;
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

            options.commandLine = Environment.CommandLine;

            // configure application
            IHost _host = AppConfigurer.ConfigureApplication(options);
            IWorker worker = _host.Services.GetRequiredService<IWorker>();

            if (options.shouldDiscoverAndExit)
            {
                TelemetryUtil.TrackEvent("Network Discovery");
                INetworkDiscoveryController networkDiscoveryController = _host.Services.GetRequiredService<INetworkDiscoveryController>();
                List<Uri> targets = networkDiscoveryController.MulticastDiscoverTargets();
                Console.WriteLine("Discovered targets:");
                targets.ForEach(t => Console.WriteLine(t));
                return (int)ExitCodes.Success;
            }

            // run application
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
