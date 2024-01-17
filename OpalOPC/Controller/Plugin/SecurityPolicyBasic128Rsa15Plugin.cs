using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityPolicyBasic128Rsa15Plugin : PreAuthPlugin
    {
        // Basic128Rsa15 is deprecated - https://profiles.opcfoundation.org/profilefolder/474

        private static readonly PluginId s_pluginId = PluginId.SecurityPolicyBasic128Rsa15;
        private static readonly string s_category = PluginCategories.TransportSecurity;
        private static readonly string s_issueTitle = "Deprecated Security Policy Basic128Rsa15";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static readonly double s_severity = 4.8;

        public SecurityPolicyBasic128Rsa15Plugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity) { }

        public override (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions)
        {
            _logger.LogTrace("{Message}", $"Testing {discoveryUrl} for Security Policy Basic128Rsa15");

            List<ISecurityTestSession> sessions = [];

            if (endpointDescriptions.Find(d => d.SecurityPolicyUri == SecurityPolicies.Basic128Rsa15) != null)
            {
                _logger.LogTrace("{Message}", $"Endpoint {discoveryUrl} has security policy Basic128Rsa15");
                return (CreateIssue(), sessions);
            }

            return (null, sessions);
        }

    }
}
