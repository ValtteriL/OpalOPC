using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityPolicyBasic256Plugin : Plugin
    {
        // Basic256 is deprecated - https://profiles.opcfoundation.org/profilefolder/474

        private PluginId _pluginId = PluginId.SecurityPolicyBasic256;
        private string _category = PluginCategories.TransportSecurity;

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:L/I:L/A:N
        private double _severity = 4.8;

        public SecurityPolicyBasic256Plugin(ILogger logger) : base(logger) {}

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for Security Policy Basic256");

            IEnumerable<Endpoint> Basic256Endpoints = target.GetEndpointsBySecurityPolicyUri(SecurityPolicies.Basic256);

            foreach (Endpoint endpoint in Basic256Endpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} uses Basic256");
                endpoint.Issues.Add(Issues.SecurityPolicyBasic256);
            }

            return target;
        }

    }
}