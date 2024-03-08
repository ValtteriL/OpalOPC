using Microsoft.Win32;

namespace OpalOPCWPF.Models
{
    public interface IOpenFileDialog
    {
        string FileName { get; set; }
        string Filter { get; set; }
        bool? ShowDialog();
        bool CheckFileExists { get; set; }
    }

    public class MyOpenFileDialog : IOpenFileDialog
    {
        private readonly OpenFileDialog _openFileDialog;

        public MyOpenFileDialog()
        {
            _openFileDialog = new OpenFileDialog();
        }

        public string FileName
        {
            get => _openFileDialog.FileName;
            set => _openFileDialog.FileName = value;
        }

        public string Filter
        {
            get => _openFileDialog.Filter;
            set => _openFileDialog.Filter = value;
        }

        public bool? ShowDialog()
        {
            return _openFileDialog.ShowDialog();
        }

        public bool CheckFileExists
        {
            get => _openFileDialog.CheckFileExists;
            set => _openFileDialog.CheckFileExists = value;
        }
    }

}
