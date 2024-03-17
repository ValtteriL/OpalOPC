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
    public static async Task<int> Main(string[] args)
    {
        TelemetryUtil.TrackEvent("CLI started");

        try
        {
            using Options options = new Argparser(args).parseArgs();

            if (options.shouldExit)
            {
                Environment.Exit((int)options.exitCode);
            }

            CLILoggerProvider loggerProvider = new(options.logLevel);

            // configure application
            IHost _host = AppConfigurer.ConfigureApplication(options, loggerProvider);
            IWorker worker = _host.Services.GetRequiredService<IWorker>();

            if (options.shouldStoreLicenseAndExit)
            {
                // store license
                ILicensingController licensingController = _host.Services.GetRequiredService<ILicensingController>();
                await licensingController.StoreLicense(options.licenseKey);
                return (int)ExitCodes.Success;
            }

            if (options.shouldDiscoverAndExit)
            {
                // discover targets
                INetworkDiscoveryController networkDiscoveryController = _host.Services.GetRequiredService<INetworkDiscoveryController>();
                IList<Uri> targets = await networkDiscoveryController.MulticastDiscoverTargets(5);
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
