using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Controller;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Win32;

namespace OpalOPC.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }

        private void Navbar_About_Click(object sender, RoutedEventArgs e)
        {
            // create new instance of window
            VersionWindow versionWindow = new();

            // open window as a new dialog
            versionWindow.ShowDialog();
        }

        private void DragAndDropTargetsFileButton_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                HandleFileOpen(files[0]);
            }
        }

        private void HandleFileOpen(string path)
        {
            // Handle target file
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            viewModel.AddTargetsFromFile(path);
        }

        private void DragAndDropTargetsFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                // Handle target file
                MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
                viewModel.AddTargetsFromFile(System.IO.Path.GetFullPath(fileName));
            }
        }

        private void BrowseOutputReportFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                CheckFileExists = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string path = System.IO.Path.GetFullPath(openFileDialog.FileName);

                // Handle output location selection
                MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
                viewModel.SetOutputFileLocation(path);
            }
        }


        private void TargetListViewItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            // Handle target deletion
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            viewModel.DeleteTarget((string)btn!.DataContext);
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // populate target textbox with selected target
            TextBlock? block = sender as TextBlock;

            // Handle target deletion
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            viewModel.SetTargetToAdd((string)block!.DataContext);
        }
    }
}
