using OpalOPCWPF.Models;

namespace OpalOPCWPF.GuiUtil
{
    public class OpenFileDialogUtil(IFilePathUtil filePathUtil)
    {
        private readonly IFilePathUtil _filePathUtil = filePathUtil;

        public OpenFileDialogUtil() : this(new FilePathUtil())
        {
        }

        public string GetFilePathFromUser(IOpenFileDialog openFileDialog, string filter)
        {
            openFileDialog.FileName = string.Empty;
            openFileDialog.Filter = filter;

            if (openFileDialog.ShowDialog() == true && openFileDialog.FileName != string.Empty)
            {
                return _filePathUtil.GetFullPath(openFileDialog.FileName);
            }

            return string.Empty;
        }
    }
}
