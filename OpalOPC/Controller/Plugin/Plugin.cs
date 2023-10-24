using Microsoft.Extensions.Logging;
using Model;

namespace Plugin
{
    public interface IPlugin
    {
        public Target Run(Target target);
        public Issue CreateIssue();
        public PluginId pluginId { get; }
    }

    public abstract class Plugin : IPlugin
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

        public abstract Target Run(Target target);

        public Issue CreateIssue()
        {
            return new Issue((int)pluginId, _name, _severity);
        }
    }
}