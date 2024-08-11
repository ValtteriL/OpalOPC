using Controller;
using Microsoft.Extensions.Logging;
using Model;
using Util;

namespace ScannerApplication
{
    public interface IWorker
    {
        Task<int> Run(Options options);
    }
    public class Worker(ILogger<Worker> logger, IReportController reportController, IDiscoveryController discoveryController, ISecurityTestController securityTestController, ITaskUtil taskUtil) : IWorker
    {
        public async Task<int> Run(Options options)
        {
            DateTime start = DateTime.Now;
            logger.LogInformation("{Message}", $"Starting OpalOPC {Util.VersionUtil.AppVersion} ( https://opalopc.com )");

            if (options.targets.Count == 0)
            {
                logger.LogWarning("{Message}", "No targets were specified, so 0 applications will be scanned.");
            }

            taskUtil.CheckForCancellation();

            ICollection<Target> targets = discoveryController.DiscoverTargets(options.targets);

            taskUtil.CheckForCancellation();

            ICollection<Target> testedTargets = securityTestController.TestTargetSecurity(targets, options.authenticationData);

            taskUtil.CheckForCancellation();

            DateTime end = DateTime.Now;

            TimeSpan ts = (end - start);
            string runStatus = $"OpalOPC done: {options.targets.Count} Discovery URLs ({targets.Count} applications found) scanned in {Math.Round(ts.TotalSeconds, 2)} seconds";
            logger.LogInformation("{Message}", runStatus);

            Report report = reportController.GenerateReport(testedTargets, start, end, options.commandLine, runStatus);
            reportController.WriteReports(report, options.HtmlOutputStream!, options.SarifOutputStream!);

            logger.LogInformation("{Message}", $"HTML report saved to {options.HtmlOutputReportName} (Use browser to view it)");
            logger.LogInformation("{Message}", $"SARIF report saved to {options.SarifOutputReportName} (Use SARIF viewer to view it)");

            return ExitCodes.Success;
        }
    }
}
