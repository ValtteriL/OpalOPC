using Model;
using Xunit;

namespace Tests;
public class CommonCredentialsIssue_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        int pluginId = 0;
        string name = "a";
        double severity = 0;
        List<(string, string)> credentials = new() { ("username", "password") };
        CommonCredentialsIssue commonCredentialsIssue = new(
                pluginId, name, severity, credentials
            );

        Assert.True(commonCredentialsIssue != null);
    }

    [Fact]
    public void constructor_SetsProperties()
    {
        int pluginId = 0;
        string name = "a";
        double severity = 0;
        List<(string, string)> credentials = new() { ("username", "password") };
        CommonCredentialsIssue commonCredentialsIssue = new(
                pluginId, name, severity, credentials
            );

        Assert.True(commonCredentialsIssue.Credentials == credentials);
        Assert.True(commonCredentialsIssue.PluginId == pluginId);
        Assert.True(commonCredentialsIssue.Name == name);
        Assert.True(commonCredentialsIssue.Severity == severity);
    }
}
