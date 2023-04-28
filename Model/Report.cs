namespace Model
{
    public class Report
    {
        public List<Target> Targets { get; private set; } = new List<Target>();
        DateTime Timestamp { get; set; }

        // parameterless constructor for XML serializer
        internal Report()
        {}

        public Report(ICollection<Target> opcTargets)
        {
            Targets = opcTargets.ToList();
            Timestamp = DateTime.Now;
        }
    }
}