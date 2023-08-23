using Controller;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Check for updates when MainWindow has loaded
            ILogger logger = new Logger<MainWindow>(new NullLoggerFactory());
            VersionCheckController versionCheckController = new VersionCheckController(logger);
            await Task.Run(() => { versionCheckController.CheckVersion(); }); // run in background thread
            if (!versionCheckController.IsUpToDate)
            {
                UpdateWindow updateWindow = new UpdateWindow();
                updateWindow.ShowDialog();
            }

        }


        private void Navbar_About_Click(object sender, RoutedEventArgs e)
        {
            // create new instance of window
            VersionWindow versionWindow = new VersionWindow();

            // open window as a new dialog
            versionWindow.ShowDialog();
        }

        private void Navbar_Help_Click(object sender, RoutedEventArgs e)
        {
            
            Button button = (Button)sender;
            Process.Start(new ProcessStartInfo(button.Tag.ToString()!) { UseShellExecute = true });
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
            MainWindowViewModel viewModel = (MainWindowViewModel)this.DataContext;
            viewModel.AddTargetsFromFile(path);
        }

        private void DragAndDropTargetsFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                // Handle target file
                MainWindowViewModel viewModel = (MainWindowViewModel)this.DataContext;
                viewModel.AddTargetsFromFile(System.IO.Path.GetFullPath(fileName));
            }
        }

        private void BrowseOutputReportFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                string path = System.IO.Path.GetFullPath(openFileDialog.FileName);

                // Handle output location selection
                MainWindowViewModel viewModel = (MainWindowViewModel)this.DataContext;
                viewModel.SetOutputFileLocation(path);
            }
        }


        private void TargetListViewItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            // Handle target deletion
            MainWindowViewModel viewModel = (MainWindowViewModel)this.DataContext;
            viewModel.DeleteTarget((string)btn!.DataContext);
        }


    }
}
