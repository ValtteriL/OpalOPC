namespace Model
{
    public class Issue(int pluginId, string name, double severity)
    {
        public int PluginId { get; set; } = pluginId;
        public string Name { get; set; } = name;
        public double Severity { get; set; } = severity;
    }
}
