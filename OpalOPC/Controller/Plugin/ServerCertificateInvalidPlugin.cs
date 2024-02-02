using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Plugin;

namespace Plugin
{
    public class ServerCertificateInvalidPlugin : PreAuthPlugin
    {
        private static readonly PluginId s_pluginId = PluginId.ServerCertificateExpired;
        private static readonly string s_category = PluginCategories.TransportSecurity;
        private static readonly string s_issueTitle = "Server certificate is invalid";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:A/AC:H/PR:N/UI:R/S:U/C:L/I:L/A:N
        private static readonly double s_severity = 3.7;

        public ServerCertificateInvalidPlugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
        }

        public override (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions)
        {
            foreach (EndpointDescription endpointDescription in endpointDescriptions)
            {
                // parse the certificate
                X509Certificate2 certificate = new(endpointDescription.ServerCertificate);
                if(!certificate.Verify())
                {
                    return (CreateIssue(), new List<ISecurityTestSession>());
                }

            }

            return (null, new List<ISecurityTestSession>());
        }
    }
}
