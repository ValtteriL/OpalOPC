using Microsoft.Extensions.Logging;
using Util;

namespace Model
{
    public interface IOptions
    {
        public List<Uri> targets { get; init; }
        public Stream? HtmlOutputStream { get; init; }
        public Stream? SarifOutputStream { get; init; }
        public string? HtmlOutputReportName { get; init; }
        public string? SarifOutputReportName { get; init; }
        public LogLevel logLevel { get; init; }
        public bool shouldShowHelp { get; init; }
        public int exitCode { get; set; }
        public bool shouldExit { get; init; }
        public bool shouldShowVersion { get; init; }
        public bool shouldDiscoverAndExit { get; init; }
        public AuthenticationData authenticationData { get; init; }
        public string commandLine { get; init; }
        public string apiUri { get; set; }
        public bool shouldStoreLicenseAndExit { get; init; }
        public string licenseKey { get; init; }
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
        public int exitCode { get; set; } = ExitCodes.Success;
        public bool shouldExit { get; init; } = false;
        public bool shouldShowVersion { get; init; }
        public bool shouldDiscoverAndExit { get; init; }
        public AuthenticationData authenticationData { get; init; } = new();
        public string commandLine { get; init; } = string.Empty;
        public string apiUri { get; set; } = "https://api.opalopc.com/known-vulnerabilities";
        public bool shouldStoreLicenseAndExit { get; init; } = false;
        public string licenseKey { get; init; } = string.Empty;

        public void Dispose()
        {
            HtmlOutputStream?.Dispose();
            SarifOutputStream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
