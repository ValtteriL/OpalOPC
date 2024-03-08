using Model;
using Opc.Ua;
using Plugin;
using View;
using Xunit;

namespace Tests;
public class HtmlReporterTest
{
    private readonly HtmlReporter _reporter;
    private readonly MemoryStream _memoryStream;
    private readonly string _commandLine = "commandline";
    private readonly string _runStatus = "rustatus";
    private readonly Target _target;
    private readonly List<Target> _targets;

    public HtmlReporterTest()
    {
        _memoryStream = new();
        _reporter = new();

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
        server.AddIssue(new Issue(PluginId.BruteForce, "description", 1.0));
        server.AddIssue(new Issue(PluginId.AuditingDisabled, "issue name 2", 6.0));
        server.AddIssue(new Issue(PluginId.AnonymousAuthentication, "issue name 3", 9.2));
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
        StreamReader reader = new(_memoryStream);
        string reportString = reader.ReadToEnd();

        VerifyReportContainsAllInfo(report, reportString);
    }

    private static void VerifyReportContainsAllInfo(Report report, string reportString)
    {
        Assert.Contains(report.StartTimeString, reportString);
        Assert.Contains(report.EndTimeString, reportString);
        Assert.Contains(report.Targets.Count.ToString(), reportString);
        Assert.Contains(report.Version, reportString);
        Assert.Contains(report.Command, reportString);
        Assert.Contains(report.RunStatus, reportString);
        foreach (Target target in report.Targets)
        {
            Assert.Contains(target.ApplicationName, reportString);
            Assert.Contains(target.ApplicationUri, reportString);
            Assert.Contains(target.Type.ToString(), reportString);
            Assert.Contains(target.ProductUri, reportString);

            Assert.Contains(target.Servers.Count.ToString(), reportString);
            foreach (Server server in target.Servers)
            {
                Assert.Contains(server.DiscoveryUrl, reportString);
                Assert.Contains(server.Issues.Count.ToString(), reportString);
                Assert.Contains(server.Errors.Count.ToString(), reportString);
                foreach (Issue issue in server.Issues)
                {
                    Assert.Contains(issue.Name, reportString);
                    Assert.Contains(issue.PluginIdInt.ToString(), reportString);
                    Assert.Contains(issue.Severity.ToString(), reportString);
                }
                foreach (Error error in server.Errors)
                {
                    Assert.Contains(error.Message, reportString);
                }
            }
        }
    }
}
