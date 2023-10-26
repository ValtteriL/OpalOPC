using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Util;

namespace Plugin
{
    public class CommonCredentialsPlugin : PreAuthPlugin
    {
        private static readonly PluginId _pluginId = PluginId.CommonCredentials;
        private static readonly string _category = PluginCategories.Authentication;
        private static string _issueTitle = "Common credentials in use";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:L
        private static readonly double _severity = 7.3;

        public CommonCredentialsPlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) { }

        public override (Issue?, ICollection<ISession>) Run(Endpoint endpoint)
        {
            _logger.LogTrace($"Testing {endpoint} for common credentials");

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
                    _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} uses common credentials ({username}:{password})");
                    validCredentials.Add((username, password));
                }
            }

            if (validCredentials.Any())
            {
                IEnumerable<string> credpairs = validCredentials.Select(c => $"{c.username}:{c.password}");
                _issueTitle = $"Common credentials in use ({string.Join(", ", credpairs)})";
                return (new CommonCredentialsIssue((int)_pluginId, _issueTitle, _severity, validCredentials), sessions);
            }

            return (null, sessions);
        }

        // Check if endpoint is bruteable = username + application authentication is disabled OR self-signed certificates accepted
        private static bool IsBruteable(Endpoint endpoint)
        {
            return endpoint.UserTokenTypes.Contains(UserTokenType.UserName)
                && (endpoint.SecurityMode == MessageSecurityMode.None
                    || SelfSignedCertificatePlugin.SelfSignedCertAccepted(endpoint.EndpointDescription).Result);
        }

        private static ISession? IdentityCanLogin(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            try
            {
                ConnectionUtil util = new();
                ISession session = util.StartSession(endpointDescription, userIdentity).Result;
                return session;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}