using Microsoft.CodeAnalysis.Sarif;
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using Plugin;
using Util;
using View;
using Xunit;

namespace Tests;
public class SarifReporterTest
{
    private readonly Mock<IKnownVulnerabilityApiRequestUtil> _knownVulnerabilityApiRequestUtil = new();
    private readonly Mock<ILogger<IPluginRepository>> _logger = new();

    private readonly SarifReporter _reporter;
    private readonly MemoryStream _memoryStream = new();
    private readonly string _commandLine = "commandline";
    private readonly string _runStatus = "runtatus";
    private readonly Target _target;
    private readonly List<Target> _targets;

    public SarifReporterTest()
    {
        PluginRepository _pluginRepository = new(_logger.Object, _knownVulnerabilityApiRequestUtil.Object);
        _reporter = new(_pluginRepository);

        _target = new(new ApplicationDescription()
        {
            ApplicationType = ApplicationType.Client,
            ApplicationName = "application 1011",
            ApplicationUri = "applicationUri 1042111",
            ProductUri = "productUri 1014123211"
        });
        Server server = new("opc.tcp://discoveryuri", []);
        server.AddError(new Error("error message 1"));
        server.AddError(new Error("error message 2"));
        server.AddError(new Error("error message 3"));
        server.AddIssue(new Issue(PluginId.AuditingDisabled, "issue name 1", 1.0));
        server.AddIssue(new Issue(PluginId.CommonCredentials, "issue name 2", 6.0));
        server.AddIssue(new Issue(PluginId.SecurityModeNone, "issue name 3", 9.2));
        _target.AddServer(server);

        _targets = [_target];
    }

    [Fact]
    public void WriteReportToStream_NonNullReportSucceeds()
    {
        Report report = new([], DateTime.Now, DateTime.Now, _commandLine, _runStatus);

        _reporter.WriteReportToStream(report, _memoryStream);
    }

    [Fact]
    public void WriteReportToStream_ReportContainsAllInfo()
    {
        Report report = new(_targets, DateTime.Now, DateTime.Now, _commandLine, _runStatus);

        _reporter.WriteReportToStream(report, _memoryStream);

        _memoryStream.Seek(0, SeekOrigin.Begin);
        SarifLog sarifLog = SarifLog.Load(_memoryStream);

        VerifyReportContainsAllInfo(report, sarifLog);
    }

    private static void VerifyReportContainsAllInfo(Report report, SarifLog sarifLog)
    {
        Assert.True(sarifLog.Runs.Count == 1);
        Run run = sarifLog.Runs[0];

        Assert.True(run.Invocations.Count == 1);
        Invocation invocation = run.Invocations[0];

        // check that misc info is in report

        Assert.True(run.Tool.Driver.Version == report.Version);
        Assert.True(invocation.CommandLine == report.Command);

        // check that invocation starttimeutc and report.starttime.touniversaltime are within 1 second of each other
        // sarif does some rounding of the time, so we can't check for exact equality
        Assert.True(invocation.StartTimeUtc - report.StartTime.ToUniversalTime() < TimeSpan.FromSeconds(1));
        Assert.True(invocation.EndTimeUtc - report.EndTime.ToUniversalTime() < TimeSpan.FromSeconds(1));

        Assert.True(invocation.ExitCode == 0);
        Assert.True(invocation.ExecutionSuccessful);

        foreach (Target target in report.Targets)
        {
            foreach (Server server in target.Servers)
            {
                foreach (Issue issue in server.Issues)
                {
                    // check that all issues are in report

                    Result result = run.Results.Where(r => r.RuleId == $"{issue.PluginIdInt}").First();
                    Assert.True(issue.Name == result.Message.Text);
                    Assert.True(result.Locations[0].PhysicalLocation.ArtifactLocation.Uri == new Uri(server.DiscoveryUrl));
                    Assert.True(result.Rank == issue.Severity * 10);

                    if (issue.Severity > 0)
                        Assert.True(result.Level == FailureLevel.Error);
                    else
                        Assert.True(result.Level == FailureLevel.Note);

                    ReportingDescriptor rule = run.Tool.Driver.Rules.Where(r => r.Id == $"{issue.PluginIdInt}").First();

                    Assert.True(result.Level == rule.DefaultConfiguration.Level);
                    Assert.True(result.RuleIndex == run.Tool.Driver.Rules.IndexOf(rule));

                    Assert.True(run.Artifacts.Where(a => a.Description.Text.Contains(target.ApplicationName)).Any());
                }
                foreach (Error error in server.Errors)
                {
                    // check that all errors are in report

                    Result result = run.Results.First(r => r.Message.Text == error.Message);
                    ReportingDescriptor rule = run.Tool.Driver.Rules.Where(r => r.Id == SarifReporter.ErrorRuleId).First();

                    Assert.True(result.RuleIndex == run.Tool.Driver.Rules.IndexOf(rule));
                    Assert.True(result.Level == FailureLevel.Warning);
                    Assert.True(result.Rank == 0);
                    Assert.True(result.Locations[0].PhysicalLocation.ArtifactLocation.Uri == new Uri(server.DiscoveryUrl));
                }
            }
        }
    }
}
