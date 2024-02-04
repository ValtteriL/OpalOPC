namespace Controller
{
    public interface INetworkDiscoveryController
    {
        public List<Uri> MulticastDiscoverTargets();
    }

    public class NetworkDiscoveryController : INetworkDiscoveryController
    {
        public List<Uri> MulticastDiscoverTargets()
        {
            return [];
        }

    }
}
