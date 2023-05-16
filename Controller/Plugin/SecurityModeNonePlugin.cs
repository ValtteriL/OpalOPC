using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityModeNonePlugin : Plugin
    {
        // "The SecurityMode should be ′Sign′ or ′SignAndEncrypt′."
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/

        private PluginId _pluginId = PluginId.SecurityModeNone;
        private string _category = PluginCategories.TransportSecurity;

        public SecurityModeNonePlugin(ILogger logger) : base(logger) {}

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for Message Security Mode None");

            IEnumerable<Endpoint> NoneSecurityModeEndpoints = target.GetEndpointsBySecurityMode(MessageSecurityMode.None);

            foreach (Endpoint endpoint in NoneSecurityModeEndpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has security mode None");
                endpoint.Issues.Add(Issues.SecurityModeNone);
            }

            return target;
        }

    }
}