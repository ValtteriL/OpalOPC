using Opc.Ua;
using Plugin;

namespace Model
{
    public class CredentialsIssue : Issue
    {
        public ICollection<(string, string)> UsernamesPasswords { get; } = [];
        public ICollection<CertificateIdentifier> UserCertificates { get; } = [];

        public CredentialsIssue(PluginId pluginId, string name, double severity, ICollection<(string, string)> usernamesPasswords, ICollection<CertificateIdentifier> userCertificates) : base(pluginId, name, severity)
        {
            UsernamesPasswords = usernamesPasswords;
            UserCertificates = userCertificates;
        }

        public CredentialsIssue(PluginId pluginId, string name, double severity, ICollection<(string, string)> usernamesPasswords) : base(pluginId, name, severity)
        {
            UsernamesPasswords = usernamesPasswords;
        }
    }
}
