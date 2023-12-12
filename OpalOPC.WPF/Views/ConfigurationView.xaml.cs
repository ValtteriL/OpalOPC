using System.Windows;
using System.Windows.Controls;
using OpalOPC.WPF.GuiUtil;
using OpalOPC.WPF.Models;
using OpalOPC.WPF.ViewModels;
using Opc.Ua;

namespace OpalOPC.WPF.Views
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : UserControl
    {
        private readonly ConfigurationViewModel _viewModel;
        private readonly MyOpenFileDialog _openFileDialog;
        private readonly OpenFileDialogUtil _openFileDialogUtil = new();
        private readonly string _pemFilesFilter = "PEM files (*.pem)|*.pem";
        private readonly string _allFilesFilter = "All files (*.*)|*.*";

        public ConfigurationView()
        {
            InitializeComponent();
            DataContext = new ConfigurationViewModel();
            _viewModel = (ConfigurationViewModel)DataContext;

            _openFileDialog = new()
            {
                Filter = "PEM files (*.pem)|*.pem",
            };
        }

        private void BrowseApplicationCertificateButton_Click(object sender, RoutedEventArgs e)
        {
            string path = _openFileDialogUtil.GetFilePathFromUser(_openFileDialog, _pemFilesFilter);
            _viewModel.SetApplicationCertificatePath(path);
        }

        private void BrowseApplicationPrivateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            string path = _openFileDialogUtil.GetFilePathFromUser(_openFileDialog, _pemFilesFilter);
            _viewModel.SetApplicationPrivateKeyPath(path);
        }

        private void ApplicationCertificatesListItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            // Handle target deletion
            _viewModel.DeleteApplicationCertificate((CertificateIdentifier)btn!.DataContext);
        }

        private void BrowseUserCertificateButton_Click(object sender, RoutedEventArgs e)
        {
            string path = _openFileDialogUtil.GetFilePathFromUser(_openFileDialog, _pemFilesFilter);
            _viewModel.SetUserCertificatePath(path);
        }

        private void BrowseUserPrivateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            string path = _openFileDialogUtil.GetFilePathFromUser(_openFileDialog, _pemFilesFilter);
            _viewModel.SetUserPrivateKeyPath(path);
        }

        private void UserCertificatesListItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            // Handle target deletion
            _viewModel.DeleteUserCertificate((CertificateIdentifier)btn!.DataContext);
        }

        private void UsernamePasswordListItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            // Handle target deletion
            _viewModel.DeleteUsernamePassword(((string, string))btn!.DataContext);
        }

        private void DragAndDropUsernamePasswordFileButton_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                _viewModel.AddUsernamesPasswordsFromFile(files[0]);
            }
        }

        private void DragAndDropUsernamePasswordFileButton_Click(object sender, RoutedEventArgs e)
        {
            string path = _openFileDialogUtil.GetFilePathFromUser(_openFileDialog, _allFilesFilter);
            _viewModel.AddUsernamesPasswordsFromFile(path);
        }

        private void DragAndDropBruteUsernamePasswordFileButton_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                _viewModel.AddBruteUsernamesPasswordsFromFile(files[0]);
            }
        }

        private void DragAndDropBruteUsernamePasswordFileButton_Click(object sender, RoutedEventArgs e)
        {
            string path = _openFileDialogUtil.GetFilePathFromUser(_openFileDialog, _allFilesFilter);
            _viewModel.AddBruteUsernamesPasswordsFromFile(path);
        }

        private void BruteUsernamePasswordListItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            // Handle target deletion
            _viewModel.DeleteBruteUsernamePassword(((string, string))btn!.DataContext);
        }
    }
}
