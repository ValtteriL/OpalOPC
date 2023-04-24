using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua;
using Opc.Ua.Security.Certificates;

namespace Util
{
    public static class ConnectionUtil
    {
        private const string Subject = "CN=Test Cert Subject, C=US, S=Arizona, O=OPC Foundation";

        // Authenticate with OPC UA server and start a session
        public async static Task<Opc.Ua.Client.Session> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            ApplicationConfiguration m_configuration = new ApplicationConfiguration();
            m_configuration.ApplicationName = "";
            m_configuration.ApplicationUri = "";
            m_configuration.ApplicationType = ApplicationType.Server;
            m_configuration.ClientConfiguration = new ClientConfiguration();
            m_configuration.SecurityConfiguration = new SecurityConfiguration();

            // Use self-signed certificate to connect
            m_configuration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier(CertificateBuilder.Create(Subject).CreateForRSA());

            // accept any server certificates
            m_configuration.CertificateValidator = new CertificateValidator();
            m_configuration.CertificateValidator.AutoAcceptUntrustedCertificates = true;

            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(m_configuration);
            ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

            var session = await Opc.Ua.Client.Session.Create(
                m_configuration,
                endpoint,
                false,
                false,
                m_configuration.ApplicationName,
                30 * 1000,
                userIdentity,
                null
            ).ConfigureAwait(false);

            if (!session.Connected)
            {
                return null;
            }
            return session;
        }
    }
}