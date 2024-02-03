using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class ServerCertificatePlugin : PreAuthPlugin
    {
        private static readonly PluginId s_pluginId = PluginId.ServerCertificate;
        private static readonly string s_category = PluginCategories.Reconnaissance;
        private static readonly string s_issueTitle = "Server Certificate";
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        private static readonly double s_severity = 0;

        public ServerCertificatePlugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
        }

        public override (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions)
        {
            _logger.LogTrace("{Message}", $"Checking Server Certificate on {discoveryUrl}");

            // create set of X509Certificate2 objects
            HashSet<X509Certificate2> certificates = [];

            foreach (EndpointDescription endpointDescription in endpointDescriptions)
            {
                // parse the certificate
                certificates.Add(new X509Certificate2(endpointDescription.ServerCertificate));
            }

            // create list of ServerCertificateJson objects
            List<ServerCertificateJson> serverCertificateJsons = new();

            foreach (X509Certificate2 certificate in certificates)
            {
                try
                {
                    ServerCertificateJson serverCertificateJson = new(certificate);
                    serverCertificateJsons.Add(serverCertificateJson);
                }
                catch (CryptographicException)
                {
                    continue;
                }
            }

            if (serverCertificateJsons.Count == 0)
            {
                return (null, new List<ISecurityTestSession>());
            }

            Issue issue = CreateIssue(serverCertificateJsons);
            return (issue, new List<ISecurityTestSession>());
        }

        private Issue CreateIssue(List<ServerCertificateJson> serverCertificateJsons)
        {
            // create empty json object
            string issueDescription = $"{s_issueTitle}s: {JsonSerializer.Serialize(
                serverCertificateJsons,
                _jsonSerializerOptions)}";
            return new Issue((int)s_pluginId, issueDescription, _severity);
        }

        private class ServerCertificateJson(X509Certificate2 certificate)
        {
            public string Subject { get; set; } = certificate.Subject;
            public string SubjectName { get; set; } = certificate.SubjectName.Name;
            public string Issuer { get; set; } = certificate.Issuer;
            public string IssuerName { get; set; } = certificate.IssuerName.Name;
            public DateTime NotBefore { get; set; } = certificate.NotBefore;
            public DateTime NotAfter { get; set; } = certificate.NotAfter;
            public string SerialNumber { get; set; } = certificate.SerialNumber;
            public string Thumbprint { get; set; } = certificate.Thumbprint;
        }
    }
}
