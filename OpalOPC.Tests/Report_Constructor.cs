using Model;
using Xunit;

namespace Tests;
public class Report_Constructor
{

    [Fact]
    public void constructor_SetsProperties()
    {
        DateTime start = DateTime.Now;
        DateTime stop = DateTime.MaxValue;
        List<Target> targets = [];
        string commandLine = string.Empty;
        string runStatus = string.Empty;

        Report report = new(targets, start, stop, commandLine, runStatus);

        Assert.True(report.Targets.Count == targets.Count);
        Assert.True(report.RunStatus == runStatus);
        Assert.True(report.Command == commandLine);
        Assert.True(report.Version != null);
    }
}
