using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;

namespace Plugin
{
    public class SecurityModeNonePlugin : PreAuthPlugin
    {
        // "The SecurityMode should be ′Sign′ or ′SignAndEncrypt′."
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/

        private static readonly PluginId s_pluginId = PluginId.SecurityModeNone;
        private static readonly string s_category = PluginCategories.TransportSecurity;
        private static readonly string s_issueTitle = "Message Security Mode None";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static readonly double s_severity = 6.5;

        public SecurityModeNonePlugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity) { }

        public override (Issue?, ICollection<ISession>) Run(Endpoint endpoint)
        {
            _logger.LogTrace("{Message}", $"Testing {endpoint.EndpointUrl} for Message Security Mode None");

            List<ISession> sessions = new();

            if (endpoint.SecurityMode == MessageSecurityMode.None)
            {
                _logger.LogTrace("{Message}", $"Endpoint {endpoint.EndpointUrl} has security mode None");
                return (CreateIssue(), sessions);
            }

            return (null, sessions);
        }

    }
}
