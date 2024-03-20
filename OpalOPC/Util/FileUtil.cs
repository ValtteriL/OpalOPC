using System.Security.Cryptography.X509Certificates;
using Opc.Ua;
using static System.Environment;

namespace Util
{
    public interface IFileUtil
    {
        public ICollection<string> ReadFileToList(string path);
        public ICollection<string> ReadFileInAppdataToList(string filename);
        public CertificateIdentifier CreateCertificateIdentifierFromPemFile(string certPath, string privkeyPath);
        public Stream Create(string path);
        public void WriteCertificateToFileInAppdata(CertificateIdentifier certificate, string filename);
        public CertificateIdentifier CreateCertificateIdentifierFromPfxFileInAppdata(string filename);
        public bool FileExistsInAppdata(string filename);
        public Task WriteStringToFileInAppdata(string filename, string contents);
        public string OpalOPCDirectoryPath { get; }
    }

    public class FileUtil : IFileUtil
    {

        // create own appdata directory if it doesn't exist - this is cross platform
        // see https://developers.redhat.com/blog/2018/11/07/dotnet-special-folder-api-linux#
        private static readonly string s_dirName = "OpalOPC";
        public readonly string _opalOPCDirectory = Directory.CreateDirectory(Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.Create), s_dirName)).FullName;

        public string OpalOPCDirectoryPath => _opalOPCDirectory;

        public ICollection<string> ReadFileToList(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        public ICollection<string> ReadFileInAppdataToList(string filename)
        {
            return ReadFileToList(Path.Combine(_opalOPCDirectory, filename));
        }

        public CertificateIdentifier CreateCertificateIdentifierFromPemFile(string certPath, string privkeyPath)
        {
            return new CertificateIdentifier(X509Certificate2.CreateFromPemFile(certPath, privkeyPath));
        }

        public Stream Create(string path)
        {
            return File.Create(path);
        }

        public async void WriteCertificateToFileInAppdata(CertificateIdentifier certificate, string filename)
        {
            await File.WriteAllBytesAsync(Path.Combine(_opalOPCDirectory, filename), certificate.Certificate.Export(X509ContentType.Pfx));
        }

        public CertificateIdentifier CreateCertificateIdentifierFromPfxFileInAppdata(string filename)
        {
            return new CertificateIdentifier(new X509Certificate2(Path.Combine(_opalOPCDirectory, filename)));
        }

        public async Task WriteStringToFileInAppdata(string filename, string contents)
        {
            await File.WriteAllTextAsync(Path.Combine(_opalOPCDirectory, filename), contents);
        }

        public bool FileExistsInAppdata(string filename)
        {
            return File.Exists(Path.Combine(_opalOPCDirectory, filename));
        }
    }
}
