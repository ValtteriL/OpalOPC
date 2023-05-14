using Microsoft.Extensions.Logging;
using Model;
using View;

namespace Controller
{
    public class VersionCheckController
    {

        ILogger _logger;

        public VersionCheckController(ILogger logger)
        {
            _logger = logger;
        }

        // Check what is the latest version, if not same as the current version, warn
        // if no network connection - just generate trace message
        public void CheckVersion()
        {
            // TODO
            _logger.LogTrace("Using latest version");
        }
    }
}