using Microsoft.Extensions.Logging;

namespace Model
{
    public interface IOptions
    {
        List<Uri> targets { get; set; }
        Stream? OutputStream { get; set; }
        string? OutputReportName { get; set; }
        LogLevel logLevel { get; set; }
        bool shouldShowHelp { get; set; }
        int? exitCode { get; set; }
        bool shouldShowVersion { get; set; }
        AuthenticationData authenticationData { get; set; }
        bool acceptEula { get; set; }
        string commandLine { get; set; }
    }

    public class Options : IDisposable, IOptions
    {
        public List<Uri> targets { get; set;  } = [];
        public Stream? OutputStream { get; set; }
        public string? OutputReportName { get; set; }
        public LogLevel logLevel { get; set; } = LogLevel.Information;
        public bool shouldShowHelp { get; set; }
        public int? exitCode { get; set; }
        public bool shouldShowVersion { get; set; }
        public AuthenticationData authenticationData { get; set; } = new();
        public bool acceptEula { get; set; } = false;
        public string commandLine { get; set; } = string.Empty;

        public void Dispose()
        {
            OutputStream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
