using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Plugin
{
    public class BruteForcePlugin : PreAuthPlugin
    {
        private static readonly PluginId s_pluginId = PluginId.BruteForce;
        private static readonly string s_category = PluginCategories.Authentication;
        private static string s_issueTitle = "Credential brute force successful";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:L
        private static readonly double s_severity = 7.3;

        private readonly IConnectionUtil _connectionUtil;
        private readonly AuthenticationData _authenticationData;

        public BruteForcePlugin(ILogger logger, AuthenticationData authenticationData) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _connectionUtil = new ConnectionUtil();
            _authenticationData = authenticationData;
        }

        public BruteForcePlugin(ILogger logger, IConnectionUtil connectionUtil, AuthenticationData authenticationData) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _connectionUtil = connectionUtil;
            _authenticationData = authenticationData;
        }

        public override (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions)
        {
            _logger.LogTrace("{Message}", $"Brute forcing credentials for {discoveryUrl}");

            List<ISecurityTestSession> sessions = [];

            List<EndpointDescription> usernameEndpoints = endpointDescriptions.FindAll(e => e.UserIdentityTokens.Any(t => t.TokenType == UserTokenType.UserName));
            EndpointDescription? usernameEndpointsNoApplicationAuthentication = usernameEndpoints.Find(e => e.SecurityPolicyUri == SecurityPolicies.None);
            EndpointDescription? usernameEndpointWithApplicationAuthentication = usernameEndpoints.Find(e => e.SecurityPolicyUri != SecurityPolicies.None);

            List<(string username, string password)> validUsernamePasswords = [];

            EndpointDescription? endpointToTryWithoutOrWithSelfSignedAppCertificate = usernameEndpointsNoApplicationAuthentication ?? usernameEndpointWithApplicationAuthentication;

            if (endpointToTryWithoutOrWithSelfSignedAppCertificate != null)
            {
                AttempLoginWithUsernamesPasswords(sessions, validUsernamePasswords, new Endpoint(endpointToTryWithoutOrWithSelfSignedAppCertificate));
            }

            if (validUsernamePasswords.Count == 0 && usernameEndpointWithApplicationAuthentication != null)
            {
                // no valid credentials found, try again with different application certificates
                foreach (CertificateIdentifier applicationCertificate in _authenticationData.applicationCertificates)
                {
                    AttempLoginWithUsernamesPasswords(sessions, validUsernamePasswords, new Endpoint(usernameEndpointWithApplicationAuthentication), applicationCertificate);
                }
            }

            if (validUsernamePasswords.Count != 0)
            {
                IEnumerable<string> credpairs = validUsernamePasswords.Select(c => $"{c.username}:{c.password}");
                s_issueTitle = $"Brute forced credentials in use ({string.Join(", ", credpairs)})";
                return (new CredentialsIssue(s_pluginId, s_issueTitle, s_severity, validUsernamePasswords), sessions);
            }

            return (null, sessions);
        }

        private void AttempLoginWithUsernamesPasswords(List<ISecurityTestSession> sessions, List<(string, string)> validUsernamePasswords, Endpoint endpoint, CertificateIdentifier? certificateIdentifier = null)
        {
            foreach ((string username, string password) in _authenticationData.bruteForceCredentials)
            {
                ISecurityTestSession? session;

                if (certificateIdentifier == null)
                    session = _connectionUtil.AttemptLogin(endpoint, new UserIdentity(username, password));
                else
                    session = _connectionUtil.AttemptLogin(endpoint, new UserIdentity(username, password), certificateIdentifier);

                if (session != null && session.Session.Connected)
                {
                    _logger.LogTrace("{Message}", $"Successfully brute forced credentials {username}:{password} for {endpoint.EndpointUrl}");
                    sessions.Add(session);
                    validUsernamePasswords.Add((username, password));
                }
            }
        }
    }
}
