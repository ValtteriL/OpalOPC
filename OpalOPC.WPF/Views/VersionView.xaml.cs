using System.Windows.Controls;
using OpalOPCWPF.ViewModels;

namespace OpalOPCWPF.Views
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
