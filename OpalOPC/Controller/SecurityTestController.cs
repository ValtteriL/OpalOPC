using Microsoft.Extensions.Logging;
using Model;
using Plugin;
using Util;

namespace Controller
{

    public class SecurityTestController
    {

        ILogger _logger;
        ICollection<IPlugin> _securityTestPlugins;
        readonly CancellationToken? _token;

        public SecurityTestController(ILogger logger, ICollection<IPlugin> SecurityTestPlugins, CancellationToken? token = null)
        {
            _logger = logger;
            _securityTestPlugins = SecurityTestPlugins;
            _token = token;
        }


        // Run all security tests and return result-populated opcTarget
        public ICollection<Target> TestTargetSecurity(ICollection<Target> opcTargets)
        {
            _logger.LogTrace($"Loaded {_securityTestPlugins.Count} security test plugins");
            _logger.LogTrace($"Plugins: {String.Join(", ", _securityTestPlugins.Select(p => (int)p.pluginId))}");

            _logger.LogDebug($"Starting security tests of {opcTargets.Count} targets");

            foreach (Target target in opcTargets)
            {
                _logger.LogDebug($"Testing {target.ApplicationName} ({target.ProductUri})");

                // run each test plugin against this target
                foreach (IPlugin plugin in _securityTestPlugins)
                {
                    TaskUtil.CheckForCancellation(_token);
                    plugin.Run(target);
                }
            }

            return opcTargets;
        }
    }
}