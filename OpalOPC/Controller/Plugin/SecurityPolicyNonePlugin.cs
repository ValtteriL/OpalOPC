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
        private static readonly PluginId s_pluginId = PluginId.SecurityPolicyNone;
        private static readonly string s_category = PluginCategories.TransportSecurity;
        private static readonly string s_issueTitle = "Security Policy None";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:L/A:N
        private static readonly double s_severity = 5.4;

        public SecurityPolicyNonePlugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity) { }

        public override (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions)
        {
            _logger.LogTrace("{Message}", $"Testing {discoveryUrl} for Security Policy None");

            List<ISecurityTestSession> sessions = [];

            if (endpointDescriptions.Find(d => d.SecurityPolicyUri == SecurityPolicies.None) != null)
            {
                _logger.LogTrace("{Message}", $"Endpoint {discoveryUrl} has security policy None");
                return (CreateIssue(), sessions);
            }

            return (null, sessions);
        }

    }
}
