using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityPolicyBasic128Rsa15Plugin : Plugin
    {
        // Basic128Rsa15 is deprecated - https://profiles.opcfoundation.org/profilefolder/474

        private static PluginId _pluginId = PluginId.SecurityPolicyBasic128Rsa15;
        private static string _category = PluginCategories.TransportSecurity;
        private static string _issueTitle = "Deprecated Security Policy Basic128Rsa15";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static double _severity = 4.8;

        public SecurityPolicyBasic128Rsa15Plugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) {}

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for Security Policy Basic128Rsa15");

            IEnumerable<Endpoint> Basic128Rsa15Endpoints = target.GetEndpointsBySecurityPolicyUri(SecurityPolicies.Basic128Rsa15);

            foreach (Endpoint endpoint in Basic128Rsa15Endpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} uses Basic128Rsa15");
                endpoint.Issues.Add(CreateIssue());
            }

            return target;
        }

    }
}