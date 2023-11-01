using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;

namespace Plugin
{
    public class SecurityPolicyBasic256Plugin : PreAuthPlugin
    {
        // Basic256 is deprecated - https://profiles.opcfoundation.org/profilefolder/474

        private static readonly PluginId s_pluginId = PluginId.SecurityPolicyBasic256;
        private static readonly string s_category = PluginCategories.TransportSecurity;
        private static readonly string s_issueTitle = "Deprecated Security Policy Basic256";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static readonly double s_severity = 4.8;

        public SecurityPolicyBasic256Plugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity) { }

        public override (Issue?, ICollection<ISession>) Run(Endpoint endpoint)
        {
            _logger.LogTrace("{Message}", $"Testing {endpoint.EndpointUrl} for Security Policy Basic256");

            List<ISession> sessions = new();

            if (endpoint.SecurityPolicyUri == SecurityPolicies.Basic256)
            {
                _logger.LogTrace("{Message}", $"Endpoint {endpoint.EndpointUrl} uses Basic256");
                return (CreateIssue(), sessions);
            }

            return (null, sessions);
        }

    }
}
