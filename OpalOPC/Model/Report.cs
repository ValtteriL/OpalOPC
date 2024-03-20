namespace Model
{
    public class Report
    {
        public List<Target> Targets { get; private set; } = [];
        public DateTime StartTime { get; private set; }
        public string StartTimeString => StartTime.ToString(Dateformat);
        public DateTime EndTime { get; private set; }
        public string EndTimeString => EndTime.ToString(Dateformat);
        public string Version { get; private set; } = Util.VersionUtil.AppVersion;
        public string Command { get; private set; }
        public string RunStatus { get; private set; }

        private const string Dateformat = "ddd dd MMMM HH:mm:ss yyyy";

        public Report(ICollection<Target> opcTargets, DateTime startTime, DateTime endTime, string commandLine, string runStatus)
        {
            // sort issues in servers by severity
            foreach (Target target in opcTargets)
            {
                target.SortServersByIssueSeverity();
            }

            // sort targets by highest server issue severity
            Targets =
            [
                .. opcTargets.OrderByDescending(t => t.Servers.Count != 0 ? t.Servers.Max(s => s.Issues.Count != 0 ? s.Issues.Max(i => i.Severity) : 0) : 0),
            ];

            StartTime = startTime;
            EndTime = endTime;
            Command = commandLine;
            RunStatus = runStatus;
        }
    }
}
