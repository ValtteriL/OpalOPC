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

        public ConfigurationView()
        {
            InitializeComponent();
            DataContext = new ConfigurationViewModel();
            _viewModel = (ConfigurationViewModel)DataContext;

            _openFileDialog = new()
            {
                Filter = "All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };
        }

        private void BrowseApplicationCertificateButton_Click(object sender, RoutedEventArgs e)
        {
            string path = GetFilePathFromUser();
            _viewModel.SetApplicationCertificatePath(path);
        }

        private void BrowseApplicationPrivateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            string path = GetFilePathFromUser();
            _viewModel.SetApplicationPrivateKeyPath(path);
        }

        private string GetFilePathFromUser()
        {
            if (_openFileDialog.ShowDialog() == true)
            {
                return System.IO.Path.GetFullPath(_openFileDialog.FileName);
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
            string path = GetFilePathFromUser();
            _viewModel.SetUserCertificatePath(path);
        }

        private void BrowseUserPrivateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            string path = GetFilePathFromUser();
            _viewModel.SetUserPrivateKeyPath(path);
        }

        private void UserCertificatesListItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            // Handle target deletion
            _viewModel.DeleteUserCertificate((CertificateIdentifier)btn!.DataContext);
        }
    }
}
