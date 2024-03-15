using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Plugin
{
    public class ProvidedCredentialsPlugin(ILogger logger, IConnectionUtil connectionUtil, AuthenticationData authenticationData) : PreAuthPlugin(logger, s_pluginId, s_category, s_issueTitle, s_severity)
    {
        private static readonly PluginId s_pluginId = PluginId.ProvidedCredentials;
        private static readonly string s_category = PluginCategories.Authentication;
        private static string s_issueTitle = "Provided credentials";

        // Info
        private static readonly double s_severity = 0;

        public override (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions)
        {
            _logger.LogTrace("{Message}", $"Testing {discoveryUrl} for provided credentials");

            List<ISecurityTestSession> sessions = [];
            List<(string username, string password)> validUsernamePasswords = [];
            List<CertificateIdentifier> validCertificates = [];

            List<EndpointDescription> usernameEndpoints = endpointDescriptions.FindAll(d => d.UserIdentityTokens.Any(t => t.TokenType == UserTokenType.UserName));
            EndpointDescription? usernameEndpointNoApplicationAuthentication = usernameEndpoints.Find(e => e.SecurityPolicyUri == SecurityPolicies.None);
            EndpointDescription? usernameEndpointWithApplicationAuthentication = usernameEndpoints.Find(e => e.SecurityPolicyUri != SecurityPolicies.None);
            List<EndpointDescription> certificateEndpoints = endpointDescriptions.FindAll(e => e.UserIdentityTokens.Any(t => t.TokenType == UserTokenType.Certificate));
            EndpointDescription? certificateEndpointNoApplicationAuthentication = certificateEndpoints.Find(e => e.SecurityPolicyUri == SecurityPolicies.None);
            EndpointDescription? certificateEndpointWithApplicationAuthentication = certificateEndpoints.Find(e => e.SecurityPolicyUri != SecurityPolicies.None);

            EndpointDescription? usernameEndpointToTryWithoutOrWithSelfSignedAppCertificate = usernameEndpointNoApplicationAuthentication ?? usernameEndpointWithApplicationAuthentication;
            EndpointDescription? certificateEndpointToTryWithoutOrWithSelfSignedAppCertificate = certificateEndpointNoApplicationAuthentication ?? certificateEndpointWithApplicationAuthentication;

            if (usernameEndpointToTryWithoutOrWithSelfSignedAppCertificate != null)
            {
                AttempLoginWithUsernamesPasswords(sessions, validUsernamePasswords, new Endpoint(usernameEndpointToTryWithoutOrWithSelfSignedAppCertificate));
            }
            if (certificateEndpointToTryWithoutOrWithSelfSignedAppCertificate != null)
            {
                AttempLoginWithUserCertificates(sessions, validCertificates, new Endpoint(certificateEndpointToTryWithoutOrWithSelfSignedAppCertificate));
            }

            if (validUsernamePasswords.Count == 0 && validCertificates.Count == 0)
            {
                // no valid credentials found, try again with different application certificates
                foreach (CertificateIdentifier applicationCertificate in authenticationData.applicationCertificates)
                {
                    if (usernameEndpointWithApplicationAuthentication != null)
                    {
                        AttempLoginWithUsernamesPasswords(sessions, validUsernamePasswords, new Endpoint(usernameEndpointWithApplicationAuthentication), applicationCertificate);
                    }
                    if (certificateEndpointWithApplicationAuthentication != null)
                    {
                        AttempLoginWithUserCertificates(sessions, validCertificates, new Endpoint(certificateEndpointWithApplicationAuthentication), applicationCertificate);
                    }
                }
            }

            if (validUsernamePasswords.Count != 0 || validCertificates.Count != 0)
            {
                IEnumerable<string> credpairs = validUsernamePasswords.Select(c => $"{c.username}:{c.password}").Concat(validCertificates.Select(c => $"certificate:{c.Thumbprint}"));
                s_issueTitle = $"Provided credentials in use ({string.Join(", ", credpairs)})";
                return (new CredentialsIssue(s_pluginId, s_issueTitle, s_severity, validUsernamePasswords, validCertificates), sessions);
            }

            return (null, sessions);
        }

        private void AttempLoginWithUserCertificates(List<ISecurityTestSession> sessions, List<CertificateIdentifier> validCertificates, Endpoint endpoint, CertificateIdentifier? certificateIdentifier = null)
        {
            foreach (CertificateIdentifier userCertificate in authenticationData.userCertificates)
            {
                ISecurityTestSession? session;
                if (certificateIdentifier == null)
                    session = connectionUtil.AttemptLogin(endpoint, new UserIdentity(userCertificate));
                else
                    session = connectionUtil.AttemptLogin(endpoint, new UserIdentity(userCertificate), certificateIdentifier);

                if (session != null && session.Session.Connected)
                {
                    _logger.LogTrace("{Message}", $"Endpoint {endpoint.EndpointUrl} uses provided user certificate ({userCertificate.Thumbprint})");
                    sessions.Add(session);
                    validCertificates.Add(userCertificate);
                }
            }
        }

        private void AttempLoginWithUsernamesPasswords(List<ISecurityTestSession> sessions, List<(string, string)> validUsernamePasswords, Endpoint endpoint, CertificateIdentifier? certificateIdentifier = null)
        {
            foreach ((string username, string password) in authenticationData.loginCredentials)
            {
                ISecurityTestSession? session;
                if (certificateIdentifier == null)
                    session = connectionUtil.AttemptLogin(endpoint, new UserIdentity(username, password));
                else
                    session = connectionUtil.AttemptLogin(endpoint, new UserIdentity(username, password), certificateIdentifier);

                if (session != null && session.Session.Connected)
                {
                    _logger.LogTrace("{Message}", $"Endpoint {endpoint.EndpointUrl} uses provided username:password ({username}:{password})");
                    sessions.Add(session);
                    validUsernamePasswords.Add((username, password));
                }
            }
        }
    }
}
