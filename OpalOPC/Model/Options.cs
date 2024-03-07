using Microsoft.Extensions.Logging;
using Util;

namespace Model
{
    public interface IOptions
    {
        List<Uri> targets { get; }
        Stream? HtmlOutputStream { get; }
        Stream? SarifOutputStream { get; }
        string? HtmlOutputReportName { get; }
        string? SarifOutputReportName { get; }
        LogLevel logLevel { get; }
        bool shouldShowHelp { get; }
        bool shouldExit { get; }
        int exitCode { get; }
        bool shouldShowVersion { get; }
        AuthenticationData authenticationData { get; }
        string commandLine { get; }
    }

    public class Options : IDisposable, IOptions
    {
        public List<Uri> targets { get; init; } = [];
        public Stream? HtmlOutputStream { get; init; }
        public Stream? SarifOutputStream { get; init; }
        public string? HtmlOutputReportName { get; init; }
        public string? SarifOutputReportName { get; init; }
        public LogLevel logLevel { get; init; } = LogLevel.Information;
        public bool shouldShowHelp { get; init; }
        public int exitCode { get; init; } = ExitCodes.Success;
        public bool shouldExit { get; init; } = false;
        public bool shouldShowVersion { get; init; }
        public bool shouldDiscoverAndExit { get; init; }
        public AuthenticationData authenticationData { get; init; } = new();
        public string commandLine { get; init; } = string.Empty;
        public string apiUri { get; set; } = "https://api.opalopc.com/known-vulnerabilities";

        public void Dispose()
        {
            HtmlOutputStream?.Dispose();
            SarifOutputStream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
