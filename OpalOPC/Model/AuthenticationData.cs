using Opc.Ua;

namespace Model
{
    public class AuthenticationData
    {
        public List<string> usernames = new();
        public List<string> passwords = new();
        public List<CertificateIdentifier> applicationCertificates = new();
        public List<CertificateIdentifier> userCertificates = new();

        public List<(string, string)> loginCredentials = new();
        public List<(string, string)> bruteForceCredentials = new();

        public void AddLoginCredential(string username, string password)
        {
            loginCredentials.Add((username, password));
        }

        public void AddBruteForceCredential(string username, string password)
        {
            bruteForceCredentials.Add((username, password));
        }
    }
}
