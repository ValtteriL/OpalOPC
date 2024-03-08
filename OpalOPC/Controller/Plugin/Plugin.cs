using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;

namespace Plugin
{
    public enum Plugintype
    {
        PreAuthPlugin = 1,
        PostAuthPlugin = 2,
        SessionCredentialPlugin = 3,
        PostAuthMultipleIssuesPlugin = 4
    }

    public interface IPlugin
    {
        public PluginId Id { get; }

        public Plugintype Type { get; }

        public string Name { get; }

        public double Severity { get; }
    }

    public abstract class Plugin(ILogger logger, PluginId pluginId, string category, string name, double severity)
    {
        public ILogger _logger = logger;
        public PluginId Id { get; } = pluginId;
        public string Name => _name;
        public double Severity => _severity;

        protected readonly double _severity = severity;
        protected readonly string _category = category;
        protected readonly string _name = name;

        protected virtual Issue CreateIssue()
        {
            return new Issue(Id, _name, _severity);
        }
    }

    public interface IPreAuthPlugin : IPlugin
    {
        public (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions);
    }

    public abstract class PreAuthPlugin(ILogger logger, PluginId pluginId, string category, string name, double severity) : Plugin(logger, pluginId, category, name, severity), IPreAuthPlugin
    {
        public virtual Plugintype Type => Plugintype.PreAuthPlugin;

        public abstract (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions);
    }

    public interface IPostAuthPlugin : IPlugin
    {
        public Issue? Run(ISession session);
    }

    public abstract class PostAuthPlugin(ILogger logger, PluginId pluginId, string category, string name, double severity) : Plugin(logger, pluginId, category, name, severity), IPostAuthPlugin
    {
        public Plugintype Type => Plugintype.PostAuthPlugin;
        public abstract Issue? Run(ISession session);
    }

    public interface IMultipleIssuesPostAuthPlugin : IPlugin
    {
        public Task<List<Issue>> Run(ISession session);
    }

    public abstract class MultipleIssuesPostAuthPlugin(ILogger logger, PluginId pluginId, string category, string name, double severity) : Plugin(logger, pluginId, category, name, severity), IMultipleIssuesPostAuthPlugin
    {
        public Plugintype Type => Plugintype.PostAuthMultipleIssuesPlugin;
        public abstract Task<List<Issue>> Run(ISession session);
    }

    public interface ISessionCredentialPlugin : IPlugin
    {
        public Issue? Run(ICollection<ISecurityTestSession> sessionsAndCredentials);
    }

    public abstract class SessionCredentialPlugin(ILogger logger, PluginId pluginId, string category, string name, double severity) : Plugin(logger, pluginId, category, name, severity), ISessionCredentialPlugin
    {
        public Plugintype Type => Plugintype.SessionCredentialPlugin;
        public abstract Issue? Run(ICollection<ISecurityTestSession> sessionsAndCredentials);
    }
}
