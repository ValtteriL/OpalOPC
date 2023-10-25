using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua.Client;

namespace Plugin
{
    public interface IPlugin
    {
        public Issue CreateIssue();
        public PluginId pluginId { get; }
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

    public interface IPreAuthPlugin
    {
        public Issue? Run(Endpoint endpoint);
    }

    public abstract class PreAuthPlugin : Plugin, IPlugin, IPreAuthPlugin
    {
        public PreAuthPlugin(ILogger logger, PluginId pluginId, string category, string name, double severity) : base(logger, pluginId, category, name, severity)
        {
        }

        public abstract Issue? Run(Endpoint endpoint);
    }

    public interface IPostAuthPlugin
    {
        public Issue? Run(ISession session);
    }

    public abstract class PostAuthPlugin : Plugin, IPlugin, IPostAuthPlugin
    {
        public PostAuthPlugin(ILogger logger, PluginId pluginId, string category, string name, double severity) : base(logger, pluginId, category, name, severity)
        {
        }

        public abstract Issue? Run(ISession session);
    }
}