using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OpalOPC.WPF.GuiUtil;
using OpalOPC.WPF.Logger;
using Opc.Ua;
using Org.BouncyCastle.Asn1.X509;
using Util;

namespace OpalOPC.WPF.ViewModels;

public partial class ConfigurationViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddApplicationCertificateCommand))]
    private string _applicationCertificatePath = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddApplicationCertificateCommand))]
    private string _applicationPrivateKeyPath = string.Empty;

    [ObservableProperty] private ObservableCollection<CertificateIdentifier> _applicationCertificateIdentifiers = new();

    private bool ApplicationCertificatePrivateKeyPathsSet()
    {
        return !string.IsNullOrEmpty(ApplicationCertificatePath) && !string.IsNullOrEmpty(ApplicationPrivateKeyPath);
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddUserCertificateCommand))]
    private string _userCertificatePath = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddUserCertificateCommand))]
    private string _userPrivateKeyPath = string.Empty;

    [ObservableProperty] private ObservableCollection<CertificateIdentifier> _userCertificateIdentifiers = new();

    private bool UserCertificatePrivateKeyPathsSet()
    {
        return !string.IsNullOrEmpty(UserCertificatePath) && !string.IsNullOrEmpty(UserPrivateKeyPath);
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddUsernamePasswordCommand))]
    private string _username = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddUsernamePasswordCommand))]
    private string _password = string.Empty;

    private bool UsernamePasswordSet()
    {
        return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
    }


    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddBruteUsernamePasswordCommand))]
    private string _bruteUsername = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddBruteUsernamePasswordCommand))]
    private string _brutePassword = string.Empty;

    private bool BruteUsernamePasswordSet()
    {
        return !string.IsNullOrEmpty(BruteUsername) && !string.IsNullOrEmpty(BrutePassword);
    }

    private readonly IFileUtil _fileUtil;
    private readonly IMessageBoxUtil _messageBoxUtil;

    [ObservableProperty] private ObservableCollection<string> _privateKeysAndCertificates;
    [ObservableProperty] private ObservableCollection<(string, string)> _usernamesAndPasswords = new ObservableCollection<(string, string)>();
    [ObservableProperty] private ObservableCollection<(string, string)> _bruteUsernamesAndPasswords = new ObservableCollection<(string, string)>();
    public ConfigurationViewModel()
    {
        PrivateKeysAndCertificates = new ObservableCollection<string>
        {
            "475da948e4ba44d9b5bc31ab4b8006113fd5f538",
        };

        _fileUtil = new FileUtil();
        _messageBoxUtil = new MessageBoxUtil();
    }

    public ConfigurationViewModel(IFileUtil fileUtil, IMessageBoxUtil messageBoxUtil) : this()
    {
        _fileUtil = fileUtil;
        _messageBoxUtil = messageBoxUtil;
    }

    public void SetApplicationCertificatePath(string fullPath)
    {
        ApplicationCertificatePath = fullPath;
    }

    public void SetApplicationPrivateKeyPath(string fullPath)
    {
        ApplicationPrivateKeyPath = fullPath;
    }

    [RelayCommand(CanExecute = nameof(ApplicationCertificatePrivateKeyPathsSet))]
    private void AddApplicationCertificate()
    {
        // read certificate and private key into CertificateIdentifier
        // make messagebox on exception
        // add certificateIdentifier to ApplicationCertificateIdentifiers
        // clear ApplicationCertificatePath and ApplicationPrivateKeyPath
        try
        {
            CertificateIdentifier certificateIdentifier = _fileUtil.CreateCertificateIdentifierFromPemFile(ApplicationCertificatePath, ApplicationPrivateKeyPath);
            ApplicationCertificateIdentifiers.Add(certificateIdentifier);
            SetApplicationCertificatePath(string.Empty);
            SetApplicationPrivateKeyPath(string.Empty);
        }
        catch (Exception e)
        {
            _messageBoxUtil.Show(e.Message);
        }
    }

    public void SetUserCertificatePath(string fullPath)
    {
        UserCertificatePath = fullPath;
    }

    public void SetUserPrivateKeyPath(string fullPath)
    {
        UserPrivateKeyPath = fullPath;
    }

    [RelayCommand(CanExecute = nameof(UserCertificatePrivateKeyPathsSet))]
    private void AddUserCertificate()
    {
        // read certificate and private key into CertificateIdentifier
        // make messagebox on exception
        // add certificateIdentifier to ApplicationCertificateIdentifiers
        // clear UserCertificatePath and UserPrivateKeyPath
        try
        {
            CertificateIdentifier certificateIdentifier = _fileUtil.CreateCertificateIdentifierFromPemFile(UserCertificatePath, UserPrivateKeyPath);
            UserCertificateIdentifiers.Add(certificateIdentifier);
            SetUserCertificatePath(string.Empty);
            SetUserPrivateKeyPath(string.Empty);
        }
        catch (Exception e)
        {
            _messageBoxUtil.Show(e.Message);
        }
    }

    public void DeleteApplicationCertificate(CertificateIdentifier certificateIdentifier)
    {
        ApplicationCertificateIdentifiers.Remove(certificateIdentifier);
    }

    public void DeleteUserCertificate(CertificateIdentifier certificateIdentifier)
    {
        UserCertificateIdentifiers.Remove(certificateIdentifier);
    }

    [RelayCommand(CanExecute = nameof(UsernamePasswordSet))]
    private void AddUsernamePassword()
    {
        UsernamesAndPasswords.Add((Username,Password));
        Username = string.Empty;
        Password = string.Empty;
    }

    public void DeleteUsernamePassword((string, string) usernamePassword)
    {
        UsernamesAndPasswords.Remove(usernamePassword);
    }

    public void AddUsernamesPasswordsFromFile(string path)
    {
        try
        {
            AddUsernamesPasswordsFromFileToCollection(path, UsernamesAndPasswords);
        }
        catch (Exception e)
        {
            _messageBoxUtil.Show(e.Message);
        }
    }

    [RelayCommand(CanExecute = nameof(BruteUsernamePasswordSet))]
    private void AddBruteUsernamePassword()
    {
        BruteUsernamesAndPasswords.Add((BruteUsername, BrutePassword));
        BruteUsername = string.Empty;
        BrutePassword = string.Empty;
    }

    public void AddBruteUsernamesPasswordsFromFile(string path)
    {
        try
        {
            AddUsernamesPasswordsFromFileToCollection(path, BruteUsernamesAndPasswords);
        }
        catch (Exception e)
        {
            _messageBoxUtil.Show(e.Message);
        }
    }

    private void AddUsernamesPasswordsFromFileToCollection(string path, ICollection<(string, string)> collection)
    {
        _fileUtil.ReadFileToList(path).ToList().ForEach(line =>
        {
            string[] split = line.Split(':');
            collection.Add((split[0], split[1]));
        });
    }

    public void DeleteBruteUsernamePassword((string, string) usernamePassword)
    {
        BruteUsernamesAndPasswords.Remove(usernamePassword);
    }

}
