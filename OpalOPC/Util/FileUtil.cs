using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua;

namespace Util
{
    public interface IFileUtil
    {
        public ICollection<string> ReadFileToList(string path);
        public CertificateIdentifier CreateCertificateIdentifierFromPemFile(string certPath, string privkeyPath);
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
    }
}
