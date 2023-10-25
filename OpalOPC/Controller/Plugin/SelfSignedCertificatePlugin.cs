using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
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

        public override Issue? Run(Endpoint endpoint)
        {
            _logger.LogTrace($"Testing if {endpoint} accepts self signed certificate");

            if (SelfSignedCertAccepted(endpoint.EndpointDescription).Result)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} accepts self-signed client certificates");
                return CreateIssue();
            }

            return null;
        }

        public static async Task<bool> SelfSignedCertAccepted(EndpointDescription endpointDescription)
        {
            try
            {
                ConnectionUtil util = new();
                using Opc.Ua.Client.ISession session = await util.StartSession(endpointDescription, new UserIdentity());
            }
            catch (Opc.Ua.ServiceResultException)
            {
                return false;
            }
            return true;
        }

    }
}