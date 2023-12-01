using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpalOPC.WPF.GuiUtil;
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

    private readonly IFileUtil _fileUtil;
    private readonly IMessageBoxUtil _messageBoxUtil;

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
            X509Certificate2 x509Certificate2 = _fileUtil.CreateFromPemFile(ApplicationCertificatePath, ApplicationPrivateKeyPath);
            CertificateIdentifier certificateIdentifier = new(x509Certificate2);
            ApplicationCertificateIdentifiers.Add(certificateIdentifier);
            SetApplicationCertificatePath(string.Empty);
            SetApplicationPrivateKeyPath(string.Empty);
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

}
