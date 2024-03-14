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

        public override Issue? Run(IList<ISession> sessions)
        {
            ISession firstSession = sessions.First();
            _logger.LogTrace("{Message}", $"Testing {firstSession.Endpoint.EndpointUrl} for disabled auditing");

            // try checking auditing status with each session until success
            foreach (ISession session in sessions)
            {
                try
                {
                    // check if auditing enabled
                    DataValue auditingValue = session.ReadValue(Util.WellKnownNodes.Server_Auditing);
                    if (!auditingValue.GetValue(false))
                    {
                        _logger.LogTrace("{Message}", $"Endpoint {session.Endpoint.EndpointUrl} has auditing disabled");
                        return CreateIssue();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (ServiceResultException)
                {
                    // ignore errors like BadUserAccessDenied
                }
            }

            return null;
        }

    }
}
