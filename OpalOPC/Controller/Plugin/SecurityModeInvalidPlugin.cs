using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityModeInvalidPlugin : PreAuthPlugin
    {
        // "The SecurityMode should be ′Sign′ or ′SignAndEncrypt′."
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/

        private static readonly PluginId _pluginId = PluginId.SecurityModeInvalid;
        private static readonly string _category = PluginCategories.TransportSecurity;
        private static readonly string _issueTitle = "Invalid Message Security Mode";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static readonly double _severity = 6.5;

        public SecurityModeInvalidPlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) {}

        public override Issue? Run(Endpoint endpoint)
        {
            _logger.LogTrace($"Testing {endpoint} for Message Security Mode Invalid");

            if (endpoint.SecurityMode == MessageSecurityMode.Invalid)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has invalid security mode");
                return CreateIssue();
            }

            return null;
        }

    }
}