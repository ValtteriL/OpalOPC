using System.Security.Cryptography.X509Certificates;
using Opc.Ua;

namespace Model
{
    public class AuthenticationData
    {
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

        public void AddApplicationCertificate(CertificateIdentifier certificate)
        {
            applicationCertificates.Add(certificate);
        }
        public void AddUserCertificate(CertificateIdentifier certificate)
        {
            userCertificates.Add(certificate);
        }
    }
}
