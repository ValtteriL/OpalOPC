using Model;
using Opc.Ua;
using Opc.Ua.Client;

namespace Util
{
    public interface IConnectionUtil
    {
        public Task<ISecurityTestSession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity);
        public Task<ISecurityTestSession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity, CertificateIdentifier identifier);
        public ISecurityTestSession? AttemptLogin(Endpoint endpoint, UserIdentity identity, CertificateIdentifier certificateIdentifier);
        public ISecurityTestSession? AttemptLogin(Endpoint endpoint, UserIdentity identity);
    }

    public class ConnectionUtil : IConnectionUtil
    {
        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly CertificateIdentifier _certificateIdentifier;
        private readonly SelfSignedCertificateUtil _selfSignedCertificateUtil;
        private readonly string _sessionName = "OpalOPC Security Check";

        public ConnectionUtil()
        {
            // Generate self-signed certificate for client
            _selfSignedCertificateUtil = new SelfSignedCertificateUtil();
            _certificateIdentifier = _selfSignedCertificateUtil.GetCertificate();

            _applicationConfiguration = new ApplicationConfiguration
            {
                ApplicationName = "OpalOPC@host",
                ApplicationUri = "urn:OPCUA:OpalOPC",
                ApplicationType = ApplicationType.Server,
                ClientConfiguration = new ClientConfiguration(),
                SecurityConfiguration = new SecurityConfiguration(),

                // accept any server certificates
                CertificateValidator = new CertificateValidator
                {
                    AutoAcceptUntrustedCertificates = true
                }
            };

        }

        // Authenticate with OPC UA server and start a session
        public async Task<ISecurityTestSession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            SessionCredential credential = new(userIdentity, _certificateIdentifier, true);
            return await StartSession(endpointDescription, credential);
        }

        public async Task<ISecurityTestSession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity, CertificateIdentifier identifier)
        {
            SessionCredential credential = new(userIdentity, identifier);
            return await StartSession(endpointDescription, credential);
        }

        private async Task<ISecurityTestSession> StartSession(EndpointDescription endpointDescription, SessionCredential sessionCredential)
        {
            // Prepare application and endpoint configurations
            _applicationConfiguration.SecurityConfiguration.ApplicationCertificate = sessionCredential.applicationCertificate;
            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(_applicationConfiguration);

            ConfiguredEndpoint endpoint = new(null, endpointDescription, endpointConfiguration);

            DefaultSessionFactory sessionFactory = new();

            ISession session = await sessionFactory.CreateAsync(
                _applicationConfiguration,
                endpoint,
                false,
                false,
                _sessionName,
                30 * 1000,
                sessionCredential.identity,
                null
            ).ConfigureAwait(false);

            return new SecurityTestSession(session, sessionCredential);
        }

        public ISecurityTestSession? AttemptLogin(Endpoint endpoint, UserIdentity identity, CertificateIdentifier? certificateIdentifier = null)
        {
            try
            {
                ISecurityTestSession session;

                if (certificateIdentifier == null)
                {
                    session = StartSession(endpoint.EndpointDescription, identity).Result;
                }
                else
                {
                    session = StartSession(endpoint.EndpointDescription, identity, certificateIdentifier).Result;
                }

                if (session != null && session.Session.Connected)
                {
                    return session;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public ISecurityTestSession? AttemptLogin(Endpoint endpoint, UserIdentity identity) => AttemptLogin(endpoint, identity, null);
    }
}
