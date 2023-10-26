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
        private static readonly PluginId _pluginId = PluginId.SelfSignedCertificate;
        private static readonly string _category = PluginCategories.Authentication;
        private static readonly string _issueTitle = "Self signed client certificates trusted";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:L/A:N
        private static readonly double _severity = 5.4;

        public SelfSignedCertificatePlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) { }

        public override (Issue?, ICollection<ISession>) Run(Endpoint endpoint)
        {
            _logger.LogTrace($"Testing if {endpoint} accepts self signed certificate");

            List<ISession> sessions = new();

            if (SelfSignedCertAccepted(endpoint.EndpointDescription).Result)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} accepts self-signed client certificates");
                return (CreateIssue(), sessions);
            }

            return (null, sessions);
        }

        public static async Task<bool> SelfSignedCertAccepted(EndpointDescription endpointDescription)
        {
            try
            {
                ConnectionUtil util = new();
                ISession session = await util.StartSession(endpointDescription, new UserIdentity());
            }
            catch (ServiceResultException)
            {
                return false;
            }
            return true;
        }

    }
}