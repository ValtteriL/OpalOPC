using Model;
using Opc.Ua;

namespace Controller
{
    public static class DiscoveryController
    {

        public static ICollection<Target> DiscoverTargets(ICollection<Uri> discoveryUris)
        {
            ICollection<Target> targets = new List<Target>();
            foreach(Uri uri in discoveryUris)
            {
                targets = targets.Concat(DiscoverTargets(uri)).ToList();
            }

            return targets;
        }

        // Given discoveryUri, discover all applications
        private static ICollection<Target> DiscoverTargets(Uri discoveryUri)
        {
            ICollection<Target> targets = new List<Target>();

            // https://reference.opcfoundation.org/Core/Part4/v105/docs/

            // DISCOVER (find services, check supported modes, security policies, and user tokens)
            // https://reference.opcfoundation.org/GDS/v105/docs/4.3
            // 1. Use FindServers on LDS to get list of application descriptions
            // 2. Use GetEndpoint on each application's discoveryurls to get list of endpointdescriptions
            // Endpointdescriptions contain supported security policy, security mode, and all supported user tokens 
            // If the ApplicationType is Discovery server, it can be used to find other servers with FindServersOnNetwork and iterate through them

            // Application
            // - Endpoint..N
            // - Access privileges..M

            // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.2
            // ask the server for all servers it knows about
            DiscoveryClient asd = DiscoveryClient.Create(discoveryUri);

            Console.WriteLine("### Discovering applications");
            ApplicationDescriptionCollection adc = asd.FindServers(null);

            //asd.FindServers(null, "opc.tcp://echo", null, null, out ApplicationDescriptionCollection adc);
            //asd.FindServers(null, "https://echo", null, null, out ApplicationDescriptionCollection adc);

            foreach (ApplicationDescription ad in adc)
            {

                Target target = new Target(ad);

                foreach (string s in ad.DiscoveryUrls)
                {

                    // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.4
                    // ask each discoveryUrl for endpoints
                    Console.WriteLine($"### Discovering endpoints for {ad.ApplicationName}:{s}");

                    DiscoveryClient sss = DiscoveryClient.Create(new Uri(s));
                    EndpointDescriptionCollection edc = sss.GetEndpoints(null);

                    target.AddServer(s, edc);

                    if (ad.ApplicationType == ApplicationType.DiscoveryServer)
                    {

                        // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.3
                        // ask the network servers this server knows about
                        // only works with discoveryservers

                        ServerOnNetworkCollection sonc = sss.FindServersOnNetwork(0, 0, null, out DateTime dt);
                        foreach (ServerOnNetwork son in sonc)
                        {
                            Console.WriteLine($"SERVER ON NETWORK: {son.DiscoveryUrl}");
                            // GetEndpoints could be used to check the endpoint securities of the found servers
                        }
                    }
                }

                targets.Add(target);
            }

            return targets;
        }
    }
}