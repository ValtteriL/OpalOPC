using System.Security.Cryptography.X509Certificates;
using Opc.Ua;

namespace Util
{
    public interface IFileUtil
    {
        public ICollection<string> ReadFileToList(string path);
        public CertificateIdentifier CreateCertificateIdentifierFromPemFile(string certPath, string privkeyPath);
        public Stream Create(string path);
        public void WriteCertificateToDisk(CertificateIdentifier certificate, string path);
        public CertificateIdentifier CreateCertificateIdentifierFromPfxFile(string path);
        public bool FileExists(string path);
    }

    public class FileUtil : IFileUtil
    {
        public ICollection<string> ReadFileToList(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        public CertificateIdentifier CreateCertificateIdentifierFromPemFile(string certPath, string privkeyPath)
        {
            return new CertificateIdentifier(X509Certificate2.CreateFromPemFile(certPath, privkeyPath));
        }

        public Stream Create(string path)
        {
            return File.Create(path);
        }

        public async void WriteCertificateToDisk(CertificateIdentifier certificate, string path)
        {
            await File.WriteAllBytesAsync(path, certificate.Certificate.Export(X509ContentType.Pfx));
        }

        public CertificateIdentifier CreateCertificateIdentifierFromPfxFile(string path)
        {
            return new CertificateIdentifier(new X509Certificate2(path));
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
