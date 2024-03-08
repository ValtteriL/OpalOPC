using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpalOPCWPF.GuiUtil
{
    public interface IFilePathUtil
    {
        public string GetFullPath(string path);
    }
    public class FilePathUtil : IFilePathUtil
    {
        public string GetFullPath(string path)
        {
            return System.IO.Path.GetFullPath(path);
        }
    }
}
