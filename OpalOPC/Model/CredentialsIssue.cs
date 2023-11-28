using Opc.Ua;

namespace Model
{
    public class CredentialsIssue : Issue
    {
        public ICollection<(string, string)> UsernamesPasswords { get; } = new List<(string, string)>();
        public ICollection<CertificateIdentifier> UserCertificates { get; } = new List<CertificateIdentifier>();

        // parameterless constructor for XML serializer
        internal CredentialsIssue()
        { }

        public CredentialsIssue(int pluginId, string name, double severity, ICollection<(string, string)> usernamesPasswords, ICollection<CertificateIdentifier> userCertificates) : base(pluginId, name, severity)
        {
            UsernamesPasswords = usernamesPasswords;
            UserCertificates = userCertificates;
        }

        public CredentialsIssue(int pluginId, string name, double severity, ICollection<(string, string)> usernamesPasswords) : base(pluginId, name, severity)
        {
            UsernamesPasswords = usernamesPasswords;
        }
    }
}
