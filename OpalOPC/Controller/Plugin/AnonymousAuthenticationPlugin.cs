using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;

namespace Plugin
{
    public class AnonymousAuthenticationPlugin : PreAuthPlugin
    {
        // "′anonymous′ should be used only for accessing non-critical UA server resources"
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
        // try anonymous authentication
        private static readonly PluginId _pluginId = PluginId.AnonymousAuthentication;
        private static readonly string _category = PluginCategories.Authentication;
        private static readonly string _issueTitle = "Anonymous authentication enabled";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:L
        private static readonly double _severity = 7.3;

        public AnonymousAuthenticationPlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) { }

        public override Issue? Run(Endpoint endpoint)
        {
            _logger.LogTrace($"Testing {endpoint} for anonymous access");

            if (endpoint.UserTokenTypes.Contains(UserTokenType.Anonymous))
            {
                return CreateIssue();
            }

            return null;
        }

    }
}