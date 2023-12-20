using Microsoft.Extensions.Logging;

namespace Model
{
    public class Options : IDisposable
    {
        public List<Uri> targets = new();
        public Stream? OutputStream;
        public string? OutputReportName;
        public LogLevel logLevel = LogLevel.Information;
        public bool shouldShowHelp;
        public int? exitCode;
        public bool shouldShowVersion;
        public AuthenticationData authenticationData = new();
        public bool acceptEula = false;

        public void Dispose()
        {
            OutputStream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
