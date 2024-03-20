using System.Security.Cryptography.X509Certificates;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Security.Certificates;

namespace Util
{
    public interface IConnectionUtil
    {
        public Task<ISecurityTestSession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity);
        public Task<ISecurityTestSession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity, CertificateIdentifier identifier);
        public ISecurityTestSession? AttemptLogin(Endpoint endpoint, UserIdentity identity, CertificateIdentifier certificateIdentifier);
        public ISecurityTestSession? AttemptLogin(Endpoint endpoint, UserIdentity identity);
    }

    public class ConnectionUtil(ISelfSignedCertificateUtil selfSignedCertificateUtil) : IConnectionUtil
    {
        private readonly CertificateIdentifier _selfSignedCertificate = selfSignedCertificateUtil.GetCertificate(); // Generate self-signed certificate for client
        private readonly string _sessionName = "OpalOPC Security Check";

        private class RelaxedCertificateValidator : CertificateValidator
        {
            public override void Validate(X509Certificate2Collection certificateChain) { return; }
            public override void Validate(X509Certificate2Collection chain, ConfiguredEndpoint endpoint) { return; }
            protected override Task InternalValidate(X509Certificate2Collection certificates, ConfiguredEndpoint endpoint) { return Task.CompletedTask; }
        }

        // Authenticate with OPC UA server and start a session
        public async Task<ISecurityTestSession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            SessionCredential credential = new(userIdentity, _selfSignedCertificate, true);
            return await StartSession(endpointDescription, credential);
        }

        public async Task<ISecurityTestSession> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity, CertificateIdentifier identifier)
        {
            SessionCredential credential = new(userIdentity, identifier);
            return await StartSession(endpointDescription, credential);
        }

        private async Task<ISecurityTestSession> StartSession(EndpointDescription endpointDescription, SessionCredential sessionCredential)
        {
            ApplicationConfiguration applicationConfiguration = buildApplicationConfiguration(sessionCredential.applicationCertificate);

            // Prepare application and endpoint configurations
            applicationConfiguration.SecurityConfiguration.ApplicationCertificate = sessionCredential.applicationCertificate;
            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(applicationConfiguration);

            ConfiguredEndpoint endpoint = new(null, endpointDescription, endpointConfiguration);

            DefaultSessionFactory sessionFactory = new();

            ISession session = await sessionFactory.CreateAsync(
                applicationConfiguration,
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

        private static ApplicationConfiguration buildApplicationConfiguration(CertificateIdentifier applicationCertificate)
        {
            return new ApplicationConfiguration
            {
                ApplicationName = SelfSignedCertificateUtil.s_applicationName,
                ApplicationUri = applicationCertificate.Certificate.FindExtension<X509SubjectAltNameExtension>().Uris.FirstOrDefault(SelfSignedCertificateUtil.s_applicationUri),
                ApplicationType = ApplicationType.Server,
                ClientConfiguration = new ClientConfiguration(),
                SecurityConfiguration = new SecurityConfiguration()
                {
                    ApplicationCertificate = applicationCertificate
                },

                // accept any server certificates
                CertificateValidator = new RelaxedCertificateValidator
                {
                    AutoAcceptUntrustedCertificates = true
                }
            };
        }
    }
}
