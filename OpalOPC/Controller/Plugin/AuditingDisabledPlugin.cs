using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Plugin
{
    public class AuditingDisabledPlugin : Plugin
    {
        // check if auditing disabled
        private static PluginId _pluginId = PluginId.AuditingDisabled;
        private static string _category = PluginCategories.Accounting;
        private static string _issueTitle = "Auditing disabled";

        // Medium
        private static double _severity = 5.0;

        public AuditingDisabledPlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) {}

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for disabled auditing");

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
                    CommonCredentialsIssue credsIssue = (CommonCredentialsIssue) endpoint.Issues.First(i => i.GetType() == typeof(CommonCredentialsIssue));
                    identity = new UserIdentity(username: credsIssue.username, password: credsIssue.password);
                }

                ConnectionUtil util = new ConnectionUtil();
                var session = util.StartSession(endpoint.EndpointDescription, identity).Result;

                // check if auditing enabled
                DataValue auditingValue = session.ReadValue(Util.WellKnownNodes.Server_Auditing);
                if (!(bool)auditingValue.GetValue<System.Boolean>(false))
                {
                    _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has auditing disabled");
                    endpoint.Issues.Add(CreateIssue());
                }
            });

            return target;
        }

    }
}