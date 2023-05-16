using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Plugin
{
    public class CommonCredentialsPlugin : Plugin
    {
        private PluginId _pluginId = PluginId.CommonCredentials;
        private string _category = PluginCategories.Authentication;

        public CommonCredentialsPlugin(ILogger logger) : base(logger) { }

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
                        endpoint.Issues.Add(Issues.CommonCredentials(username, password, roleIds));
                    }
                }
            });
            return target;
        }

        private bool IdentityCanLogin(EndpointDescription endpointDescription, UserIdentity userIdentity, out NodeIdCollection roleIds)
        {
            roleIds = new NodeIdCollection();

            try
            {
                ConnectionUtil util = new ConnectionUtil();
                var session = util.StartSession(endpointDescription, userIdentity).Result;
                bool result = false;
                if (session.Connected)
                {
                    result = true;
                    roleIds = session.Identity.GrantedRoleIds;
                }

                session.Close();
                session.Dispose();

                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}