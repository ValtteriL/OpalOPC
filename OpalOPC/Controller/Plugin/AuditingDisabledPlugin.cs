using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Util;

namespace Plugin
{
    public class AuditingDisabledPlugin : PostAuthPlugin
    {
        // check if auditing disabled
        private static readonly PluginId _pluginId = PluginId.AuditingDisabled;
        private static readonly string _category = PluginCategories.Accounting;
        private static readonly string _issueTitle = "Auditing disabled";

        // Medium
        private static readonly double _severity = 5.0;

        public AuditingDisabledPlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) { }

        public override Issue? Run(ISession session)
        {
            _logger.LogTrace($"Testing {session.Endpoint} for disabled auditing");

            // check if auditing enabled
            DataValue auditingValue = session.ReadValue(Util.WellKnownNodes.Server_Auditing);
            if (!auditingValue.GetValue<bool>(false))
            {
                _logger.LogTrace($"Endpoint {session.Endpoint} has auditing disabled");
                return CreateIssue();
            }

            return null;
        }

    }
}