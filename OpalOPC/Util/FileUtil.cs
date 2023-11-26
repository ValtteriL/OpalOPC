using System.Collections;

namespace Util
{
    public interface IFileUtil
    {
        public ICollection<string> ReadFileToList(string path);
    }

    public class FileUtil : IFileUtil
    {
        public ICollection<string> ReadFileToList(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }
    }
}
