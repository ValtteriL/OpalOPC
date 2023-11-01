using System.Diagnostics;
using System.Windows;

namespace OpalOPC.WPF
{
    /// <summary>
    /// Interaktionslogik für VersionWindow.xaml
    /// </summary>
    public partial class VersionWindow : Window
    {
        public VersionWindow()
        {
            InitializeComponent();
            VersionTextBlock.Text = $" Version : {Util.VersionUtil.AppAssemblyVersion}";
        }

        private void Browse_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
