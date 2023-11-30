using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpalOPC.WPF.ViewModels;

public partial class ConfigurationViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<string> _privateKeysAndCertificates;
    [ObservableProperty] private ObservableCollection<string> _usersAndPasswords;
    public ConfigurationViewModel()
    {
        PrivateKeysAndCertificates = new ObservableCollection<string>
        {
            "475da948e4ba44d9b5bc31ab4b8006113fd5f538",
        };

        UsersAndPasswords = new ObservableCollection<string>
        {
            "admin:secret",
        };

    }


}
