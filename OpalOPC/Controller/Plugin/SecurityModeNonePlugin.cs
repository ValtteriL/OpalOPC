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

        private static readonly PluginId _pluginId = PluginId.SecurityModeNone;
        private static readonly string _category = PluginCategories.TransportSecurity;
        private static readonly string _issueTitle = "Message Security Mode None";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:N
        private static readonly double _severity = 6.5;

        public SecurityModeNonePlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) { }

        public override (Issue?, ICollection<ISession>) Run(Endpoint endpoint)
        {
            _logger.LogTrace($"Testing {endpoint.EndpointUrl} for Message Security Mode None");

            List<ISession> sessions = new();

            if (endpoint.SecurityMode == MessageSecurityMode.None)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has security mode None");
                return (CreateIssue(), sessions);
            }

            return (null, sessions);
        }

    }
}
