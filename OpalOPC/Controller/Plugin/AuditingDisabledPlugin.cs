using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;

namespace Plugin
{
    public class AuditingDisabledPlugin(ILogger logger) : PostAuthPlugin(logger, s_pluginId, s_category, s_issueTitle, s_severity)
    {
        // check if auditing disabled
        private static readonly PluginId s_pluginId = PluginId.AuditingDisabled;
        private static readonly string s_category = PluginCategories.Accounting;
        private static readonly string s_issueTitle = "Auditing disabled";

        // Medium
        private static readonly double s_severity = 5.0;

        public override Issue? Run(ISession session)
        {
            _logger.LogTrace("{Message}", $"Testing {session.Endpoint.EndpointUrl} for disabled auditing (session {session.Identity.DisplayName})");

            try
            {
                // check if auditing enabled
                DataValue auditingValue = session.ReadValue(Util.WellKnownNodes.Server_Auditing);
                if (!auditingValue.GetValue(false))
                {
                    _logger.LogTrace("{Message}", $"Endpoint {session.Endpoint.EndpointUrl} has auditing disabled");
                    return CreateIssue();
                }
            }
            catch (ServiceResultException)
            {
                // ignore errors like BadUserAccessDenied
            }

            return null;
        }

    }
}
