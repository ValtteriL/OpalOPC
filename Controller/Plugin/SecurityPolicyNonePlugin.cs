using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityPolicyNonePlugin : Plugin
    {
        // If securitypolicy is none, application authentication is disabled (clients do not use certificate)
        // https://opcfoundation.org/forum/opc-certification-and-interoperability-testing/allowing-anonymous-user-access-a-security-breach-in-opc-ua-conversation/
        // https://opcfoundation.org/forum/opc-ua-implementation-stacks-tools-and-samples/rationale-for-server-authenticating-client-certificates/
        private PluginId _pluginId = PluginId.SecurityPolicyNone;
        private string _category = PluginCategories.TransportSecurity;

        public SecurityPolicyNonePlugin(ILogger logger) : base(logger) {}

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for Security Policy None");

            IEnumerable<Endpoint> NoneEndpoints = target.GetEndpointsBySecurityPolicyUri(SecurityPolicies.None);

            foreach (Endpoint endpoint in NoneEndpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has security policy None");
                endpoint.Issues.Add(Issues.SecurityPolicyNone);
            }

            return target;
        }

    }
}