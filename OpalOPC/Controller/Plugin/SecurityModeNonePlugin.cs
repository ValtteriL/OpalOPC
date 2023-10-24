using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityModeNonePlugin : Plugin
    {
        // "The SecurityMode should be ′Sign′ or ′SignAndEncrypt′."
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/

        private static readonly PluginId _pluginId = PluginId.SecurityModeNone;
        private static readonly string _category = PluginCategories.TransportSecurity;
        private static readonly string _issueTitle = "Message Security Mode None";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static readonly double _severity = 6.5;

        public SecurityModeNonePlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) {}

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for Message Security Mode None");

            IEnumerable<Endpoint> NoneSecurityModeEndpoints = target.GetEndpointsBySecurityMode(MessageSecurityMode.None);

            foreach (Endpoint endpoint in NoneSecurityModeEndpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has security mode None");
                endpoint.Issues.Add(CreateIssue());
            }

            return target;
        }

    }
}