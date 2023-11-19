using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua.Client;

namespace Plugin
{
    public enum Plugintype
    {
        PreAuthPlugin = 1,
        PostAuthPlugin = 2,
        AuthPlugin = 3,
    }

    public interface IPlugin
    {
        public Issue CreateIssue();
        public PluginId pluginId { get; }

        public Plugintype Type { get; }
    }

    public abstract class Plugin
    {
        public ILogger _logger;
        public PluginId pluginId { get; }
        private readonly double _severity;
        private readonly string _category;
        private readonly string _name;

        public Plugin(ILogger logger, PluginId pluginId, string category, string name, double severity)
        {
            _logger = logger;
            this.pluginId = pluginId;
            _category = category;
            _name = name;
            _severity = severity;
        }

        public Issue CreateIssue()
        {
            return new Issue((int)pluginId, _name, _severity);
        }
    }

    public interface IPreAuthPlugin : IPlugin
    {
        public (Issue?, ICollection<ISession>) Run(Endpoint endpoint);
    }

    public abstract class PreAuthPlugin : Plugin, IPreAuthPlugin
    {
        public PreAuthPlugin(ILogger logger, PluginId pluginId, string category, string name, double severity) : base(logger, pluginId, category, name, severity)
        {
        }

        public virtual Plugintype Type => Plugintype.PreAuthPlugin;

        public abstract (Issue?, ICollection<ISession>) Run(Endpoint endpoint);
    }

    public interface IPostAuthPlugin : IPlugin
    {
        public Issue? Run(ISession session);
    }

    public abstract class PostAuthPlugin : Plugin, IPostAuthPlugin
    {
        public PostAuthPlugin(ILogger logger, PluginId pluginId, string category, string name, double severity) : base(logger, pluginId, category, name, severity)
        {
        }

        public Plugintype Type => Plugintype.PostAuthPlugin;
        public abstract Issue? Run(ISession session);
    }
}
