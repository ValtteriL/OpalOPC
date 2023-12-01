#if BUILT_FOR_WINDOWS
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Moq;
using OpalOPC.WPF;
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
    private readonly X509Certificate2 _certificate;

    public ConfigurationViewModel_Tests()
    {
        _certificate = CertificateBuilder
                    .Create("CN=Test")
                    .AddExtension(
                        new X509SubjectAltNameExtension("urn:opalopc.com:host",
                        new string[] { "host", "host.opalopc.com", "192.168.1.100" }))
                    .CreateForRSA();
    }


    // initial values when starting
    [Fact]
    public void Constructor()
    {
        ConfigurationViewModel model = new();

        Assert.True(model.ApplicationCertificatePath != null);
        Assert.True(model.ApplicationPrivateKeyPath != null);
        Assert.True(model.ApplicationCertificateIdentifiers != null);
    }

    // add application certificate
    [Fact]
    public void AddApplicationCertificate()
    {
        // Arrange
        _fileUtilMock.Setup(_fileUtilMock => _fileUtilMock.CreateFromPemFile(It.IsAny<string>(), It.IsAny<string>()))
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
        _fileUtilMock.Setup(_fileUtilMock => _fileUtilMock.CreateFromPemFile(It.IsAny<string>(), It.IsAny<string>()))
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
        CertificateIdentifier certificateIdentifier = new CertificateIdentifier(_certificate);
        model.ApplicationCertificateIdentifiers.Add(certificateIdentifier);

        // Act
        model.DeleteApplicationCertificate(certificateIdentifier);

        // Assert
        Assert.Empty(model.ApplicationCertificateIdentifiers);
    }


}
#endif
