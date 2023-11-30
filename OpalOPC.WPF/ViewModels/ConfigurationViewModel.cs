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
            "Certificate_label : PrivateKey_label 1",
            "Certificate_label : PrivateKey_label 2",
            "Certificate_label : PrivateKey_label 3",
            "Certificate_label : PrivateKey_label 4",
            "Certificate_label : PrivateKey_label 5",
            "Certificate_label : PrivateKey_label 6",
            "Certificate_label : PrivateKey_label 7",
            "Certificate_label : PrivateKey_label 8",
            "Certificate_label : PrivateKey_label 9",
            "Certificate_label : PrivateKey_label 10",
        };

        UsersAndPasswords = new ObservableCollection<string>
        {
            "Username: Password 1",
            "Username: Password 2",
            "Username: Password 3",
            "Username: Password 4",
            "Username: Password 5",
            "Username: Password 6",
            "Username: Password 7",
            "Username: Password 8",
            "Username: Password 9",
            "Username: Password 10",

        };

    }


}
