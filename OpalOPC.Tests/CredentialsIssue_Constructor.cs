using Model;
using Opc.Ua;
using Plugin;
using Xunit;

namespace Tests;
public class CredentialsIssue_Constructor
{
    [Fact]
    public void constructor_SetsProperties()
    {
        PluginId pluginId = PluginId.ServerCertificate;
        string name = "a";
        double severity = 0;
        List<(string, string)> usernamesPasswords = [("username", "password")];
        List<CertificateIdentifier> userCertificates = [new CertificateIdentifier()];
        CredentialsIssue CredentialsIssue = new(
                pluginId, name, severity, usernamesPasswords, userCertificates
            );

        Assert.True(CredentialsIssue.UsernamesPasswords == usernamesPasswords);
        Assert.True(CredentialsIssue.UserCertificates == userCertificates);
        Assert.True(CredentialsIssue.PluginId == pluginId);
        Assert.True(CredentialsIssue.Name == name);
        Assert.True(CredentialsIssue.Severity == severity);
    }
}
