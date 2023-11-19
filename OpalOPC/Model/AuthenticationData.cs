using Opc.Ua;

namespace Model
{
    public class AuthenticationData
    {
        public List<string> usernames = new();
        public List<string> passwords = new();
        public List<CertificateIdentifier> applicationCertificates = new();
        public List<CertificateIdentifier> userCertificates = new();
    }
}
