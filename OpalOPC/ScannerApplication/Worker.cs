using Controller;
using Microsoft.Extensions.Logging;
using Model;

namespace ScannerApplication
{
    public interface IWorker
    {
        void Run(Options options);
    }
    public class Worker(ILogger<Worker> logger, IScanController scanController) : IWorker
    {
        public void Run(Options options)
        {
            scanController.Scan(options.targets, options.commandLine, options.authenticationData, options.HtmlOutputStream!, options.SarifOutputStream!);

            logger.LogInformation("{Message}", $"HTML report saved to {options.HtmlOutputReportName} (Use browser to view it)");
            logger.LogInformation("{Message}", $"SARIF report saved to {options.SarifOutputReportName} (Use SARIF viewer to view it)");
        }
    }
}
