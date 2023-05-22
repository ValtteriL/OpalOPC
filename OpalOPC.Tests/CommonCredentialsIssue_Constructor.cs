namespace Tests;
using Model;
using Xunit;

public class CommonCredentialsIssue_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        int pluginId = 0;
        string name = "a";
        double severity = 0;
        string username = "username";
        string password = "password";
        CommonCredentialsIssue commonCredentialsIssue = new CommonCredentialsIssue(
                pluginId, name, severity, username, password
            );

        Assert.True(commonCredentialsIssue != null);
    }

    [Fact]
    public void constructor_SetsProperties()
    {
        int pluginId = 0;
        string name = "a";
        double severity = 0;
        string username = "username";
        string password = "password";
        CommonCredentialsIssue commonCredentialsIssue = new CommonCredentialsIssue(
                pluginId, name, severity, username, password
            );

        Assert.True(commonCredentialsIssue.username == username);
        Assert.True(commonCredentialsIssue.password == password);
        Assert.True(commonCredentialsIssue.PluginId == pluginId);
        Assert.True(commonCredentialsIssue.Name == name);
        Assert.True(commonCredentialsIssue.Severity == severity);
    }
}