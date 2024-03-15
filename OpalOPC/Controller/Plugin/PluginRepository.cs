using Microsoft.Extensions.Logging;
using Model;
using Util;

namespace Plugin
{
    public interface IPluginRepository
    {
        List<IPlugin> BuildAll(AuthenticationData authenticationData);
        List<IPlugin> GetAll();
    }

    public class PluginRepository(ILogger<IPluginRepository> logger, IKnownVulnerabilityApiRequestUtil knownVulnerabilityApiRequestUtil, ISelfSignedCertificateUtil selfSignedCertificateUtil, IConnectionUtil connectionUtil) : IPluginRepository
    {

        public List<IPlugin> GetAll() => InitializePlugins(new AuthenticationData()); // just for getting all plugins, but not running them

        public List<IPlugin> BuildAll(AuthenticationData authenticationData) => InitializePlugins(authenticationData); // for running the plugins

        private List<IPlugin> InitializePlugins(AuthenticationData authenticationData)
        {
            return [
            new SecurityModeInvalidPlugin(logger),
                new SecurityModeNonePlugin(logger),

                new SecurityPolicyBasic128Rsa15Plugin(logger),
                new SecurityPolicyBasic256Plugin(logger),
                new SecurityPolicyNonePlugin(logger),

                new AnonymousAuthenticationPlugin(logger, connectionUtil, authenticationData),
                new SelfSignedCertificatePlugin(logger),

                new ProvidedCredentialsPlugin(logger, connectionUtil, authenticationData),
                new CommonCredentialsPlugin(logger, connectionUtil, authenticationData),
                new BruteForcePlugin(logger, connectionUtil, authenticationData),
                new RBACNotSupportedPlugin(logger),
                new AuditingDisabledPlugin(logger),
                new ServerStatusPlugin(logger),
                new ServerCertificateInvalidPlugin(logger),
                new ServerCertificatePlugin(logger),
                new SelfSignedUserCertificatePlugin(logger, selfSignedCertificateUtil, connectionUtil, authenticationData),
                new KnownVulnerabilityPlugin(logger, knownVulnerabilityApiRequestUtil),
            ];
        }
    }
}
