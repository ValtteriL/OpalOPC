using System.Security.Cryptography.X509Certificates;
using Opc.Ua;
using Opc.Ua.Security.Certificates;
using static System.Environment;

namespace Util
{
    public interface ISelfSignedCertificateUtil
    {
        public CertificateIdentifier GetCertificate();
    }

    public class SelfSignedCertificateUtil : ISelfSignedCertificateUtil
    {
        private readonly string _subject = "CN=OpalOPC Security Scanner, C=FI, S=Uusimaa, O=Molemmat Oy";
        private readonly string _applicationUri = "urn:opalopc.com";
        private readonly string[] _domainNames = { "opalopc.com" };
        private readonly string _dirName = "OpalOPC";
        private readonly string _certFilename = "cert.pfx";
        private readonly string _certPath;
        private readonly IFileUtil _fileUtil;

        public SelfSignedCertificateUtil() : this(new FileUtil())
        {
        }

        public SelfSignedCertificateUtil(IFileUtil fileUtil)
        {
            _fileUtil = fileUtil;

            // create directory if it doesn't exist - this is cross platform
            // see https://developers.redhat.com/blog/2018/11/07/dotnet-special-folder-api-linux#
            _certPath = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.Create), _dirName, _certFilename);
        }

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
            CertificateIdentifier certificate = new(CertificateBuilder
                .Create(_subject)
                .AddExtension(new X509SubjectAltNameExtension(_applicationUri, _domainNames))
                .SetNotAfter(DateTime.UtcNow.AddYears(1))
                .CreateForRSA());

            _fileUtil.WriteCertificateToDisk(certificate, _certPath);

            return certificate;
        }

        private CertificateIdentifier? GetCertificateFromDisk()
        {
            try
            {
                return _fileUtil.CreateCertificateIdentifierFromPfxFile(_certPath);
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
