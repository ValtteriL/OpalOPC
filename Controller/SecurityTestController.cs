using Microsoft.Extensions.Logging;
using Model;
using Plugin;

namespace Controller
{

    public class SecurityTestController
    {

        ILogger _logger;
        ICollection<IPlugin> _securityTestPlugins;

        public SecurityTestController(ILogger logger, ICollection<IPlugin> SecurityTestPlugins)
        {
            _logger = logger;
            _securityTestPlugins = SecurityTestPlugins;
        }


        // Run all security tests and return result-populated opcTarget
        public ICollection<Target> TestTargetSecurity(ICollection<Target> opcTargets)
        {
            _logger.LogDebug($"Starting security tests of {opcTargets.Count} targets");

            foreach (Target target in opcTargets)
            {
                _logger.LogDebug($"Testing {target.ApplicationName} ({target.ProductUri})");

                // run each test plugin against this target
                foreach (IPlugin plugin in _securityTestPlugins)
                {
                    plugin.Run(target);
                }
            }

            return opcTargets;
        }
    }
}