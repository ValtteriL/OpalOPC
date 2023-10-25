using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
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

        public override Issue? Run(Endpoint endpoint)
        {
            _logger.LogTrace($"Testing {endpoint} for common credentials");

            if (!IsBruteable(endpoint))
            {
                return null;
            }

            List<(string username, string password)> validCredentials = new();

            foreach ((string username, string password) in Util.Credentials.CommonCredentials)
            {

                if (IdentityCanLogin(endpoint.EndpointDescription, new UserIdentity(username, password), out NodeIdCollection roleIds))
                {
                    _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} uses common credentials ({username}:{password})");
                    _issueTitle = $"Common credentials in use ({username}:{password})";
                    validCredentials.Add((username, password));
                    return new CommonCredentialsIssue((int)_pluginId, _issueTitle, _severity, username, password);
                }
            }

            if (validCredentials.Count == 1)
            {
                (string username, string password) = validCredentials.First();
                _issueTitle = $"Common credentials in use ({username}:{password})";
                return new CommonCredentialsIssue((int)_pluginId, _issueTitle, _severity, username, password);
            }
            else if (validCredentials.Count > 1)
            {
                // TODO
                return 1;
            }

            return null;
        }

        // Check if endpoint is bruteable = username + application authentication is disabled OR self-signed certificates accepted
        private static bool IsBruteable(Endpoint endpoint)
        {
            return endpoint.UserTokenTypes.Contains(UserTokenType.UserName)
                && (endpoint.SecurityMode == MessageSecurityMode.None
                    || SelfSignedCertificatePlugin.SelfSignedCertAccepted(endpoint.EndpointDescription).Result);
        }

        private static bool IdentityCanLogin(EndpointDescription endpointDescription, UserIdentity userIdentity, out NodeIdCollection roleIds)
        {
            roleIds = new NodeIdCollection();

            try
            {
                ConnectionUtil util = new();
                using Opc.Ua.Client.ISession session = util.StartSession(endpointDescription, userIdentity).Result;
                bool result = false;
                if (session.Connected)
                {
                    result = true;
                    roleIds = session.Identity.GrantedRoleIds;
                }

                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}