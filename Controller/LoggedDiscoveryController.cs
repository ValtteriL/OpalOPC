using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Controller
{
    public class LoggedDiscoveryController : DiscoveryController
    {

        DiscoveryController _discoveryController;
        ILogger _logger;

        public LoggedDiscoveryController(DiscoveryController discoveryController, ILogger logger)
        {
            _discoveryController = discoveryController;
            _logger = logger;
        }

        public ICollection<Target> DiscoverTargets(ICollection<Uri> discoveryUris)
        {
            _discoveryController.DiscoverTargets(discoveryUris);
        }

        private ICollection<Target> DiscoverTargets(Uri discoveryUri)
        {
        }
    }
}