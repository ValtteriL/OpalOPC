using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Security.Certificates;

namespace Util
{
    public interface IConnectionUtil
    {
        public Task<ISession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity);
        public Task<ISession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity, CertificateIdentifier identifier);
    }

    public class ConnectionUtil : IConnectionUtil
    {
        private const string Subject = "CN=Test Cert Subject, C=FI, S=Uusimaa, O=Molemmat Oy";

        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly CertificateIdentifier _certificateIdentifier;

        public ConnectionUtil()
        {
            _applicationConfiguration = new ApplicationConfiguration
            {
                ApplicationName = "OpalOPC@host",
                ApplicationUri = "urn:host:OPCUA:OpalOPC",
                ApplicationType = ApplicationType.Server,
                ClientConfiguration = new ClientConfiguration(),
                SecurityConfiguration = new SecurityConfiguration()
            };

            // Generate self-signed certificate for client
            _certificateIdentifier = new CertificateIdentifier(
                CertificateBuilder
                    .Create(Subject)
                    .AddExtension(
                        new X509SubjectAltNameExtension("urn:opalopc.com:host",
                        new string[] { "host", "host.opalopc.com", "192.168.1.100" }))
                    .CreateForRSA());

            // accept any server certificates
            _applicationConfiguration.CertificateValidator = new CertificateValidator
            {
                AutoAcceptUntrustedCertificates = true
            };

        }

        // Authenticate with OPC UA server and start a session
        public async Task<ISession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            return await StartSession(endpointDescription, userIdentity, _certificateIdentifier);
        }

        public async Task<ISession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity, CertificateIdentifier identifier)
        {

            // Prepare application and endpoint configurations
            _applicationConfiguration.SecurityConfiguration.ApplicationCertificate = identifier;
            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(_applicationConfiguration);

            ConfiguredEndpoint endpoint = new(null, endpointDescription, endpointConfiguration);

            ISessionFactory sessionFactory = new DefaultSessionFactory();

            ISession session = await sessionFactory.CreateAsync(
                _applicationConfiguration,
                endpoint,
                false,
                false,
                _applicationConfiguration.ApplicationName,
                30 * 1000,
                userIdentity,
                null
            ).ConfigureAwait(false);

            return session;
        }
    }
}
