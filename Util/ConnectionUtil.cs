using Opc.Ua;
using Opc.Ua.Client;
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
            applicationConfiguration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier(
                CertificateBuilder
                    .Create(Subject)
                    .AddExtension(
                        new X509SubjectAltNameExtension("urn:opcfoundation.org:mypc",
                        new string[] { "mypc", "mypc.opcfoundation.org", "192.168.1.100" }))
                    .CreateForRSA());

            // accept any server certificates
            applicationConfiguration.CertificateValidator = new CertificateValidator();
            applicationConfiguration.CertificateValidator.AutoAcceptUntrustedCertificates = true;

            endpointConfiguration = EndpointConfiguration.Create(applicationConfiguration);
        }

        // Authenticate with OPC UA server and start a session
        public async Task<ISession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

            ISessionFactory sessionFactory = new DefaultSessionFactory();

            var session = await sessionFactory.CreateAsync(
                this.applicationConfiguration,
                endpoint,
                false,
                false,
                this.applicationConfiguration.ApplicationName,
                30 * 1000,
                userIdentity,
                null
            ).ConfigureAwait(false);

            return session;
        }
    }
}