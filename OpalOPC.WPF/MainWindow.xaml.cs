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
            Process.Start(new ProcessStartInfo(button.Tag.ToString()) { UseShellExecute = true });
        }

        private void DragAndDropTargetsFileButton_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // also allow multiple files?
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // handle (first) file 
                HandleFileOpen(files[0]);


                Button button = (Button)sender;
                StackPanel stackPanel = (StackPanel)button.Content;
                stackPanel.Children[0].Visibility = Visibility.Collapsed;
                stackPanel.Children[2].Visibility = Visibility.Collapsed;
                TextBlock fileNameTextBlock = (TextBlock)stackPanel.Children[1]; // Annahme, dass das Dateiname-TextBlock das zweite Element im StackPanel ist
                fileNameTextBlock.Text = files[0];


            }
        }

        private void HandleFileOpen(string v)
        {
            // TODO: handle here
        }

        private void DragAndDropTargetsFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                Button button = (Button)sender;
                StackPanel stackPanel = (StackPanel)button.Content;
                stackPanel.Children[0].Visibility = Visibility.Collapsed;
                stackPanel.Children[2].Visibility = Visibility.Collapsed;
                TextBlock fileNameTextBlock = (TextBlock)stackPanel.Children[1];
                fileNameTextBlock.Text = System.IO.Path.GetFullPath(fileName);
            }
        }

        private void browseOutputReportFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                // change text of TextBox based on selected File
                outputReportFileTextBox.Text = System.IO.Path.GetFullPath(fileName);
            }
        }


        private void TargetListViewItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: implement target list delete 
        }

    }
}
