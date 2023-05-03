namespace Model
{
    public class Options
    {
        public List<Uri> targets = new List<Uri>();
        public Stream? xmlOutputStream;
        public bool verbose;
        public bool debug;
        public bool shouldShowHelp;
    }
}