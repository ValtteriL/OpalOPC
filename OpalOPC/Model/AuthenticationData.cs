using Opc.Ua;

namespace Model
{
    public class AuthenticationData
    {
        public List<CertificateIdentifier> applicationCertificates = new();
        public List<CertificateIdentifier> userCertificates = new();

        public List<(string, string)> loginCredentials = new();
        public List<(string, string)> bruteForceCredentials = new();

        public AuthenticationData()
        {
        }
        public AuthenticationData(ICollection<CertificateIdentifier> applicationCertificates, ICollection<CertificateIdentifier> userCertificates, ICollection<(string, string)> loginCredentials, ICollection<(string, string)> bruteForceCredentials)
        {
            this.applicationCertificates = applicationCertificates.ToList();
            this.userCertificates = userCertificates.ToList();
            this.loginCredentials = loginCredentials.ToList();
            this.bruteForceCredentials = bruteForceCredentials.ToList();
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
