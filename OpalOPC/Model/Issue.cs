namespace Model
{
    public class Issue
    {
        public int PluginId { get; set; }
        public string Name { get; set; }
        public double Severity { get; set; }

        public Issue(int pluginId, string name, double severity)
        {
            PluginId = pluginId;
            Name = name;
            Severity = severity;
        }
    }
}
