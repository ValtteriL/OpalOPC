using Microsoft.Extensions.Logging;
using Model;

namespace Plugin
{
    public interface IPluginRepository
    {
        List<IPlugin> GetAll(AuthenticationData authenticationData);
    }

    public class PluginRepository(ILogger<IPluginRepository> logger) : IPluginRepository
    {
        private List<IPlugin> _plugins = [];


        public List<IPlugin> GetAll(AuthenticationData authenticationData)
        {
            if (_plugins.Count == 0)
                InitializePlugins(authenticationData);

            return _plugins;
        }

        private void InitializePlugins(AuthenticationData authenticationData)
        {
            _plugins =
            [
            new SecurityModeInvalidPlugin(logger),
                new SecurityModeNonePlugin(logger),

                new SecurityPolicyBasic128Rsa15Plugin(logger),
                new SecurityPolicyBasic256Plugin(logger),
                new SecurityPolicyNonePlugin(logger),

                new AnonymousAuthenticationPlugin(logger, authenticationData),
                new SelfSignedCertificatePlugin(logger),

                new ProvidedCredentialsPlugin(logger, authenticationData),
                new CommonCredentialsPlugin(logger, authenticationData),
                new BruteForcePlugin(logger, authenticationData),
                new RBACNotSupportedPlugin(logger),
                new AuditingDisabledPlugin(logger),
                new ServerStatusPlugin(logger),
                new ServerCertificateInvalidPlugin(logger),
            ];
        }
    }
}
