using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityModeInvalidPlugin : Plugin
    {
        // "The SecurityMode should be ′Sign′ or ′SignAndEncrypt′."
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/

        private static readonly PluginId _pluginId = PluginId.SecurityModeInvalid;
        private static readonly string _category = PluginCategories.TransportSecurity;
        private static readonly string _issueTitle = "Invalid Message Security Mode";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static readonly double _severity = 6.5;

        public SecurityModeInvalidPlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) {}

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for Message Security Mode Invalid");

            IEnumerable<Endpoint> invalidSecurityModeEndpoints = target.GetEndpointsBySecurityMode(MessageSecurityMode.Invalid);

            foreach (Endpoint endpoint in invalidSecurityModeEndpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has invalid security mode");
                endpoint.Issues.Add(CreateIssue());
            }

            return target;
        }

    }
}