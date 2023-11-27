using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace Util
{
    public interface IFileUtil
    {
        public ICollection<string> ReadFileToList(string path);
        public X509Certificate2 CreateFromPemFile(string certPath, string privkeyPath);
    }

    public class FileUtil : IFileUtil
    {
        public ICollection<string> ReadFileToList(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        public X509Certificate2 CreateFromPemFile(string certPath, string privkeyPath)
        {
            return X509Certificate2.CreateFromPemFile(certPath, privkeyPath);
        }
    }
}
