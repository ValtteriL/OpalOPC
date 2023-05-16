using Microsoft.Extensions.Logging;
using Model;

namespace Plugin
{
    public interface IPlugin
    {
        PluginId Id { get; }
        string Category { get; }
        double Severity { get; }
        public Target Run(Target target);
    }

    public abstract class Plugin : IPlugin
    {
        public ILogger _logger;
        PluginId _pluginId = PluginId.Dummy;
        public PluginId Id
        {
            get => _pluginId;
        }

        double _severity = 0;
        public double Severity
        {
            get => _severity;
        }

        private string _category = PluginCategories.Dummy;
        public string Category
        {
            get => _category;
        }

        public Plugin(ILogger logger)
        {
            _logger = logger;
        }

        public abstract Target Run(Target target);
    }
}