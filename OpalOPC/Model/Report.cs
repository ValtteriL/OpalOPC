using System.Xml.Serialization;

namespace Model
{
    [XmlInclude(typeof(CommonCredentialsIssue))]
    public class Report
    {
        public List<Target> Targets { get; private set; } = new List<Target>();
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string Version { get; set; } = Util.VersionUtil.AppAssemblyVersion!.ToString();
        public string? Command { get; set; }
        public string? RunStatus { get; set; }

        private const string dateformat = "ddd MMMM HH:mm:ss yyyy";

        // parameterless constructor for XML serializer
        internal Report()
        { }

        public Report(ICollection<Target> opcTargets, DateTime Start, DateTime End, string commandLine)
        {
            Targets = opcTargets.ToList();
            StartTime = Start.ToString(dateformat);
            EndTime = End.ToString(dateformat);
            Command = commandLine;
        }
    }
}