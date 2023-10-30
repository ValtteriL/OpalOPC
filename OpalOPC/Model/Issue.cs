using Opc.Ua;

namespace Model
{
    public class Issue
    {
        public int? PluginId { get; set; }
        public string? Name { get; set; }
        public double? Severity { get; set; }

        // parameterless constructor for XML serializer
        internal Issue()
        { }

        public Issue(int pluginId, string name, double severity)
        {
            this.PluginId = pluginId;
            this.Name = name;
            this.Severity = severity;
        }
    }
}
