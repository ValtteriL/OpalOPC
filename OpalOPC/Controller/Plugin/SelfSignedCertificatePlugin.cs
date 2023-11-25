using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Util;

namespace Plugin
{
    public class SelfSignedCertificatePlugin : SessionCredentialPlugin
    {
        // "self-signed certificates should not be trusted automatically"
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
        // check if self-signed certificates are accepted
        private static readonly PluginId s_pluginId = PluginId.SelfSignedCertificate;
        private static readonly string s_category = PluginCategories.Authentication;
        private static readonly string s_issueTitle = "Self signed client application certificates trusted";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:L/A:N
        private static readonly double s_severity = 5.4;

        public SelfSignedCertificatePlugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
        }

        public override Issue? Run(ICollection<ISecurityTestSession> securityTestSessions)
        {
            // expects there to be at least one session
            _logger.LogTrace("{Message}", $"Testing if {securityTestSessions.First().EndpointUrl} accepts self signed application certificate");

            foreach (ISecurityTestSession securityTestSession in securityTestSessions)
            {
                if (securityTestSession.Credential.selfSignedAppCert)
                {
                    _logger.LogTrace("{Message}", $"Endpoint {securityTestSession.EndpointUrl} accepts self signed application certificate");
                    return CreateIssue();
                }
            }

            return null;
        }
    }
}
