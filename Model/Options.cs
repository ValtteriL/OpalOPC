namespace Model
{
    public class Options
    {
        public List<Uri> targets = new List<Uri>();
        public string? xmlOutputFile;
        public bool verbose;
        public bool debug;
        public bool shouldShowHelp;
    }
}