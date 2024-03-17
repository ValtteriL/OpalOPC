﻿using Controller;
using Microsoft.Extensions.Logging;
using Model;
using Util;

namespace ScannerApplication
{
    public interface IWorker
    {
        void Run(Options options);
    }
    public class Worker(ILogger<Worker> logger, ILicensingController licensingController, IReportController reportController, IDiscoveryController discoveryController, ISecurityTestController securityTestController, ITaskUtil taskUtil) : IWorker
    {
        public void Run(Options options)
        {
            using ILicensingController myLicensingController = licensingController; // automatically cleanup machine activation when done

            TelemetryUtil.TrackEvent("Scan started", GetScanProperties(options.targets, options.authenticationData));

            DateTime start = DateTime.Now;
            logger.LogInformation("{Message}", $"Starting OpalOPC {Util.VersionUtil.AppAssemblyVersion} ( https://opalopc.com )");


            if (!myLicensingController.IsLicensed().Result)
            {
                TelemetryUtil.TrackEvent("Not licensed");
                logger.LogCritical("{Message}", "Software license validation failed. Exiting");
                return;
            }
            else
            {
                TelemetryUtil.TrackEvent("License valid");
                logger.LogInformation("{Message}", "Software license validated successfully");
            }

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

            TelemetryUtil.TrackEvent("Scan finished", GetScanResultProperties(report, ts, options.targets, options.authenticationData));


            logger.LogInformation("{Message}", $"HTML report saved to {options.HtmlOutputReportName} (Use browser to view it)");
            logger.LogInformation("{Message}", $"SARIF report saved to {options.SarifOutputReportName} (Use SARIF viewer to view it)");
        }

        private static Dictionary<string, string> GetScanProperties(ICollection<Uri> discoveryUris, AuthenticationData authenticationData)
        {
            return new()
            {
                { "NumberOfDiscoveryUris", discoveryUris.Count.ToString() },
                { "NumberOfUserCertificates", authenticationData.userCertificates.Count.ToString() },
                { "NumberOfAppCertificates", authenticationData.applicationCertificates.Count.ToString() },
                { "NumberOfLoginCredentials", authenticationData.loginCredentials.Count.ToString() },
                { "NumberOfBruteForceCredentials", authenticationData.bruteForceCredentials.Count.ToString() },
            };
        }

        private static Dictionary<string, string> GetScanResultProperties(Report report, TimeSpan timeSpan, ICollection<Uri> discoveryUris, AuthenticationData authenticationData)
        {
            Dictionary<string, string> results = new()
            {
                { "NumberOfTargets", report.Targets.Count.ToString() },
                { "ScanTimeSeconds", timeSpan.TotalSeconds.ToString() },
                { "NumberOfIssues",  report.Targets.Sum(t => t.IssuesCount).ToString() },
                { "NumberOfErrors",  report.Targets.Sum(t => t.ErrorsCount).ToString() },
            };
            GetScanProperties(discoveryUris, authenticationData).ToList().ForEach(x => results.Add(x.Key, x.Value));

            return results;
        }
    }
}
