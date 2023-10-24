using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityPolicyBasic256Plugin : Plugin
    {
        // Basic256 is deprecated - https://profiles.opcfoundation.org/profilefolder/474

        private static readonly PluginId _pluginId = PluginId.SecurityPolicyBasic256;
        private static readonly string _category = PluginCategories.TransportSecurity;
        private static readonly string _issueTitle = "Deprecated Security Policy Basic256";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static readonly double _severity = 4.8;

        public SecurityPolicyBasic256Plugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) {}

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for Security Policy Basic256");

            IEnumerable<Endpoint> Basic256Endpoints = target.GetEndpointsBySecurityPolicyUri(SecurityPolicies.Basic256);

            foreach (Endpoint endpoint in Basic256Endpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} uses Basic256");
                endpoint.Issues.Add(CreateIssue());
            }

            return target;
        }

    }
}