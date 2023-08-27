namespace Tests;
using Model;
using Xunit;

public class Report_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        Report report = new Report(new List<Target>(), DateTime.Now, DateTime.Now, string.Empty);

        Assert.True(report != null);
    }

    [Fact]
    public void constructor_SetsProperties()
    {
        DateTime start = DateTime.Now;
        DateTime stop = DateTime.MaxValue;
        List<Target> targets = new List<Target>();
        string commandLine = string.Empty;
        
        Report report = new Report(targets, start, stop, commandLine);

        Assert.True(report.Targets.Count == targets.Count);
        Assert.True(report.RunStatus == null);
        Assert.True(report.Command != null);
        Assert.True(report.Version != null);
        Assert.True(report.StartTime != null);
        Assert.True(report.EndTime != null);
    }
}