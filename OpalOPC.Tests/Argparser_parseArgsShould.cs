using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Model;
using Moq;
using Opc.Ua;
using Opc.Ua.Security.Certificates;
using Util;
using View;
using Xunit;

namespace Tests;
public class Argparser_parseArgsShould
{

    private readonly Mock<IFileUtil> _fileUtilMock = new();
    private readonly string _username = "testusername";
    private readonly string _username2 = "testusername2";
    private readonly string _password = "testpassword";
    private readonly string _password2 = "testpassword2";
    private readonly string _path = "testuserfile";
    private readonly (string, string) _loginCredential;
    private readonly (string, string) _loginCredential2;
    private readonly List<string> _loginFileLinesSingle;
    private readonly List<string> _loginFileLines;
    public Argparser_parseArgsShould()
    {
        _loginFileLinesSingle = [$"{_username}:{_password}"];
        _loginFileLines = [$"{_username}:{_password}", $"{_username2}:{_password2}"];
        _loginCredential = (_username, _password);
        _loginCredential2 = (_username2, _password2);
    }

    [Fact]
    public void ParseArgs_NoTargets_ResultsInNoTargets()
    {
        string[] args = ["-v"];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.targets.Count == 0);
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Theory]
    [InlineData("-o", "a")]
    [InlineData("--output", "b")]
    public void ParseArgs_InputOutputFileNameMatchesOptionsReportName(string flag, string filename)
    {
        string[] args = [flag, filename];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.HtmlOutputReportName == filename + ".html");
        Assert.True(options.SarifOutputReportName == filename + ".sarif");
        Assert.True(options.HtmlOutputStream!.CanWrite);
        Assert.True(options.SarifOutputStream!.CanWrite);
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Theory]
    [InlineData("-o", "this/path/does/not/exist")]
    [InlineData("--output", "this/path/does/not/exist2")]
    public void ParseArgs_InputInvalidOutputFileNameCausesTermination(string flag, string filename)
    {
        string[] args = [flag, filename];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.exitCode == Util.ExitCodes.Error);
    }

    [Fact]
    public void ParseArgs_IncreaseVerbosity1()
    {
        string[] args = ["-v"];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.logLevel == Microsoft.Extensions.Logging.LogLevel.Debug);
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Fact]
    public void ParseArgs_IncreaseVerbosity2()
    {
        string[] args = ["-v", "-v"];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.logLevel == Microsoft.Extensions.Logging.LogLevel.Trace);
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Fact]
    public void ParseArgs_NoTargets_ResultsInHelp()
    {
        string[] args = [];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.exitCode == Util.ExitCodes.Success);
    }

    [Theory]
    [InlineData("-h")]
    [InlineData("--help")]
    public void ParseArgs_Help_ResultsInHelp(string flag)
    {
        string[] args = [flag];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.exitCode == Util.ExitCodes.Success);
    }

    [Theory]
    [InlineData("--version")]
    public void ParseArgs_Version_ResultsInVersion(string flag)
    {
        string[] args = [flag];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.exitCode == Util.ExitCodes.Success);
    }

    [Theory]
    [InlineData("-i")]
    [InlineData("--input")]
    public void ParseArgs_InvalidInputFile_ResultsInError(string flag)
    {
        string filename = "ahfoihfsa";
        string[] args = [flag, filename];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.targets.Count == 0);
        Assert.True(options.exitCode == Util.ExitCodes.Error);
    }

    [Theory]
    [InlineData("-l")]
    [InlineData("--login-credential")]
    public void ParseArgs_LoginIsAddedToAuthdata(string flag)
    {
        string[] args = [flag, $"{_loginCredential.Item1}:{_loginCredential.Item2}"];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.authenticationData.loginCredentials.Count == 1);
        Assert.Contains(_loginCredential, options.authenticationData.loginCredentials);
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Theory]
    [InlineData("-l")]
    [InlineData("--login-credential")]
    public void ParseArgs_LoginIsAddedToAuthdataMultiple(string flag)
    {
        string[] args = [flag, $"{_loginCredential.Item1}:{_loginCredential.Item2}", flag, $"{_loginCredential2.Item1}:{_loginCredential2.Item2}"];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.authenticationData.loginCredentials.Count == 2);
        Assert.Contains(_loginCredential, options.authenticationData.loginCredentials);
        Assert.Contains(_loginCredential2, options.authenticationData.loginCredentials);
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }


    [Theory]
    [InlineData("-b")]
    [InlineData("--brute-force-credential")]
    public void ParseArgs_BruteIsAddedToAuthdata(string flag)
    {
        string[] args = [flag, $"{_loginCredential.Item1}:{_loginCredential.Item2}"];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.authenticationData.bruteForceCredentials.Count == 1);
        Assert.Contains(_loginCredential, options.authenticationData.bruteForceCredentials);
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Theory]
    [InlineData("-b")]
    [InlineData("--brute-force-credential")]
    public void ParseArgs_BruteIsAddedToAuthdataMultiple(string flag)
    {
        string[] args = [flag, $"{_loginCredential.Item1}:{_loginCredential.Item2}", flag, $"{_loginCredential2.Item1}:{_loginCredential2.Item2}"];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.authenticationData.bruteForceCredentials.Count == 2);
        Assert.Contains(_loginCredential, options.authenticationData.bruteForceCredentials);
        Assert.Contains(_loginCredential2, options.authenticationData.bruteForceCredentials);
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Theory]
    [InlineData("-L")]
    [InlineData("--login-credential-file")]
    public void ParseArgs_LoginIsAddedToAuthdataFromFile(string flag)
    {
        // arrange
        string[] args = [flag, _path];

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Returns(_loginFileLinesSingle);
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // assert
        Assert.True(options.authenticationData.loginCredentials.Count == _loginFileLinesSingle.Count);
        foreach (string line in _loginFileLinesSingle)
        {
            string[] splitLine = line.Split(':', 2);
            Assert.Contains((splitLine[0], splitLine[1]), options.authenticationData.loginCredentials);
        }
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Theory]
    [InlineData("-L")]
    [InlineData("--login-credential-file")]
    public void ParseArgs_LoginIsAddedToAuthdataFromFileMultiple(string flag)
    {
        // arrange
        string[] args = [flag, _path];

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Returns(_loginFileLines);
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // assert
        Assert.True(options.authenticationData.loginCredentials.Count == _loginFileLines.Count);
        foreach (string line in _loginFileLines)
        {
            string[] splitLine = line.Split(':', 2);
            Assert.Contains((splitLine[0], splitLine[1]), options.authenticationData.loginCredentials);
        }
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Theory]
    [InlineData("-L")]
    [InlineData("--login-credential-file")]
    public void ParseArgs_LoginFileException_ResultsInError(string flag)
    {
        // arrange
        string[] args = [flag, _path];

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Throws(new System.Exception());
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // asser
        Assert.True(options.exitCode == ExitCodes.Error);
    }

    [Theory]
    [InlineData("-L")]
    [InlineData("--login-credential-file")]
    public void ParseArgs_LoginFileWithInvalidLine_ResultsInError(string flag)
    {
        // arrange
        string[] args = [flag, _path];
        string invalidLine = "asddasdasda";

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Returns([invalidLine]);
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // assert
        Assert.True(options.exitCode == ExitCodes.Error);
    }

    [Theory]
    [InlineData("-B")]
    [InlineData("--brute-force-credential-file")]
    public void ParseArgs_BruteIsAddedToAuthdataFromFile(string flag)
    {
        // arrange
        string[] args = [flag, _path];

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Returns(_loginFileLinesSingle);
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // assert
        Assert.True(options.authenticationData.bruteForceCredentials.Count == _loginFileLinesSingle.Count);
        foreach (string line in _loginFileLinesSingle)
        {
            string[] splitLine = line.Split(':', 2);
            Assert.Contains((splitLine[0], splitLine[1]), options.authenticationData.bruteForceCredentials);
        }
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Theory]
    [InlineData("-B")]
    [InlineData("--brute-force-credential-file")]
    public void ParseArgs_BruteIsAddedToAuthdataFromFileMultiple(string flag)
    {
        // arrange
        string[] args = [flag, _path];

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Returns(_loginFileLines);
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // assert
        Assert.True(options.authenticationData.bruteForceCredentials.Count == _loginFileLines.Count);
        foreach (string line in _loginFileLines)
        {
            string[] splitLine = line.Split(':', 2);
            Assert.Contains((splitLine[0], splitLine[1]), options.authenticationData.bruteForceCredentials);
        }
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
    }

    [Theory]
    [InlineData("-B")]
    [InlineData("--brute-force-credential-file")]
    public void ParseArgs_BruteFileException_ResultsInError(string flag)
    {
        // arrange
        string[] args = [flag, _path];

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Throws(new System.Exception());
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // assert
        Assert.True(options.exitCode == ExitCodes.Error);
    }

    [Theory]
    [InlineData("-B")]
    [InlineData("--brute-force-credential-file")]
    public void ParseArgs_BruteFileWithInvalidLine_ResultsInError(string flag)
    {
        // arrange
        string[] args = [flag, _path];
        string invalidLine = "asddasdasda";

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Returns([invalidLine]);
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // assert
        Assert.True(options.exitCode == ExitCodes.Error);
    }

    public enum CertType
    {
        User,
        Application,
    }

    [Theory]
    [InlineData("-c", CertType.User)]
    [InlineData("--user-certificate-and-privatekey", CertType.User)]
    [InlineData("-a", CertType.Application)]
    [InlineData("--application-certificate-and-privatekey", CertType.Application)]
    public void ParseArgs_CertFile_ResultsInUserOrApplicationCertificate(string flag, CertType certType)
    {
        // arrange
        string certpath = "certpath";
        string privkeypath = "privkeypath";
        string patharg = $"{certpath}:{privkeypath}";
        string[] args = [flag, patharg];
        X509Certificate2 cert = CertificateBuilder.Create("CN=Root CA").CreateForRSA();
        CertificateIdentifier certificateIdentifier = new(cert);

        _fileUtilMock.Setup(x => x.CreateCertificateIdentifierFromPemFile(certpath, privkeypath)).Returns(certificateIdentifier);
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // assert
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);


        List<CertificateIdentifier> certificateIdentifiers;
        if (certType == CertType.User)
        {
            certificateIdentifiers = options.authenticationData.userCertificates;
        }
        else
        {
            certificateIdentifiers = options.authenticationData.applicationCertificates;
        }

        Assert.True(certificateIdentifiers.Count == 1);
        Assert.Contains(certificateIdentifier, certificateIdentifiers);
    }

    [Theory]
    [InlineData("-c", CertType.User)]
    [InlineData("--user-certificate-and-privatekey", CertType.User)]
    [InlineData("-a", CertType.Application)]
    [InlineData("--application-certificate-and-privatekey", CertType.Application)]
    public void ParseArgs_CertFile_ResultsInUserOrApplicationCertificateMultiple(string flag, CertType certType)
    {
        // arrange
        string certpath = "certpath";
        string privkeypath = "privkeypath";
        string patharg = $"{certpath}:{privkeypath}";
        string[] args = [flag, patharg, flag, patharg];
        X509Certificate2 cert = CertificateBuilder.Create("CN=Root CA").CreateForRSA();
        CertificateIdentifier certificateIdentifier = new(cert);

        _fileUtilMock.Setup(x => x.CreateCertificateIdentifierFromPemFile(certpath, privkeypath)).Returns(certificateIdentifier);
        Argparser argparser = new(args, _fileUtilMock.Object);
        List<CertificateIdentifier> certificateIdentifiers;


        // act
        Options options = argparser.parseArgs();

        if (certType == CertType.User)
        {
            certificateIdentifiers = options.authenticationData.userCertificates;
        }
        else
        {
            certificateIdentifiers = options.authenticationData.applicationCertificates;
        }

        // assert
        Assert.True(options.exitCode == ExitCodes.Success);
        Assert.True(options.shouldExit == false);
        Assert.True(certificateIdentifiers.Count == 2);
        foreach (CertificateIdentifier identifier in certificateIdentifiers)
        {
            Assert.True(identifier.Certificate.Thumbprint == cert.Thumbprint);
        }
    }

    [Theory]
    [InlineData("-c")]
    [InlineData("--user-certificate-and-privatekey")]
    [InlineData("-a")]
    [InlineData("--application-certificate-and-privatekey")]
    public void ParseArgs_CertFileCryptoGraphicException_ResultsInError(string flag)
    {
        // arrange
        string certpath = "certpath";
        string privkeypath = "privkeypath";
        string patharg = $"{certpath}:{privkeypath}";
        string[] args = [flag, patharg];
        X509Certificate2 cert = CertificateBuilder.Create("CN=Root CA").CreateForRSA();

        _fileUtilMock.Setup(x => x.CreateCertificateIdentifierFromPemFile(certpath, privkeypath)).Throws(new CryptographicException());
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // assert
        Assert.True(options.exitCode == ExitCodes.Error);
    }

    [Theory]
    [InlineData("-d")]
    [InlineData("--discovery")]
    public void ParseArgs_DiscoverySetsShouldDiscoverAndExit(string flag)
    {
        string[] args = [flag];
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.shouldDiscoverAndExit);
    }

}
