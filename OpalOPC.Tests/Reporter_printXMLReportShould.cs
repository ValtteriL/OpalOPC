using Model;
using Opc.Ua;
using View;
using Xunit;

namespace Tests;
public class Reporter_printXHTMLReportShould
{
    private readonly Reporter _reporter;
    private readonly MemoryStream _memoryStream;
    private readonly string _commandLine = "commandline";
    private readonly string _runStatus = "rustatus";
    private readonly Target _target;
    private readonly List<Target> _targets;

    public Reporter_printXHTMLReportShould()
    {
        _memoryStream = new();

        StreamWriter sw = new(_memoryStream)
        {
            AutoFlush = true
        };
        _reporter = new(sw.BaseStream);

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
        server.AddIssue(new Issue(1, "issue name 1", 1.0));
        server.AddIssue(new Issue(2, "issue name 2", 6.0));
        server.AddIssue(new Issue(3, "issue name 3", 9.2));
        _target.AddServer(server);

        _targets = [_target];
    }

    [Fact]
    public void printXHTMLReport_NonNullReportSucceeds()
    {
        Report report = new(new List<Target>(), DateTime.Now, DateTime.Now, _commandLine, _runStatus);

        _reporter.PrintXHTMLReport(report);
    }

    [Fact]
    public void printXHTMLReport_ReportResourcesReplacedInTestingReports()
    {
        Report report = new(_targets, DateTime.Now, DateTime.Now, _commandLine, _runStatus);

        _reporter.PrintXHTMLReport(report);

        _memoryStream.Seek(0, SeekOrigin.Begin);
        StreamReader reader = new(_memoryStream);
        string reportString = reader.ReadToEnd();

        Assert.Contains(Util.XmlResources.DebugResourcePath, reportString);
        Assert.DoesNotContain(Util.XmlResources.ProdResourcePath, reportString);
        VerifyReportContainsAllInfo(report, reportString);
    }

    private static void VerifyReportContainsAllInfo(Report report, string reportString)
    {
        Assert.Contains(report.StartTime.ToString(), reportString);
        Assert.Contains(report.EndTime.ToString(), reportString);
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
                    Assert.Contains(issue.PluginId.ToString(), reportString);
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
