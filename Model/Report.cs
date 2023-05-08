namespace Model
{
    public class Report
    {
        public List<Target> Targets { get; private set; } = new List<Target>();
        public DateTime Timestamp { get; set; }
        public string Version { get; set; } = Util.VersionUtil.AppAssemblyVersion!.ToString();
        public string Command { get; set; } = Environment.CommandLine;
        public string? RunStatus { get; set; }

        // parameterless constructor for XML serializer
        internal Report()
        { }

        public Report(ICollection<Target> opcTargets)
        {
            Targets = opcTargets.ToList();
            Timestamp = DateTime.Now;
        }
    }
}