using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Util;

namespace Plugin
{
    public class SelfSignedCertificatePlugin : PreAuthPlugin
    {
        // "self-signed certificates should not be trusted automatically"
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
        // check if self-signed certificates are accepted
        private static readonly PluginId s_pluginId = PluginId.SelfSignedCertificate;
        private static readonly string s_category = PluginCategories.Authentication;
        private static readonly string s_issueTitle = "Self signed client certificates trusted";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:L/A:N
        private static readonly double s_severity = 5.4;

        private readonly IConnectionUtil _connectionUtil;

        public SelfSignedCertificatePlugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _connectionUtil = new ConnectionUtil();
        }
        public SelfSignedCertificatePlugin(ILogger logger, IConnectionUtil connectionUtil) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _connectionUtil = connectionUtil;
        }

        public override (Issue?, ICollection<ISession>) Run(Endpoint endpoint)
        {
            _logger.LogTrace("{Message}", $"Testing if {endpoint} accepts self signed certificate");

            List<ISession> sessions = new();

            if (SelfSignedCertAccepted(endpoint.EndpointDescription, _connectionUtil).Result)
            {
                _logger.LogTrace("{Message}", $"Endpoint {endpoint.EndpointUrl} accepts self-signed client certificates");
                return (CreateIssue(), sessions);
            }

            return (null, sessions);
        }

        public static async Task<bool> SelfSignedCertAccepted(EndpointDescription endpointDescription, IConnectionUtil connectionUtil)
        {
            try
            {
                ISession session = await connectionUtil.StartSession(endpointDescription, new UserIdentity());
            }
            catch (ServiceResultException)
            {
                return false;
            }
            return true;
        }

    }
}
