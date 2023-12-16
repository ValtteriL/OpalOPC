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
        readonly ICollection<Uri> _targets;
        readonly Stream _reportOutputStream;
        readonly string _commandLine;
        readonly CancellationToken? _token;
        readonly AuthenticationData _authenticationData;

        public ScanController(ILogger logger, ICollection<Uri> targets, Stream reportOutputStream, string commandLine, AuthenticationData authenticationData, CancellationToken? token = null)
        {
            _logger = logger;
            _targets = targets;
            _reportOutputStream = reportOutputStream;
            _commandLine = commandLine;
            _token = token;
            _authenticationData = authenticationData;
        }

        public void Scan()
        {
            TelemetryUtil.TrackEvent("Scan started");
            DateTime start = DateTime.Now;
            _logger.LogInformation("{Message}", $"Starting OpalOPC {Util.VersionUtil.AppAssemblyVersion} ( https://opalopc.com )");

            if (_targets.Count == 0)
            {
                _logger.LogWarning("{Message}", "No targets were specified, so 0 applications will be scanned.");
            }

            IReporter reporter = new Reporter(_reportOutputStream);

            TaskUtil.CheckForCancellation(_token);

            DiscoveryController discoveryController = new(_logger, new DiscoveryUtil(), _token);
            ICollection<Target> targets = discoveryController.DiscoverTargets(_targets);

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

            ReportController reportController = new(_logger, reporter);

            DateTime end = DateTime.Now;

            TimeSpan ts = (end - start);
            string runStatus = $"OpalOPC done: {_targets.Count} Discovery URLs ({targets.Count} applications found) scanned in {Math.Round(ts.TotalSeconds, 2)} seconds";
            _logger.LogInformation("{Message}", runStatus);

            reportController.GenerateReport(testedTargets, start, end, _commandLine, runStatus);
            reportController.WriteReport();
        }
    }
}
