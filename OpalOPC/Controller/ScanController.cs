using Microsoft.Extensions.Logging;
using Model;
using Org.BouncyCastle.Tsp;
using Plugin;
using Util;
using View;

namespace Controller
{
    public interface IScanController
    {
        void Scan(ICollection<Uri> discoveryUris, string commandLine, AuthenticationData authenticationData, Stream outputStream);
    }

    public class ScanController(ILogger<ScanController> logger, IReportController reportController, IDiscoveryController discoveryController, ISecurityTestController securityTestController, ITaskUtil taskUtil) : IScanController
    {
        public void Scan(ICollection<Uri> discoveryUris, string commandLine, AuthenticationData authenticationData, Stream outputStream)
        {
            TelemetryUtil.TrackEvent("Scan started", GetScanProperties(discoveryUris, authenticationData));

            DateTime start = DateTime.Now;
            logger.LogInformation("{Message}", $"Starting OpalOPC {Util.VersionUtil.AppAssemblyVersion} ( https://opalopc.com )");

            if (discoveryUris.Count == 0)
            {
                logger.LogWarning("{Message}", "No targets were specified, so 0 applications will be scanned.");
            }

            taskUtil.CheckForCancellation();

            ICollection<Target> targets = discoveryController.DiscoverTargets(discoveryUris);

            taskUtil.CheckForCancellation();

            // Initialize security testing plugins
            ICollection<IPlugin> securityTestPlugins = new List<IPlugin> {
            new SecurityModeInvalidPlugin(logger),
            new SecurityModeNonePlugin(logger),

            new SecurityPolicyBasic128Rsa15Plugin(logger),
            new SecurityPolicyBasic256Plugin(logger),
            new SecurityPolicyNonePlugin(logger),

            new AnonymousAuthenticationPlugin(logger, authenticationData),
            new SelfSignedCertificatePlugin(logger),

            new ProvidedCredentialsPlugin(logger, authenticationData),
            new CommonCredentialsPlugin(logger, authenticationData),
            new BruteForcePlugin(logger, authenticationData),
            new RBACNotSupportedPlugin(logger),
            new AuditingDisabledPlugin(logger),
        };

            ICollection<Target> testedTargets = securityTestController.TestTargetSecurity(securityTestPlugins, targets);

            taskUtil.CheckForCancellation();

            DateTime end = DateTime.Now;

            TimeSpan ts = (end - start);
            string runStatus = $"OpalOPC done: {discoveryUris.Count} Discovery URLs ({targets.Count} applications found) scanned in {Math.Round(ts.TotalSeconds, 2)} seconds";
            logger.LogInformation("{Message}", runStatus);


            Report report = reportController.GenerateReport(testedTargets, start, end, commandLine, runStatus);
            reportController.WriteReport(report, outputStream);

            TelemetryUtil.TrackEvent("Scan finished", GetScanResultProperties(report, ts, discoveryUris, authenticationData));
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
