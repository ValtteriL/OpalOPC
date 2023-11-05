using System.Xml.Serialization;

namespace Model
{
    [XmlInclude(typeof(CommonCredentialsIssue))]
    public class Report
    {
        public List<Target> Targets { get; private set; } = new();
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string Version { get; set; } = Util.VersionUtil.AppAssemblyVersion!.ToString();
        public string? Command { get; set; }
        public string? RunStatus { get; set; }

        private const string Dateformat = "ddd dd MMMM HH:mm:ss yyyy";

        // parameterless constructor for XML serializer
        internal Report()
        { }

        public Report(ICollection<Target> opcTargets, DateTime Start, DateTime End, string commandLine)
        {
            // Merge opctarget endpoints
            foreach (Target target in opcTargets)
            {
                target.MergeEndpoints();
            }

            // sort targets by highest server issue severity
            Targets = opcTargets.OrderByDescending(t => t.Servers.Max(s => s.Endpoints.Max(e => e.Issues.Max(i => i.Severity)))).ToList();

            StartTime = Start.ToString(Dateformat);
            EndTime = End.ToString(Dateformat);
            Command = commandLine;
        }
    }
}
