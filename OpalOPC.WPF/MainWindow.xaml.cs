using System.Windows;
using Util;

namespace OpalOPC.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            TelemetryUtil.TrackEvent("GUI started");
            DataContext = new ViewModels.MainWindowViewModel();
            InitializeComponent();
        }

        private void Navbar_About_Click(object sender, RoutedEventArgs e)
        {
            // create new instance of window
            VersionWindow versionWindow = new();

            // open window as a new dialog
            versionWindow.ShowDialog();
        }

    }
}
