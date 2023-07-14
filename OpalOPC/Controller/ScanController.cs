using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model;
using Opc.Ua;
using Plugin;
using View;

namespace Controller
{
    public class ScanController
    {
        ILogger _logger;
        ICollection<Uri> _targets;
        Stream _reportOutputStream;
        string _commandLine;

        public ScanController(ILogger logger, ICollection<Uri> targets, Stream reportOutputStream, string commandLine)
        {
            _logger = logger;
            _targets = targets;
            _reportOutputStream = reportOutputStream;
            _commandLine = commandLine;
        }

        public void Scan()
        {
            DateTime start = DateTime.Now;
            _logger.LogInformation($"Starting OpalOPC {Util.VersionUtil.AppAssemblyVersion} ( https://opalopc.com )");

            if (_targets.Count == 0)
            {
                _logger.LogWarning("No targets were specified, so 0 applications will be scanned.");
            }

            IReporter reporter = new Reporter(_reportOutputStream);

            DiscoveryController discoveryController = new DiscoveryController(_logger);
            ICollection<Target> targets = discoveryController.DiscoverTargets(_targets);

            // Initialize security testing plugins
            ICollection<IPlugin> securityTestPlugins = new List<IPlugin> {
            new SecurityModeInvalidPlugin(_logger),
            new SecurityModeNonePlugin(_logger),

            new SecurityPolicyBasic128Rsa15Plugin(_logger),
            new SecurityPolicyBasic256Plugin(_logger),
            new SecurityPolicyNonePlugin(_logger),

            new AnonymousAuthenticationPlugin(_logger),
            new SelfSignedCertificatePlugin(_logger),

            new CommonCredentialsPlugin(_logger),
            new RBACNotSupportedPlugin(_logger),
            new AuditingDisabledPlugin(_logger),
        };

            SecurityTestController securityTestController = new SecurityTestController(_logger, securityTestPlugins);
            ICollection<Target> testedTargets = securityTestController.TestTargetSecurity(targets);

            ReportController reportController = new ReportController(_logger, reporter);

            DateTime end = DateTime.Now;
            reportController.GenerateReport(testedTargets, start, end, _commandLine);

            TimeSpan ts = (end - start);
            string runStatus = $"OpalOPC done: {_targets.Count} Discovery URLs ({reportController.report!.Targets.Count} applications found) scanned in {Math.Round(ts.TotalSeconds, 2)} seconds";
            _logger.LogInformation(runStatus);

            reportController.WriteReport(runStatus);
        }
    }
}