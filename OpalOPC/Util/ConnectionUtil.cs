using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Security.Certificates;

namespace Util
{
    public interface IConnectionUtil
    {
        public Task<ISession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity);

    }

    public class ConnectionUtil : IConnectionUtil
    {
        private const string Subject = "CN=Test Cert Subject, C=FI, S=Uusimaa, O=Molemmat Oy";

        private readonly ApplicationConfiguration applicationConfiguration;
        private readonly EndpointConfiguration endpointConfiguration;

        public ConnectionUtil()
        {
            applicationConfiguration = new ApplicationConfiguration
            {
                ApplicationName = "OpalOPC@host",
                ApplicationUri = "urn:host:OPCUA:OpalOPC",
                ApplicationType = ApplicationType.Server,
                ClientConfiguration = new ClientConfiguration(),
                SecurityConfiguration = new SecurityConfiguration()
            };

            // Use self-signed certificate to connect
            applicationConfiguration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier(
                CertificateBuilder
                    .Create(Subject)
                    .AddExtension(
                        new X509SubjectAltNameExtension("urn:opalopc.com:host",
                        new string[] { "host", "host.opalopc.com", "192.168.1.100" }))
                    .CreateForRSA());

            // accept any server certificates
            applicationConfiguration.CertificateValidator = new CertificateValidator
            {
                AutoAcceptUntrustedCertificates = true
            };

            endpointConfiguration = EndpointConfiguration.Create(applicationConfiguration);
        }

        // Authenticate with OPC UA server and start a session
        public async Task<ISession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            ConfiguredEndpoint endpoint = new(null, endpointDescription, endpointConfiguration);

            ISessionFactory sessionFactory = new DefaultSessionFactory();

            using ISession session = await sessionFactory.CreateAsync(
                applicationConfiguration,
                endpoint,
                false,
                false,
                applicationConfiguration.ApplicationName,
                30 * 1000,
                userIdentity,
                null
            ).ConfigureAwait(false);

            return session;
        }
    }
}
