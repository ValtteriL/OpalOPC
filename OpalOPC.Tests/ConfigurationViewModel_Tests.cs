#if BUILT_FOR_WINDOWS
using Moq;
using OpalOPC.WPF.GuiUtil;
using OpalOPC.WPF.ViewModels;
using Opc.Ua;
using Opc.Ua.Security.Certificates;
using Util;
using Xunit;

namespace Tests;
public class ConfigurationViewModel_Tests
{
    private readonly Mock<IFileUtil> _fileUtilMock = new();
    private readonly Mock<IMessageBoxUtil> _messageBoxUtilMock = new();
    private readonly CertificateIdentifier _certificate;

    public ConfigurationViewModel_Tests()
    {
        _certificate = new CertificateIdentifier(CertificateBuilder
                    .Create("CN=Test")
                    .AddExtension(
                        new X509SubjectAltNameExtension("urn:opalopc.com:host",
                        new string[] { "host", "host.opalopc.com", "192.168.1.100" }))
                    .CreateForRSA());
    }


    // initial values when starting
    [Fact]
    public void Constructor()
    {
        ConfigurationViewModel model = new();

        Assert.True(model.ApplicationCertificatePath != null);
        Assert.True(model.ApplicationPrivateKeyPath != null);
        Assert.True(model.ApplicationCertificateIdentifiers != null);
        Assert.True(model.UserCertificatePath != null);
        Assert.True(model.UserPrivateKeyPath != null);
        Assert.True(model.UserCertificateIdentifiers != null);
    }

    // add application certificate
    [Fact]
    public void AddApplicationCertificate()
    {
        // Arrange
        _fileUtilMock.Setup(_fileUtilMock => _fileUtilMock.CreateCertificateIdentifierFromPemFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_certificate);
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);
        model.SetApplicationCertificatePath("test");
        model.SetApplicationPrivateKeyPath("test");

        // Act
        model.AddApplicationCertificateCommand.Execute(null);

        // Assert
        Assert.True(model.ApplicationCertificateIdentifiers.Count == 1);
        Assert.True(model.ApplicationPrivateKeyPath == string.Empty);
        Assert.True(model.ApplicationCertificatePath == string.Empty);
    }

    // add application certficiate throws exception
    [Fact]
    public void AddApplicationCertificate_ThrowsException()
    {
        // Arrange
        _fileUtilMock.Setup(_fileUtilMock => _fileUtilMock.CreateCertificateIdentifierFromPemFile(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception());
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);
        string certpath = "certpath";
        string privkeypath = "privkeypath";

        model.SetApplicationCertificatePath(certpath);
        model.SetApplicationPrivateKeyPath(privkeypath);

        // Act
        model.AddApplicationCertificateCommand.Execute(null);

        // Assert
        Assert.Empty(model.ApplicationCertificateIdentifiers);
        Assert.True(model.ApplicationPrivateKeyPath == privkeypath);
        Assert.True(model.ApplicationCertificatePath == certpath);
    }

    // add application certificate
    [Fact]
    public void RemoveApplicationCertificate()
    {
        // Arrange
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);
        model.ApplicationCertificateIdentifiers.Add(_certificate);

        // Act
        model.DeleteApplicationCertificate(_certificate);

        // Assert
        Assert.Empty(model.ApplicationCertificateIdentifiers);
    }

    // add user certificate
    [Fact]
    public void AddUserCertificate()
    {
        // Arrange
        _fileUtilMock.Setup(_fileUtilMock => _fileUtilMock.CreateCertificateIdentifierFromPemFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_certificate);
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);
        model.SetUserCertificatePath("test");
        model.SetUserPrivateKeyPath("test");

        // Act
        model.AddUserCertificateCommand.Execute(null);

        // Assert
        Assert.True(model.UserCertificateIdentifiers.Count == 1);
        Assert.True(model.UserPrivateKeyPath == string.Empty);
        Assert.True(model.UserCertificatePath == string.Empty);
    }

    // add user certficiate throws exception
    [Fact]
    public void AddUserCertificate_ThrowsException()
    {
        // Arrange
        _fileUtilMock.Setup(_fileUtilMock => _fileUtilMock.CreateCertificateIdentifierFromPemFile(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception());
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);
        string certpath = "certpath";
        string privkeypath = "privkeypath";

        model.SetUserCertificatePath(certpath);
        model.SetUserPrivateKeyPath(privkeypath);

        // Act
        model.AddUserCertificateCommand.Execute(null);

        // Assert
        Assert.Empty(model.UserCertificateIdentifiers);
        Assert.True(model.UserPrivateKeyPath == privkeypath);
        Assert.True(model.UserCertificatePath == certpath);
    }

    // add user certificate
    [Fact]
    public void RemoveUserCertificate()
    {
        // Arrange
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);
        model.UserCertificateIdentifiers.Add(_certificate);

        // Act
        model.DeleteUserCertificate(_certificate);

        // Assert
        Assert.Empty(model.UserCertificateIdentifiers);
    }

    // add username and password
    [Fact]
    public void AddUsernameAndPassword()
    {
        // Arrange
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);
        string username = "username";
        string password = "password";

        model.Username = username;
        model.Password = password;

        // Act
        model.AddUsernamePasswordCommand.Execute(null);

        // Assert
        Assert.True(model.Username == string.Empty);
        Assert.True(model.Password == string.Empty);
        Assert.True(model.UsernamesAndPasswords.Count == 1);
        Assert.Contains((username, password), model.UsernamesAndPasswords);
    }

    // remove username and password
    [Fact]
    public void RemoveUsernameAndPassword()
    {
        // Arrange
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);
        string username = "username";
        string password = "password";

        model.UsernamesAndPasswords.Add((username, password));

        // Act
        model.DeleteUsernamePassword((username, password));

        // Assert
        Assert.True(model.UsernamesAndPasswords.Count == 0);
    }

    // add usernames and passwords from file
    [Fact]
    public void AddUsernamesAndPasswordsFromFile()
    {
        // Arrange
        string path = "path";
        string[] lines = ["username:password", "username2:password2"];
        _fileUtilMock.Setup(_fileUtilMock => _fileUtilMock.ReadFileToList(It.IsAny<string>()))
            .Returns(lines.ToList());
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);

        // Act
        model.AddUsernamesPasswordsFromFile(path);

        // Assert
        Assert.True(model.UsernamesAndPasswords.Count == 2);
        Assert.Contains(("username", "password"), model.UsernamesAndPasswords);
        Assert.Contains(("username2", "password2"), model.UsernamesAndPasswords);
    }

    // add usernames and passwords from file throws exception
    [Fact]
    public void AddUsernamesAndPasswordsFromFile_ThrowsException()
    {
        // Arrange
        string path = "path";
        _fileUtilMock.Setup(_fileUtilMock => _fileUtilMock.ReadFileToList(It.IsAny<string>()))
            .Throws(new Exception());
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);

        // Act
        model.AddUsernamesPasswordsFromFile(path);

        // Assert
        Assert.True(model.UsernamesAndPasswords.Count == 0);
    }

    // add brute username and password
    [Fact]
    public void AddBruteUsernameAndPassword()
    {
        // Arrange
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);
        string username = "username";
        string password = "password";

        model.BruteUsername = username;
        model.BrutePassword = password;

        // Act
        model.AddBruteUsernamePasswordCommand.Execute(null);

        // Assert
        Assert.True(model.BruteUsername == string.Empty);
        Assert.True(model.BrutePassword == string.Empty);
        Assert.True(model.BruteUsernamesAndPasswords.Count == 1);
        Assert.Contains((username, password), model.BruteUsernamesAndPasswords);
    }

    // remove brute username and password
    [Fact]
    public void RemoveBruteUsernameAndPassword()
    {
        // Arrange
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);
        string username = "username";
        string password = "password";

        model.BruteUsernamesAndPasswords.Add((username, password));

        // Act
        model.DeleteBruteUsernamePassword((username, password));

        // Assert
        Assert.True(model.BruteUsernamesAndPasswords.Count == 0);
    }

    // add brute usernames and passwords from file
    [Fact]
    public void AddBruteUsernamesAndPasswordsFromFile()
    {
        // Arrange
        string path = "path";
        string[] lines = ["username:password", "username2:password2"];
        _fileUtilMock.Setup(_fileUtilMock => _fileUtilMock.ReadFileToList(It.IsAny<string>()))
            .Returns(lines.ToList());
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);

        // Act
        model.AddBruteUsernamesPasswordsFromFile(path);

        // Assert
        Assert.True(model.BruteUsernamesAndPasswords.Count == 2);
        Assert.Contains(("username", "password"), model.BruteUsernamesAndPasswords);
        Assert.Contains(("username2", "password2"), model.BruteUsernamesAndPasswords);
    }

    // add brute usernames and passwords from file throws exception
    [Fact]
    public void AddBruteUsernamesAndPasswordsFromFile_ThrowsException()
    {
        // Arrange
        string path = "path";
        _fileUtilMock.Setup(_fileUtilMock => _fileUtilMock.ReadFileToList(It.IsAny<string>()))
            .Throws(new Exception());
        ConfigurationViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object);

        // Act
        model.AddBruteUsernamesPasswordsFromFile(path);

        // Assert
        Assert.True(model.BruteUsernamesAndPasswords.Count == 0);
    }
}
#endif
