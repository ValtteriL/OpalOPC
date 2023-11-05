using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
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

        public CommonCredentialsPlugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _connectionUtil = new ConnectionUtil();
        }

        public CommonCredentialsPlugin(ILogger logger, IConnectionUtil connectionUtil) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _connectionUtil = connectionUtil;
        }



        public override (Issue?, ICollection<ISession>) Run(Endpoint endpoint)
        {
            _logger.LogTrace("{Message}", $"Testing {endpoint.EndpointUrl} for common credentials");

            List<ISession> sessions = new();

            if (!IsBruteable(endpoint))
            {
                return (null, sessions);
            }

            List<(string username, string password)> validCredentials = new();

            foreach ((string username, string password) in Util.Credentials.CommonCredentials)
            {
                ISession? session = IdentityCanLogin(endpoint.EndpointDescription, new UserIdentity(username, password));

                if (session != null && session.Connected)
                {
                    _logger.LogTrace("{Message}", $"Endpoint {endpoint.EndpointUrl} uses common credentials ({username}:{password})");
                    sessions.Add(session);
                    validCredentials.Add((username, password));
                }
            }

            if (validCredentials.Any())
            {
                IEnumerable<string> credpairs = validCredentials.Select(c => $"{c.username}:{c.password}");
                s_issueTitle = $"Common credentials in use ({string.Join(", ", credpairs)})";
                return (new CommonCredentialsIssue((int)s_pluginId, s_issueTitle, s_severity, validCredentials), sessions);
            }

            return (null, sessions);
        }

        // Check if endpoint is bruteable = username + application authentication is disabled OR self-signed certificates accepted
        private bool IsBruteable(Endpoint endpoint)
        {
            return endpoint.UserTokenTypes.Contains(UserTokenType.UserName)
                && (endpoint.SecurityMode == MessageSecurityMode.None
                    || SelfSignedCertificatePlugin.SelfSignedCertAccepted(endpoint.EndpointDescription, _connectionUtil).Result);
        }

        private ISession? IdentityCanLogin(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            try
            {
                ISession session = _connectionUtil.StartSession(endpointDescription, userIdentity).Result;
                return session;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
