using Microsoft.Extensions.Logging;
using Model;

namespace Plugin
{
    public interface IPlugin
    {
        public Target Run(Target target);
        public Issue CreateIssue();
    }

    public abstract class Plugin : IPlugin
    {
        public ILogger _logger;
        private PluginId _pluginId;
        private double _severity;
        private string _category;
        private string _name;

        public Plugin(ILogger logger, PluginId pluginId, string category, string name, double severity)
        {
            _logger = logger;
            _pluginId = pluginId;
            _category = category;
            _name = name;
            _severity = severity;
        }

        public abstract Target Run(Target target);

        public Issue CreateIssue()
        {
            return new Issue((int)_pluginId, _name, _severity);
        }
    }
}