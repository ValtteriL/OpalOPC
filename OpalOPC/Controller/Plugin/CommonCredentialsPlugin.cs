using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Plugin
{
    public class CommonCredentialsPlugin : PreAuthPlugin
    {
        private static readonly PluginId s_pluginId = PluginId.CommonCredentials;
        private static readonly string s_category = PluginCategories.Authentication;
        private static string s_issueTitle = "Common credentials in use";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:L
        private static readonly double s_severity = 7.3;

        private readonly IConnectionUtil _connectionUtil;
        private readonly AuthenticationData _authenticationData;

        public CommonCredentialsPlugin(ILogger logger, AuthenticationData authenticationData) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _connectionUtil = new ConnectionUtil();
            _authenticationData = authenticationData;
        }

        public CommonCredentialsPlugin(ILogger logger, IConnectionUtil connectionUtil, AuthenticationData authenticationData) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _connectionUtil = connectionUtil;
            _authenticationData = authenticationData;
        }

        public override (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions)
        {
            _logger.LogTrace("{Message}", $"Testing {discoveryUrl} for common credentials");

            List<ISecurityTestSession> sessions = new();

            List<EndpointDescription> usernameEndpoints = endpointDescriptions.FindAll(e => e.UserIdentityTokens.Any(t => t.TokenType == UserTokenType.UserName));
            EndpointDescription? usernameEndpointsNoApplicationAuthentication = usernameEndpoints.Find(e => e.SecurityPolicyUri == SecurityPolicies.None);
            EndpointDescription? usernameEndpointWithApplicationAuthentication = usernameEndpoints.Find(e => e.SecurityPolicyUri != SecurityPolicies.None);

            List<(string username, string password)> validCredentials = new();

            if (usernameEndpointsNoApplicationAuthentication != null)
            {
                AttempLoginWithUsernamesPasswords(sessions, validCredentials, new Endpoint(usernameEndpointsNoApplicationAuthentication));
            }
            else if (usernameEndpointWithApplicationAuthentication != null)
            {
                foreach (CertificateIdentifier applicationCertificate in _authenticationData.applicationCertificates)
                {
                    AttempLoginWithUsernamesPasswords(sessions, validCredentials, new Endpoint(usernameEndpointWithApplicationAuthentication), applicationCertificate);
                }
            }

            if (validCredentials.Any())
            {
                IEnumerable<string> credpairs = validCredentials.Select(c => $"{c.username}:{c.password}");
                s_issueTitle = $"Common credentials in use ({string.Join(", ", credpairs)})";
                return (new CredentialsIssue((int)s_pluginId, s_issueTitle, s_severity, validCredentials), sessions);
            }

            return (null, sessions);
        }

        private void AttempLoginWithUsernamesPasswords(List<ISecurityTestSession> sessions, List<(string, string)> validUsernamePasswords, Endpoint endpoint, CertificateIdentifier? certificateIdentifier = null)
        {
            foreach ((string username, string password) in Util.Credentials.CommonCredentials)
            {
                ISecurityTestSession? session;

                if (certificateIdentifier == null)
                    session = _connectionUtil.AttemptLogin(endpoint, new UserIdentity(username, password));
                else
                    session = _connectionUtil.AttemptLogin(endpoint, new UserIdentity(username, password), certificateIdentifier);

                if (session != null && session.Session.Connected)
                {
                    _logger.LogTrace("{Message}", $"Endpoint {endpoint.EndpointUrl} uses common credentials ({username}:{password})");
                    sessions.Add(session);
                    validUsernamePasswords.Add((username, password));
                }
            }
        }

    }
}
