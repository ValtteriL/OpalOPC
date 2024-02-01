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
            scanController.Scan(options.targets, options.commandLine, options.authenticationData, options.OutputStream!);

            if (options.OutputReportName != null)
            {
                logger.LogInformation("{Message}", $"Report saved to {options.OutputReportName} (Use browser to view it)");
            }
        }
    }
}
