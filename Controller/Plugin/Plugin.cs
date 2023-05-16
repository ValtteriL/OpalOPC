using Microsoft.Extensions.Logging;
using Model;

namespace Plugin
{
    public interface IPlugin
    {
        PluginId Id { get; }
        string Category { get; }
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

        private string _category = "Dummy";
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