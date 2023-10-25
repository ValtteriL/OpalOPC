using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityPolicyNonePlugin : PreAuthPlugin
    {
        // If securitypolicy is none, application authentication is disabled (clients do not use certificate)
        // https://opcfoundation.org/forum/opc-certification-and-interoperability-testing/allowing-anonymous-user-access-a-security-breach-in-opc-ua-conversation/
        // https://opcfoundation.org/forum/opc-ua-implementation-stacks-tools-and-samples/rationale-for-server-authenticating-client-certificates/
        private static readonly PluginId _pluginId = PluginId.SecurityPolicyNone;
        private static readonly string _category = PluginCategories.TransportSecurity;
        private static readonly string _issueTitle = "Security Policy None";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:L/A:N
        private static readonly double _severity = 5.4;

        public SecurityPolicyNonePlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) {}

        public override Issue? Run(Endpoint endpoint)
        {
            _logger.LogTrace($"Testing {endpoint} for Security Policy None");

            if (endpoint.SecurityPolicyUri == SecurityPolicies.None)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has security policy None");
                return CreateIssue();
            }

            return null;
        }

    }
}