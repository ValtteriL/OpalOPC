using System.Security.Cryptography.X509Certificates;
using Moq;
using Opc.Ua;
using Opc.Ua.Security.Certificates;
using Util;
using Xunit;

namespace Tests;
public class SelfSignedCertificateUtil_Tests
{
    private readonly Mock<IFileUtil> _fileUtilMock;
    private readonly CertificateIdentifier _certificateIdentifier;

    public SelfSignedCertificateUtil_Tests()
    {
        _fileUtilMock = new Mock<IFileUtil>();

        X509Certificate2 cert = CertificateBuilder.Create("CN=Root CA").CreateForRSA();
        _certificateIdentifier = new(cert);
    }

    // creates new certificate if not found
    [Fact]
    public void GetCertificate_CreatesNewCertificate()
    {
        // arrange
        SelfSignedCertificateUtil selfSignedCertificateUtil = new(_fileUtilMock.Object);
        _fileUtilMock.Setup(fileUtil => fileUtil.CreateCertificateIdentifierFromPfxFile(It.IsAny<string>())).Throws(new FileNotFoundException());

        // act
        CertificateIdentifier certificate = selfSignedCertificateUtil.GetCertificate();

        // assert
        Assert.NotNull(certificate);
        CertificateNotExpired(certificate);
    }

    // returns existing certificate if found
    [Fact]
    public void GetCertificate_ReturnsExistingCertificate()
    {
        // arrange
        SelfSignedCertificateUtil selfSignedCertificateUtil = new(_fileUtilMock.Object);
        _fileUtilMock.Setup(fileUtil => fileUtil.CreateCertificateIdentifierFromPfxFile(It.IsAny<string>())).Returns(_certificateIdentifier);

        // act
        CertificateIdentifier certificate = selfSignedCertificateUtil.GetCertificate();

        // assert
        Assert.NotNull(certificate);
        Assert.Equal(_certificateIdentifier, certificate);
        CertificateNotExpired(certificate);
    }

    // creates new certificate if existing certificate has expired
    [Fact]
    public void GetCertificate_CreatesNewCertificateIfExistingCertificateHasExpired()
    {
        // arrange
        SelfSignedCertificateUtil selfSignedCertificateUtil = new(_fileUtilMock.Object);
        X509Certificate2 cert = CertificateBuilder.Create("CN=Root CA").SetNotAfter(DateTime.Now - TimeSpan.FromDays(1)).CreateForRSA();
        CertificateIdentifier certificateIdentifier = new(cert);
        _fileUtilMock.Setup(fileUtil => fileUtil.CreateCertificateIdentifierFromPemFile(It.IsAny<string>(), It.IsAny<string>())).Returns(certificateIdentifier);

        // act
        CertificateIdentifier certificate = selfSignedCertificateUtil.GetCertificate();

        // assert
        Assert.NotNull(certificate);
        Assert.NotEqual(certificateIdentifier, certificate);
        CertificateNotExpired(certificate);
    }

    // throws exception if error writing certificate to disk
    [Fact]
    public void GetCertificate_ThrowsExceptionIfErrorWritingCertificateToDisk()
    {
        // arrange
        SelfSignedCertificateUtil selfSignedCertificateUtil = new(_fileUtilMock.Object);
        _fileUtilMock.Setup(fileUtil => fileUtil.WriteCertificateToDisk(It.IsAny<CertificateIdentifier>(), It.IsAny<string>())).Throws(new Exception());

        // act
        try
        {
            CertificateIdentifier certificate = selfSignedCertificateUtil.GetCertificate();
        }
        catch (Exception e)
        {
            Assert.NotNull(e);
            return;
        }

        // assert
        Assert.True(false);
    }

    private static void CertificateNotExpired(CertificateIdentifier certificate)
    {
        Assert.True(certificate.Certificate.NotAfter > DateTime.Now);
        Assert.True(certificate.Certificate.NotBefore < DateTime.Now);
    }
}

