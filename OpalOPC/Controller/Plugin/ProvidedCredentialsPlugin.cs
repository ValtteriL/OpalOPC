using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Util;

namespace Plugin
{
    public class ProvidedCredentialsPlugin : PreAuthPlugin
    {
        private static readonly PluginId s_pluginId = PluginId.ProvidedCredentials;
        private static readonly string s_category = PluginCategories.Authentication;
        private static string s_issueTitle = "Provided credentials";

        // Info
        private static readonly double s_severity = 0;

        private readonly IConnectionUtil _connectionUtil;
        private readonly AuthenticationData _authenticationData;

        public ProvidedCredentialsPlugin(ILogger logger, AuthenticationData authenticationData) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _connectionUtil = new ConnectionUtil();
            _authenticationData = authenticationData;
        }

        public ProvidedCredentialsPlugin(ILogger logger, IConnectionUtil connectionUtil, AuthenticationData authenticationData) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _connectionUtil = connectionUtil;
            _authenticationData = authenticationData;
        }



        public override (Issue?, ICollection<ISecurityTestSession>) Run(Endpoint endpoint)
        {
            _logger.LogTrace("{Message}", $"Trying to authenticate to {endpoint.EndpointUrl} with provided user credentials");

            List<ISecurityTestSession> sessions = new();
            List<(string username, string password)> validUsernamePasswords = new();
            List<CertificateIdentifier> validCertificates = new();

            if (IsBruteable(endpoint))
            {
                AttempLoginWithUsernamesPasswords(sessions, validUsernamePasswords, endpoint);
            }
            AttempLoginWithUserCertificates(sessions, validCertificates, endpoint);

            if (!validUsernamePasswords.Any() && !validCertificates.Any())
            {
                // no valid credentials found, try again with different application certificates
                foreach (CertificateIdentifier applicationCertificate in _authenticationData.applicationCertificates)
                {
                    if (IsBruteable(endpoint))
                    {
                        AttempLoginWithUsernamesPasswords(sessions, validUsernamePasswords, endpoint, applicationCertificate);
                    }

                    AttempLoginWithUserCertificates(sessions, validCertificates, endpoint, applicationCertificate);
                }
            }

            if (validUsernamePasswords.Any() || validCertificates.Any())
            {
                IEnumerable<string> credpairs = validUsernamePasswords.Select(c => $"{c.username}:{c.password}").Concat(validCertificates.Select(c => $"certificate:{c.Thumbprint}"));
                s_issueTitle = $"Provided credentials in use ({string.Join(", ", credpairs)})";
                return (new CredentialsIssue((int)s_pluginId, s_issueTitle, s_severity, validUsernamePasswords, validCertificates), sessions);
            }

            return (null, sessions);
        }

        // Check if endpoint is bruteable = username + application authentication is disabled OR self-signed certificates accepted
        // we can only test if username authentication is enabled - we can't test if self-signed certificates are accepted
        // this means that we may try to brute a non-bruteable endpoint, but we will not miss any bruteable endpoints
        private static bool IsBruteable(Endpoint endpoint)
        {
            return endpoint.UserTokenTypes.Contains(UserTokenType.UserName);
        }

        private void AttempLoginWithUserCertificates(List<ISecurityTestSession> sessions, List<CertificateIdentifier> validCertificates, Endpoint endpoint, CertificateIdentifier? certificateIdentifier = null)
        {
            foreach (CertificateIdentifier userCertificate in _authenticationData.userCertificates)
            {
                ISecurityTestSession? session = AttemptLogin(endpoint, new UserIdentity(userCertificate), certificateIdentifier);
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
            foreach ((string username, string password) in _authenticationData.loginCredentials)
            {
                ISecurityTestSession? session = AttemptLogin(endpoint, new UserIdentity(username, password), certificateIdentifier);
                if (session != null && session.Session.Connected)
                {
                    _logger.LogTrace("{Message}", $"Endpoint {endpoint.EndpointUrl} uses provided username:password ({username}:{password})");
                    sessions.Add(session);
                    validUsernamePasswords.Add((username, password));
                }
            }
        }

        private ISecurityTestSession? AttemptLogin(Endpoint endpoint, UserIdentity identity, CertificateIdentifier? certificateIdentifier = null)
        {
            try
            {
                ISecurityTestSession session;

                if (certificateIdentifier == null)
                {
                    session = _connectionUtil.StartSession(endpoint.EndpointDescription, identity).Result;
                }
                else
                {
                    session = _connectionUtil.StartSession(endpoint.EndpointDescription, identity, certificateIdentifier).Result;
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

    }
}
