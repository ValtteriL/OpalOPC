using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;

namespace Plugin
{
    public class RBACNotSupportedPlugin : PostAuthPlugin
    {
        // check if auditing disabled
        private static readonly PluginId s_pluginId = PluginId.RBACNotSupported;
        private static readonly string s_category = PluginCategories.Authorization;
        private static readonly string s_issueTitle = "RBAC not supported";

        // Info
        private static readonly double s_severity = 0;

        public RBACNotSupportedPlugin(ILogger logger) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity) { }

        public override Issue? Run(ISession session)
        {
            _logger.LogTrace("{Message}", $"Testing {session.Endpoint.EndpointUrl} for RBAC support");

            // check if rbac supported (if its advertised in profiles or not)
            DataValue serverProfileArrayValue = session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray);
            string[] serverProfileArray = serverProfileArrayValue.GetValue<string[]>(Array.Empty<string>());
            if (!serverProfileArray.Intersect(_rBAC_Profiles).Any())
            {
                _logger.LogTrace("{Message}", $"Endpoint {session.Endpoint.EndpointUrl} is not capable of RBAC");
                return CreateIssue();
            }

            return null;
        }

        private readonly ICollection<string> _rBAC_Profiles = new List<string> {
                Util.WellKnownProfiles.Security_User_Access_Control_Full,
                Util.WellKnownProfileURIs.Security_User_Access_Control_Full,
                Util.WellKnownProfiles.UAFX_Controller_Server_Profile,
                Util.WellKnownProfileURIs.UAFX_Controller_Server_Profile
            };

    }
}
