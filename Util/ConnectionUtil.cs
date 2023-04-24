using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua;
using Opc.Ua.Security.Certificates;

namespace Util
{
    public class ConnectionUtil
    {
        private const string Subject = "CN=Test Cert Subject, C=US, S=Arizona, O=OPC Foundation";

        private ApplicationConfiguration applicationConfiguration;
        private EndpointConfiguration endpointConfiguration;

        public ConnectionUtil()
        {
            applicationConfiguration = new ApplicationConfiguration();
            applicationConfiguration.ApplicationName = "";
            applicationConfiguration.ApplicationUri = "";
            applicationConfiguration.ApplicationType = ApplicationType.Server;
            applicationConfiguration.ClientConfiguration = new ClientConfiguration();
            applicationConfiguration.SecurityConfiguration = new SecurityConfiguration();

            // Use self-signed certificate to connect
            applicationConfiguration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier(CertificateBuilder.Create(Subject).CreateForRSA());

            // accept any server certificates
            applicationConfiguration.CertificateValidator = new CertificateValidator();
            applicationConfiguration.CertificateValidator.AutoAcceptUntrustedCertificates = true;

            endpointConfiguration = EndpointConfiguration.Create(applicationConfiguration);
        }

        // Authenticate with OPC UA server and start a session
        public async Task<Opc.Ua.Client.Session> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

            var session = await Opc.Ua.Client.Session.Create(
                this.applicationConfiguration,
                endpoint,
                false,
                false,
                this.applicationConfiguration.ApplicationName,
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