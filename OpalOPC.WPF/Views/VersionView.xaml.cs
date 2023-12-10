using System.Windows.Controls;
using OpalOPC.WPF.ViewModels;

namespace OpalOPC.WPF.Views
{
    /// <summary>
    /// Interaction logic for VersionView.xaml
    /// </summary>
    public partial class VersionView : UserControl
    {
        public VersionView()
        {
            InitializeComponent();
            DataContext = new VersionViewModel();
        }
    }
}
