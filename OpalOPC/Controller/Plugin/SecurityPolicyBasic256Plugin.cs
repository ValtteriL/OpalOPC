using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;

namespace Plugin
{
    public class SecurityPolicyBasic256Plugin : PreAuthPlugin
    {
        // Basic256 is deprecated - https://profiles.opcfoundation.org/profilefolder/474

        private static readonly PluginId _pluginId = PluginId.SecurityPolicyBasic256;
        private static readonly string _category = PluginCategories.TransportSecurity;
        private static readonly string _issueTitle = "Deprecated Security Policy Basic256";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static readonly double _severity = 4.8;

        public SecurityPolicyBasic256Plugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) {}

        public override (Issue?, ICollection<ISession>) Run(Endpoint endpoint)
        {
            _logger.LogTrace($"Testing {endpoint} for Security Policy Basic256");

            List<ISession> sessions = new();

            if (endpoint.SecurityPolicyUri == SecurityPolicies.Basic256)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} uses Basic256");
                return (CreateIssue(), sessions);
            }

            return (null, sessions);
        }

    }
}