using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class SecurityModeInvalidPlugin : PreAuthPlugin
    {
        // "The SecurityMode should be ′Sign′ or ′SignAndEncrypt′."
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/

        private static readonly PluginId s_pluginId = PluginId.SecurityModeInvalid;
        private static readonly string s_category = PluginCategories.TransportSecurity;
        private static readonly string s_issueTitle = "Invalid Message Security Mode";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static readonly double s_severity = 6.5;

        public SecurityModeInvalidPlugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity) { }

        public override (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions)
        {
            _logger.LogTrace("{Message}", $"Testing {discoveryUrl} for Message Security Mode Invalid");

            List<ISecurityTestSession> sessions = [];

            if (endpointDescriptions.Find(d => d.SecurityMode == MessageSecurityMode.Invalid) != null)
            {
                _logger.LogTrace("{Message}", $"Endpoint {discoveryUrl} has invalid security mode");
                return (CreateIssue(), sessions);
            }

            return (null, sessions);
        }

    }
}
