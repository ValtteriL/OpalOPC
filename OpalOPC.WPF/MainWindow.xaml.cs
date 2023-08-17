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
            InitializeComponent();

            this.Title = $"OpalOPC {Util.VersionUtil.AppAssemblyVersion}";

            // create dummy entries for targets list
            targetsList.ItemsSource = new string[]
            {
                "193.23.54.9",
                "http://szeyuvcexemple.com",
                "http://testet.com",
                "193.23.54.10",
                "http://test.com",
                "193.23.54.10",
                "193.23.54.10",
                "http://szeyuvcexemple.com",
                "193.23.54.10",
                "http://test.com",
                "http://szeyuvcexemple.com",
                "http://szeyuvcexemple.com",
                "http://szeyuvcexemple.com",
                "http://szeyuvcexemple.com",
                "http://szeyuvcexemple.com",
            };

            // set dummy text log
            LogTextBox.Text = "2023-08-01 10:00:01 - INFO: Application started.\r\n2023-08-01 10:01:15 - WARNING: Disk space is running low.\r\n2023-08-01 10:05:42 - ERROR: Connection to database failed.\r\n2023-08-01 10:10:23 - INFO: User logged in.\r\n2023-08-01 10:11:56 - DEBUG: Entering main loop.\r\n2023-08-01 10:15:30 - INFO: Data synchronization completed.\r\n2023-08-01 10:20:09 - WARNING: CPU temperature above normal range.\r\n2023-08-01 10:25:17 - ERROR: Critical system component failed.\r\n2023-08-01 10:30:45 - INFO: Backup process initiated.\r\n2023-08-01 10:35:02 - DEBUG: Processing records.\r\n2023-08-01 10:40:21 - INFO: Task completed successfully.\r\n2023-08-01 10:45:37 - WARNING: Network connection unstable.\r\n2023-08-01 10:50:12 - ERROR: File not found: report.docx\r\n2023-08-01 10:55:28 - INFO: Email sent to user@example.com.\r\n2023-08-01 11:00:04 - DEBUG: Generating report.\r\n2023-08-01 11:05:59 - INFO: Application update available.\r\n2023-08-01 11:10:44 - WARNING: Low battery level.\r\n2023-08-01 11:15:18 - ERROR: Printer jam detected.\r\n2023-08-01 11:20:37 - INFO: System maintenance in progress.\r\n2023-08-01 11:25:52 - DEBUG: UI elements initialized.\r\n2023-08-01 11:30:28 - INFO: User preferences saved.\r\n2023-08-01 11:35:07 - WARNING: External device disconnected abruptly.\r\n2023-08-01 11:40:45 - ERROR: Application crash detected.\r\n2023-08-01 11:45:09 - INFO: New user registered.\r\n2023-08-01 11:50:33 - DEBUG: Processing input data.\r\n2023-08-01 11:55:21 - INFO: Report generated and sent.\r\n2023-08-01 12:00:00 - WARNING: Security breach attempt blocked.\r\n2023-08-01 12:05:42 - ERROR: Database corruption detected.\r\n2023-08-01 12:10:28 - INFO: Configuration settings updated.\r\n2023-08-01 12:15:19 - DEBUG: Application performance optimization.\r\n2023-08-01 12:20:53 - INFO: Task queue cleared.\r\n2023-08-01 12:25:36 - WARNING: Remote server connection lost.\r\n2023-08-01 12:30:05 - ERROR: Data integrity check failed.\r\n2023-08-01 12:35:22 - INFO: User profile picture updated.\r\n2023-08-01 12:40:18 - DEBUG: Searching for records.\r\n2023-08-01 12:45:03 - INFO: Notification sent to all users.\r\n2023-08-01 12:50:17 - WARNING: Insufficient memory for operation.\r\n2023-08-01 12:55:31 - ERROR: Unable to establish secure connection.\r\n2023-08-01 13:00:02 - INFO: System restart completed.\r\n2023-08-01 13:05:14 - DEBUG: Processing user requests.\r\n2023-08-01 13:10:20 - INFO: Log files archived.\r\n2023-08-01 13:15:05 - WARNING: Resource usage exceeds limits.\r\n2023-08-01 13:20:08 - ERROR: Invalid input data received.\r\n2023-08-01 13:25:23 - INFO: Automated backup executed.\r\n2023-08-01 13:30:39 - DEBUG: Data validation in progress.\r\n2023-08-01 13:35:11 - INFO: Scheduled maintenance started.\r\n2023-08-01 13:40:28 - WARNING: Network latency spike detected.\r\n2023-08-01 13:45:06 - ERROR: Payment processing failed.\r\n2023-08-01 13:50:14 - INFO: User session timeout extended.\r\n2023-08-01 13:55:22 - DEBUG: User activity tracking enabled.\r\n2023-08-01 14:00:04 - INFO: Application update installed.\r\n2023-08-01 14:05:18 - WARNING: Disk fragmentation detected.\r\n2023-08-01 14:10:35 - ERROR: Unauthorized access attempt.\r\n2023-08-01 14:15:09 - INFO: Database maintenance completed.\r\n2023-08-01 14:20:25 - DEBUG: Performance metrics collected.\r\n2023-08-01 14:25:07 - INFO: Data export in progress.\r\n2023-08-01 14:30:11 - WARNING: Server load at critical level.\r\n2023-08-01 14:35:16 - ERROR: Service unavailable.\r\n2023-08-01 14:40:29 - INFO: Server restart initiated.\r\n2023-08-01 14:45:15 - DEBUG: Code review session scheduled.\r\n2023-08-01 14:50:28 - INFO: New feature development started.\r\n2023-08-01 14:55:19 - WARNING: Server security patch required.\r\n2023-08-01 15:00:03 - ERROR: Software license expired.\r\n2023-08-01 15:05:14 - INFO: System backup successful.\r\n2023-08-01 15:10:22 - DEBUG: Database optimization process.\r\n2023-08-01 15:15:08 - INFO: User preferences reset.\r\n2023-08-01 15:20:26 - WARNING: Unauthorized file access detected.\r\n2023-08-01 15:25:07 - ERROR: Data corruption detected.\r\n2023-08-01 15:30:14 - INFO: User account locked for security.\r\n2023-08-01 15:35:20 - DEBUG: Bug fixing and code refactoring.\r\n2023-08-01 15:40:15 - INFO: System update scheduled.\r\n2023-08-01 15:45:23 - WARNING: Low disk space on backup drive.\r\n2023-08-01 15:50:09 - ERROR: Network connection lost.\r\n2023-08-01 15:55:17 - INFO: User access logs analyzed.\r\n2023-08-01 16:00:01 - DEBUG: API integration testing.\r\n2023-08-01 16:05:12 - INFO: Application log rotation completed.\r\n2023-08-01 16:10:28 - WARNING: Database backup overdue.\r\n2023-08-01 16:15:05 - ERROR: Server hardware failure.\r\n2023-08-01 16:20:25 - INFO: Data migration in progress.\r\n2023-08-01 16:25:03 - DEBUG: Performance profiling started.\r\n2023-08-01 16:30:29 - INFO: User feedback survey launched.\r\n2023-08-01 16:35:19 - WARNING: Critical security vulnerability.\r\n2023-08-01 16:40:12 - ERROR: Application crash detected.\r\n2023-08-01 16:45:28 - INFO: Daily system health check completed.\r\n2023-08-01 16:50:13 - DEBUG: Code review feedback addressed.\r\n2023-08-01 16:55:23 - INFO: Remote server synchronization.\r\n2023-08-01 17:00:00 - WARNING: Database performance degradation.\r\n2023-08-01 17:05:16 - ERROR: Unauthorized access attempt.\r\n2023-08-01 17:10:22 - INFO: Weekly maintenance scheduled.\r\n2023-08-01 17:15:09 - DEBUG: Logging enhancements implemented.\r\n2023-08-01 17:20:27 - INFO: Data validation completed.\r\n2023-08-01 17:25:05 - WARNING: Disk failure imminent.\r\n2023-08-01 17:30:14 - ERROR: Server overload detected.\r\n2023-08-01 17:35:21 - INFO: User account recovery initiated.\r\n2023-08-01 17:40:16 - DEBUG: System resource allocation adjustments.\r\n2023-08-01 17:45:24 - INFO: Application configuration updated.\r\n2023-08-01 17:50:11 - WARNING: Network latency above acceptable levels.\r\n2023-08-01 17:55:18 - ERROR: Unable to establish secure connection.\r\n2023-08-01 18:00:02 - INFO: User manual updated.\r\n2023-08-01 18:05:15 - DEBUG: Performance optimization strategies.\r\n2023-08-01 18:10:28 - INFO: Data encryption audit completed.\r\n2023-08-01 18:15:08 - WARNING: Firewall rule violation detected.\r\n2023-08-01 18:20:26 - ERROR: System crash due to memory leak.\r\n2023-08-01 18:25:07 - INFO: Automated reports generation.\r\n2023-08-01 18:30:10 - DEBUG: Server logs analysis.\r\n2023-08-01 18:35:17 - INFO: User activity tracking report generated.\r\n2023-08-01 18:40:15 - WARNING: Critical software update required.\r\n2023-08-01 18:45:22 - ERROR: Database replication failure.\r\n2023-08-01 18:50:10 - INFO: Data backup verification completed.\r\n2023-08-01 18:55:18 - DEBUG: Application profiling for optimization.\r\n2023-08-01 19:00:03 - INFO: Server maintenance mode activated.\r\n2023-08-01 19:05:14 - WARNING: Disk fragmentation detected.\r\n2023-08-01 19:10:28 - ERROR: Server security breach detected.\r\n2023-08-01 19:15:07 - INFO: User password reset initiated.\r\n2023-08-01 19:20:26 - DEBUG: Log file compression in progress.\r\n2023-08-01 19:25:05 - INFO: Real-time monitoring activated.\r\n2023-08-01 19:30:09 - WARNING: Unauthorized access attempt.\r\n2023-08-01 19:35:17 - ERROR: Critical application error.\r\n2023-08-01 19:40:14 - INFO: Performance benchmarking started.\r\n2023-08-01 19:45:23 - DEBUG: Continuous integration testing.\r\n2023-08-01 19:50:12 - INFO: Data consistency check completed.\r\n2023-08-01 19:55:19 - WARNING: Server disk space reaching capacity.\r\n2023-08-01 20:00:01 - ERROR: Application instability detected.\r\n2023-08-01 20:05:15 - INFO: Configuration backup successful.\r\n2023-08-01 20:10:29 - DEBUG: Load testing of new feature.\r\n2023-08-01 20:15:06 - INFO: Security audit in progress.\r\n2023-08-01 20:20:28 - WARNING: Network connectivity issues.\r\n2023-08-01 20:25:06 - ERROR: Data corruption detected.\r\n2023-08-01 20:30:11 - INFO: User profile updated.\r\n2023-08-01 20:35:18 - DEBUG: Application log analysis.\r\n2023-08-01 20:40:15 - INFO: New API endpoint created.\r\n2023-08-01 20:45:24 - WARNING: Abnormal CPU usage detected.\r\n2023-08-01 20:50:13 - ERROR: Database query optimization required.\r\n2023-08-01 20:55:20 - INFO: Remote server backup initiated.\r\n2023-08-01 21:00:02 - DEBUG: Code review feedback incorporated.\r\n2023-08-01 21:05:15 - INFO: Data synchronization completed.\r\n2023-08-01 21:10:28 - WARNING: Disk space utilization exceeded threshold.\r\n2023-08-01 21:15:08 - ERROR: Application crash during high load.\r\n2023-08-01 21:20:26 - INFO: User access logs archived.\r\n2023-08-01 21:25:07 - DEBUG: Performance profiling results.\r\n2023-08-01 21:30:12 - INFO: Backup process initiated.\r\n2023-08-01 21:35:19 - WARNING: Server resource contention detected.\r\n2023-08-01 21:40:16 - ERROR: Data integrity violation.\r\n2023-08-01 21:45:25 - INFO: User feedback analysis in progress.\r\n2023-08-01 21:50:14 - DEBUG: UI responsiveness improvements.\r\n2023-08-01 21:55:21 - INFO: Weekly report generation.";
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


        private void Navbar_Scan_Click(object sender, RoutedEventArgs e)
        {
            // TODO: implement scan click
        }

        private void Navbar_Help_Click(object sender, RoutedEventArgs e)
        {
            
            Button button = (Button)sender;
            Process.Start(new ProcessStartInfo(button.Tag.ToString()) { UseShellExecute = true });
        }

        private void AddNewTarget_Click(object sender, RoutedEventArgs e)
        {
            // TODO: implement add new target click
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

        private void NormalVerbosityRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // TODO: implement normal verbosity checked
        }

        private void DebugVerbosityRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // TODO: implement normal verbosity checked
        }

        private void TraceVerbosityRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // TODO: implement normal verbosity checked
        }

       
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: start button click
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: stop button click
        }

        private void OpenReportButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: openreport button click
        }

    }
}
