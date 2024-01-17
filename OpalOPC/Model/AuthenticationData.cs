using Opc.Ua;

namespace Model
{
    public class AuthenticationData
    {
        public List<CertificateIdentifier> applicationCertificates = [];
        public List<CertificateIdentifier> userCertificates = [];

        public List<(string, string)> loginCredentials = [];
        public List<(string, string)> bruteForceCredentials = [];

        public AuthenticationData()
        {
        }
        public AuthenticationData(ICollection<CertificateIdentifier> applicationCertificates, ICollection<CertificateIdentifier> userCertificates, ICollection<(string, string)> loginCredentials, ICollection<(string, string)> bruteForceCredentials)
        {
            this.applicationCertificates = [.. applicationCertificates];
            this.userCertificates = [.. userCertificates];
            this.loginCredentials = [.. loginCredentials];
            this.bruteForceCredentials = [.. bruteForceCredentials];
        }

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
