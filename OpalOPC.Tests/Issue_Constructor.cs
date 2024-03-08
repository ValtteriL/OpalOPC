using Model;
using Plugin;
using Xunit;

namespace Tests;
public class Issue_Constructor
{
    [Fact]
    public void constructor_SetsProperties()
    {
        PluginId pluginId = PluginId.BruteForce;
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
