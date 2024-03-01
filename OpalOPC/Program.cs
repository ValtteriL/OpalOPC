using Controller;
using Logger;
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

            CLILoggerProvider loggerProvider = new(options.logLevel);

            // configure application
            IHost _host = AppConfigurer.ConfigureApplication(options, loggerProvider);
            IWorker worker = _host.Services.GetRequiredService<IWorker>();

            if (options.shouldDiscoverAndExit)
            {
                INetworkDiscoveryController networkDiscoveryController = _host.Services.GetRequiredService<INetworkDiscoveryController>();
                IList<Uri> targets = networkDiscoveryController.MulticastDiscoverTargets(5);
                Console.WriteLine("Discovered targets:");
                foreach (Uri target in targets)
                {
                    Console.WriteLine(target);
                }
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
