using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

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
        }

        private void Browse_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // TODO: Implement link Handling here
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
