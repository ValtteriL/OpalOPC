using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Plugin
{
    public class CommonCredentialsPlugin : Plugin
    {
        private static readonly PluginId _pluginId = PluginId.CommonCredentials;
        private static readonly string _category = PluginCategories.Authentication;
        private static string _issueTitle = "Common credentials in use";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:L
        private static readonly double _severity = 7.3;

        public CommonCredentialsPlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) { }

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for common credentials");

            Parallel.ForEach(target.GetBruteableEndpoints(), endpoint =>
            {
                foreach ((string username, string password) in Util.Credentials.CommonCredentials)
                {
                    if (IdentityCanLogin(endpoint.EndpointDescription, new UserIdentity(username, password), out NodeIdCollection roleIds))
                    {
                        _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} uses common credentials ({username}:{password})");
                        _issueTitle = $"Common credentials in use ({username}:{password})";
                        endpoint.Issues.Add(new CommonCredentialsIssue((int)_pluginId, _issueTitle, _severity, username, password));
                    }
                }
            });
            return target;
        }

        private static bool IdentityCanLogin(EndpointDescription endpointDescription, UserIdentity userIdentity, out NodeIdCollection roleIds)
        {
            roleIds = new NodeIdCollection();

            try
            {
                ConnectionUtil util = new ConnectionUtil();
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