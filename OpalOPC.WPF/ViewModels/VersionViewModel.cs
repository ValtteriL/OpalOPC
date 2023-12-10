using System.Diagnostics;
using System.Windows.Automation.Provider;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpalOPC.WPF.ViewModels;

public partial class VersionViewModel : ObservableObject
{
    [ObservableProperty] private string _version;
    public VersionViewModel()
    {
        Version = $"{Util.VersionUtil.AppAssemblyVersion}";
    }

    [RelayCommand]
    public static void Navigate(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
