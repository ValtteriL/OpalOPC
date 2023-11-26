using Microsoft.Extensions.Logging;

namespace Model
{
    public class Options
    {
        public List<Uri> targets = new();
        public Stream? xmlOutputStream;
        public string? xmlOutputReportName;
        public LogLevel logLevel = LogLevel.Information;
        public bool shouldShowHelp;
        public int? exitCode;
        public bool shouldShowVersion;
        public AuthenticationData authenticationData = new();
    }
}
