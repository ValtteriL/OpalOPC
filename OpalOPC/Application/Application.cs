using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller;
using Microsoft.Extensions.Logging;
using Model;

namespace Application
{
    internal interface IWorker
    {
        void Run(Options options);
    }
    internal class Worker(ILogger<Worker> logger, IVersionCheckController versionCheckController, IScanController scanController) : IWorker
    {
        public void Run(Options options)
        {
            versionCheckController.CheckVersion();
            scanController.Scan(options.targets, Environment.CommandLine, options.authenticationData, options.OutputStream);

            if (options.OutputReportName != null)
            {
                logger.LogInformation("{Message}", $"Report saved to {options.OutputReportName} (Use browser to view it)");
            }

#if DEBUG
            logger.LogInformation("{Message}", $"Access report directly: http://localhost:8000/{options.OutputReportName}");
#endif
        }
    }
}
