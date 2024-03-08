using Plugin;

namespace Model
{
    public class Issue(PluginId pluginId, string name, double severity)
    {
        public PluginId PluginId { get; } = pluginId;
        public int PluginIdInt = (int)pluginId;
        public string Name { get; } = name;
        public double Severity { get; } = severity;
    }
}
