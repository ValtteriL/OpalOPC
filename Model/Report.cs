using System.Xml.Serialization;

namespace Model
{
    public class Report
    {
        public List<Target> Targets { get; private set; } = new List<Target>();
        public DateTime Timestamp { get; set; }

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