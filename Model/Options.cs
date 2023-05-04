using Microsoft.Extensions.Logging;

namespace Model
{
    public class Options
    {
        public List<Uri> targets = new List<Uri>();
        public Stream? xmlOutputStream;
        public LogLevel logLevel = LogLevel.Information;
        public bool shouldShowHelp;
    }
}