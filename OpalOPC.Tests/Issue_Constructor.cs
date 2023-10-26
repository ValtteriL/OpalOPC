namespace Tests;
using Model;
using Xunit;

public class Issue_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        int pluginId = 0;
        string name = "a";
        double severity = 0;

        Issue issue = new(
                pluginId, name, severity
            );

        Assert.True(issue != null);
    }

    [Fact]
    public void constructor_SetsProperties()
    {
        int pluginId = 0;
        string name = "a";
        double severity = 0;
        Issue issue = new(
                pluginId, name, severity
            );

        Assert.True(issue.PluginId == pluginId);
        Assert.True(issue.Name == name);
        Assert.True(issue.Severity == severity);
    }
}