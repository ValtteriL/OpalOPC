using System.Security.Cryptography.X509Certificates;
using Opc.Ua;

namespace Util
{
    public interface ISelfSignedCertificateUtil
    {
        public CertificateIdentifier GetCertificate();
    }

    public class SelfSignedCertificateUtil(IFileUtil fileUtil) : ISelfSignedCertificateUtil
    {
        public static readonly string s_applicationName = "OpalOPC@host";
        private readonly string _subject = "CN=OpalOPC Security Scanner, C=FI, S=Uusimaa, O=Molemmat Oy";
        public static readonly string s_applicationUri = "urn:OPCUA:OpalOPC";
        private readonly string[] _domainNames = ["opalopc.com"];
        private readonly string _certFilename = "cert.pfx";


        public CertificateIdentifier GetCertificate()
        {
            CertificateIdentifier? certificate = GetCertificateFromDisk();

            if (certificate != null && IsCertificateValid(certificate))
            {
                return certificate;
            }
            else
            {
                return GenerateNewCertificate();
            }
        }

        private CertificateIdentifier GenerateNewCertificate()
        {
            CertificateIdentifier certificate = new(CertificateFactory.CreateCertificate(
                s_applicationUri,
                s_applicationName,
                _subject,
                _domainNames)
                .SetNotAfter(DateTime.UtcNow.AddYears(1)).CreateForRSA());

            fileUtil.WriteCertificateToFileInAppdata(certificate, _certFilename);

            return certificate;
        }

        private CertificateIdentifier? GetCertificateFromDisk()
        {
            try
            {
                return fileUtil.CreateCertificateIdentifierFromPfxFileInAppdata(_certFilename);
            }
            catch
            {
                return null;
            }
        }

        private static bool IsCertificateValid(CertificateIdentifier certificate)
        {
            // check only if certificate is still valid
            DateTime time = DateTime.Now;
            X509Certificate2 c = certificate.Certificate;
            return time > c.NotBefore && time < certificate.Certificate.NotAfter;
        }
    }
}
