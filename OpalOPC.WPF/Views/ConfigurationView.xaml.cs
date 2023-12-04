using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
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
        private readonly OpenFileDialog _openFileDialog;
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
            string path = GetFilePathFromUser(_pemFilesFilter);
            _viewModel.SetApplicationCertificatePath(path);
        }

        private void BrowseApplicationPrivateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            string path = GetFilePathFromUser(_pemFilesFilter);
            _viewModel.SetApplicationPrivateKeyPath(path);
        }

        private string GetFilePathFromUser(string filter)
        {
            string filename = _openFileDialog.FileName;
            _openFileDialog.FileName = string.Empty;
            _openFileDialog.Filter = filter;

            if (_openFileDialog.ShowDialog() == true)
            {
                return System.IO.Path.GetFullPath(filename);
            }

            return string.Empty;
        }

        private void ApplicationCertificatesListItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            // Handle target deletion
            _viewModel.DeleteApplicationCertificate((CertificateIdentifier)btn!.DataContext);
        }

        private void BrowseUserCertificateButton_Click(object sender, RoutedEventArgs e)
        {
            string path = GetFilePathFromUser(_pemFilesFilter);
            _viewModel.SetUserCertificatePath(path);
        }

        private void BrowseUserPrivateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            string path = GetFilePathFromUser(_pemFilesFilter);
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
            string path = GetFilePathFromUser(_allFilesFilter);
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
            string path = GetFilePathFromUser(_allFilesFilter);
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
