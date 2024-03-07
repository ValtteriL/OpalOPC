using OpalOPCWPF.Models;

namespace OpalOPCWPF.GuiUtil
{
    public class OpenFileDialogUtil
    {
        private readonly IFilePathUtil _filePathUtil;

        public OpenFileDialogUtil() : this(new FilePathUtil())
        {
        }
        public OpenFileDialogUtil(IFilePathUtil filePathUtil)
        {
            _filePathUtil = filePathUtil;
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
