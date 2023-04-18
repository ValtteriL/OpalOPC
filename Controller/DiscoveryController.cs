using Model;
using Opc.Ua;

namespace Controller
{
    public static class DiscoveryController
    {

        // Given discoveryUri, discover all applications
        public static IEnumerable<OpcTarget> DiscoverTargets(Uri discoveryUri)
        {
            IEnumerable<OpcTarget> targets = new List<OpcTarget>();

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

            foreach (ApplicationDescription ad in adc)
            {

                OpcTarget target = new OpcTarget(ad);

                foreach (string s in ad.DiscoveryUrls)
                {

                    // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.4
                    // ask each discoveryUrl for endpoints
                    Console.WriteLine($"### Discovering endpoints for {ad.ApplicationName}:{s}");

                    DiscoveryClient sss = DiscoveryClient.Create(new Uri(s.Replace("echo", "echo.koti.kontu").Replace("opc.http", "http"))); // TODO: make something smarter up
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

                targets = targets.Append(target);
            }

            return targets;
        }
    }
}