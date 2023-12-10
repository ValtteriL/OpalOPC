using System.Xml.Serialization;

namespace Model
{
    public class Report
    {
        public List<Target> Targets { get; private set; } = new();
        public string StartTime { get; private set; }
        public string EndTime { get; private set; }
        public string Version { get; private set; } = Util.VersionUtil.AppAssemblyVersion!.ToString();
        public string Command { get; private set; }
        public string RunStatus { get; private set; }

        private const string Dateformat = "ddd dd MMMM HH:mm:ss yyyy";

        public Report(ICollection<Target> opcTargets, DateTime Start, DateTime End, string commandLine, string runStatus)
        {
            // sort issues in servers by severity
            foreach (Target target in opcTargets)
            {
                target.SortServersByIssueSeverity();
            }

            // sort targets by highest server issue severity
            Targets = opcTargets.OrderByDescending(t => t.Servers.Any() ? t.Servers.Max(s => s.Issues.Any() ? s.Issues.Max(i => i.Severity) : 0) : 0).ToList();

            StartTime = Start.ToString(Dateformat);
            EndTime = End.ToString(Dateformat);
            Command = commandLine;
            RunStatus = runStatus;
        }
    }
}
