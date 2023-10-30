using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;
using Util;

namespace Plugin
{
    public class RBACNotSupportedPlugin : PostAuthPlugin
    {
        // check if auditing disabled
        private static readonly PluginId _pluginId = PluginId.RBACNotSupported;
        private static readonly string _category = PluginCategories.Authorization;
        private static readonly string _issueTitle = "RBAC not supported";

        // Info
        private static readonly double _severity = 0;

        public RBACNotSupportedPlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) { }

        public override Issue? Run(ISession session)
        {
            _logger.LogTrace($"Testing {session.Endpoint} for RBAC support");

            // check if rbac supported (if its advertised in profiles or not)
            DataValue serverProfileArrayValue = session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray);
            string[] serverProfileArray = serverProfileArrayValue.GetValue<string[]>(Array.Empty<string>());
            if (!serverProfileArray.Intersect(RBAC_Profiles).Any())
            {
                _logger.LogTrace($"Endpoint {session.Endpoint} is not capable of RBAC");
                return CreateIssue();
            }

            return null;
        }

        private readonly ICollection<string> RBAC_Profiles = new List<string> {
                Util.WellKnownProfiles.Security_User_Access_Control_Full,
                Util.WellKnownProfileURIs.Security_User_Access_Control_Full,
                Util.WellKnownProfiles.UAFX_Controller_Server_Profile,
                Util.WellKnownProfileURIs.UAFX_Controller_Server_Profile
            };

    }
}
