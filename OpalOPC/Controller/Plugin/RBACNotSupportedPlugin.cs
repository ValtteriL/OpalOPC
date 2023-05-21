using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Plugin
{
    public class RBACNotSupportedPlugin : Plugin
    {
        // check if auditing disabled
        private static PluginId _pluginId = PluginId.RBACNotSupported;
        private static string _category = PluginCategories.Authorization;
        private static string _issueTitle = "RBAC not supported";

        // Info
        private static double _severity = 0;

        public RBACNotSupportedPlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) { }

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for RBAC support");

            // take all endpoints where login is possible
            IEnumerable<Endpoint> targetEndpoints = target.GetLoginSuccessfulEndpoints();

            Parallel.ForEach(targetEndpoints, endpoint =>
            {
                UserIdentity identity;

                // use anonymous if available, otherwise first valid credential
                if (endpoint.UserTokenTypes.Contains(UserTokenType.Anonymous))
                {
                    identity = new UserIdentity();
                }
                else
                {
                    CommonCredentialsIssue credsIssue = (CommonCredentialsIssue)endpoint.Issues.First(i => i.GetType() == typeof(CommonCredentialsIssue));
                    identity = new UserIdentity(username: credsIssue.username, password: credsIssue.password);
                }

                ConnectionUtil util = new ConnectionUtil();
                var session = util.StartSession(endpoint.EndpointDescription, identity).Result;

                // check if rbac supported (if its advertised in profiles or not)
                DataValue serverProfileArrayValue = session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray);
                string[] serverProfileArray = (string[])serverProfileArrayValue.GetValue<string[]>(new string[0]);
                if (!serverProfileArray.Intersect(RBAC_Profiles).Any())
                {
                    _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} is not capable of RBAC");
                    endpoint.Issues.Add(CreateIssue());
                }
            });

            return target;
        }

        private ICollection<string> RBAC_Profiles = new List<string> {
                Util.WellKnownProfiles.Security_User_Access_Control_Full,
                Util.WellKnownProfileURIs.Security_User_Access_Control_Full,
                Util.WellKnownProfiles.UAFX_Controller_Server_Profile,
                Util.WellKnownProfileURIs.UAFX_Controller_Server_Profile
            };

    }
}