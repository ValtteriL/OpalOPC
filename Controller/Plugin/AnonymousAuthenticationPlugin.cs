using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class AnonymousAuthenticationPlugin : Plugin
    {
        // "′anonymous′ should be used only for accessing non-critical UA server resources"
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
        // try anonymous authentication
        private PluginId _pluginId = PluginId.AnonymousAuthentication;
        private string _category = PluginCategories.Authentication;

        public AnonymousAuthenticationPlugin(ILogger logger) : base(logger) {}

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for anonymous access");

            IEnumerable<Endpoint> anonymousEndpoints = target.GetEndpointsByUserTokenType(UserTokenType.Anonymous);
            foreach (Endpoint endpoint in anonymousEndpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} allows anonymous authentication");
                endpoint.Issues.Add(Issues.AnonymousAuthentication);
            }

            return target;
        }

    }
}