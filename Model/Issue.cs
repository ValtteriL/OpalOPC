using Opc.Ua;

namespace Model
{
    public class Issue
    {
        public int? PluginId { get; set; }
        public string? Title { get; set; }
        public double? Severity { get; set; }

        // parameterless constructor for XML serializer
        internal Issue()
        { }

        public Issue(int pluginId, string title, double severity)
        {
            this.PluginId = pluginId;
            this.Title = title;
            this.Severity = severity;
        }
    }
}