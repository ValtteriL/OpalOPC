using Microsoft.Extensions.Logging;
using Model;
using Plugin;
using Util;
using View;

namespace Controller
{
    public class ScanController
    {
        readonly ILogger _logger;
        readonly ICollection<Uri> _discoveryUris;
        readonly Stream _reportOutputStream;
        readonly string _commandLine;
        readonly CancellationToken? _token;
        readonly AuthenticationData _authenticationData;

        public ScanController(ILogger logger, ICollection<Uri> discoveryUris, Stream reportOutputStream, string commandLine, AuthenticationData authenticationData, CancellationToken? token = null)
        {
            _logger = logger;
            _discoveryUris = discoveryUris;
            _reportOutputStream = reportOutputStream;
            _commandLine = commandLine;
            _token = token;
            _authenticationData = authenticationData;
        }

        public void Scan()
        {
            TelemetryUtil.TrackEvent("Scan started", GetScanProperties());

            DateTime start = DateTime.Now;
            _logger.LogInformation("{Message}", $"Starting OpalOPC {Util.VersionUtil.AppAssemblyVersion} ( https://opalopc.com )");

            if (_discoveryUris.Count == 0)
            {
                _logger.LogWarning("{Message}", "No targets were specified, so 0 applications will be scanned.");
            }

            IReporter reporter = new Reporter(_reportOutputStream);

            TaskUtil.CheckForCancellation(_token);

            DiscoveryController discoveryController = new(_logger, new DiscoveryUtil(), _token);
            ICollection<Target> targets = discoveryController.DiscoverTargets(_discoveryUris);

            TaskUtil.CheckForCancellation(_token);

            // Initialize security testing plugins
            ICollection<IPlugin> securityTestPlugins = new List<IPlugin> {
            new SecurityModeInvalidPlugin(_logger),
            new SecurityModeNonePlugin(_logger),

            new SecurityPolicyBasic128Rsa15Plugin(_logger),
            new SecurityPolicyBasic256Plugin(_logger),
            new SecurityPolicyNonePlugin(_logger),

            new AnonymousAuthenticationPlugin(_logger, _authenticationData),
            new SelfSignedCertificatePlugin(_logger),

            new ProvidedCredentialsPlugin(_logger, _authenticationData),
            new CommonCredentialsPlugin(_logger, _authenticationData),
            new BruteForcePlugin(_logger, _authenticationData),
            new RBACNotSupportedPlugin(_logger),
            new AuditingDisabledPlugin(_logger),
        };

            SecurityTestController securityTestController = new(_logger, securityTestPlugins, _token);
            ICollection<Target> testedTargets = securityTestController.TestTargetSecurity(targets);

            TaskUtil.CheckForCancellation(_token);

            DateTime end = DateTime.Now;

            TimeSpan ts = (end - start);
            string runStatus = $"OpalOPC done: {_discoveryUris.Count} Discovery URLs ({targets.Count} applications found) scanned in {Math.Round(ts.TotalSeconds, 2)} seconds";
            _logger.LogInformation("{Message}", runStatus);

            ReportController reportController = new(_logger, reporter, testedTargets, start, end, _commandLine, runStatus);
            reportController.WriteReport();

            TelemetryUtil.TrackEvent("Scan finished", GetScanResultProperties(reportController.report, ts));
        }

        private Dictionary<string, string> GetScanProperties()
        {
            return new()
            {
                { "NumberOfDiscoveryUris", _discoveryUris.Count.ToString() },
                { "NumberOfUserCertificates", _authenticationData.userCertificates.Count.ToString() },
                { "NumberOfAppCertificates", _authenticationData.applicationCertificates.Count.ToString() },
                { "NumberOfLoginCredentials", _authenticationData.loginCredentials.Count.ToString() },
                { "NumberOfBruteForceCredentials", _authenticationData.bruteForceCredentials.Count.ToString() },
            };
        }

        private Dictionary<string, string> GetScanResultProperties(Report report, TimeSpan timeSpan)
        {
            Dictionary<string, string> results = new()
            {
                { "NumberOfTargets", report.Targets.Count.ToString() },
                { "ScanTimeSeconds", timeSpan.TotalSeconds.ToString() },
                { "NumberOfIssues",  report.Targets.Sum(t => t.IssuesCount).ToString() },
                { "NumberOfErrors",  report.Targets.Sum(t => t.ErrorsCount).ToString() },
            };
            GetScanProperties().ToList().ForEach(x => results.Add(x.Key, x.Value));

            return results;
        }
    }
}
