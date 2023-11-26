using Model;
using Moq;
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
    private readonly List<string> _usernameFileLines;
    private readonly List<string> _passwordFileLines;
    private readonly List<string> _loginFileLinesSingle;
    private readonly List<string> _loginFileLines;
    public Argparser_parseArgsShould()
    {
        _usernameFileLines = new List<string> { _username, _username2 };
        _passwordFileLines = new List<string> { _password, _password2 };
        _loginFileLinesSingle = new List<string> { $"{_username}:{_password}" };
        _loginFileLines = new List<string> { $"{_username}:{_password}", $"{_username2}:{_password2}" };
        _loginCredential = (_username, _password);
        _loginCredential2 = (_username2, _password2);
    }

    [Fact]
    public void ParseArgs_NoTargets_ResultsInNoTargets()
    {
        string[] args = { "-v" };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.targets.Count == 0);
        Assert.True(options.exitCode == null);
    }

    [Theory]
    [InlineData("-o", "a")]
    [InlineData("--output", "b")]
    public void ParseArgs_InputOutputFileNameMatchesOptionsReportName(string flag, string filename)
    {
        string[] args = { flag, filename };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.xmlOutputReportName == filename);
        Assert.True(options.xmlOutputStream!.CanWrite);
        Assert.True(options.exitCode == null);
    }

    [Theory]
    [InlineData("-o", "this/path/does/not/exist")]
    [InlineData("--output", "this/path/does/not/exist2")]
    public void ParseArgs_InputInvalidOutputFileNameCausesTermination(string flag, string filename)
    {
        string[] args = { flag, filename };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.exitCode == Util.ExitCodes.Error);
    }

    [Fact]
    public void ParseArgs_IncreaseVerbosity1()
    {
        string[] args = { "-v" };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.logLevel == Microsoft.Extensions.Logging.LogLevel.Debug);
        Assert.True(options.exitCode == null);
    }

    [Fact]
    public void ParseArgs_IncreaseVerbosity2()
    {
        string[] args = { "-v", "-v" };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.logLevel == Microsoft.Extensions.Logging.LogLevel.Trace);
        Assert.True(options.exitCode == null);
    }

    [Fact]
    public void ParseArgs_SilenceLogging()
    {
        string[] args = { "-s" };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.logLevel == Microsoft.Extensions.Logging.LogLevel.None);
        Assert.True(options.exitCode == null);
    }

    [Fact]
    public void ParseArgs_NoTargets_ResultsInHelp()
    {
        string[] args = Array.Empty<string>();
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.exitCode == Util.ExitCodes.Success);
    }

    [Theory]
    [InlineData("-h")]
    [InlineData("--help")]
    public void ParseArgs_Help_ResultsInHelp(string flag)
    {
        string[] args = { flag };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.exitCode == Util.ExitCodes.Success);
    }

    [Theory]
    [InlineData("--version")]
    public void ParseArgs_Version_ResultsInVersion(string flag)
    {
        string[] args = { flag };
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
        string[] args = { flag, filename };
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
        string[] args = { flag, $"{_loginCredential.Item1}:{_loginCredential.Item2}" };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.authenticationData.loginCredentials.Count == 1);
        Assert.Contains(_loginCredential, options.authenticationData.loginCredentials);
        Assert.True(options.exitCode == null);
    }

    [Theory]
    [InlineData("-l")]
    [InlineData("--login-credential")]
    public void ParseArgs_LoginIsAddedToAuthdataMultiple(string flag)
    {
        string[] args = { flag, $"{_loginCredential.Item1}:{_loginCredential.Item2}", flag, $"{_loginCredential2.Item1}:{_loginCredential2.Item2}" };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.authenticationData.loginCredentials.Count == 2);
        Assert.Contains(_loginCredential, options.authenticationData.loginCredentials);
        Assert.Contains(_loginCredential2, options.authenticationData.loginCredentials);
        Assert.True(options.exitCode == null);
    }


    [Theory]
    [InlineData("-b")]
    [InlineData("--brute-force-credential")]
    public void ParseArgs_BruteIsAddedToAuthdata(string flag)
    {
        string[] args = { flag, $"{_loginCredential.Item1}:{_loginCredential.Item2}" };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.authenticationData.bruteForceCredentials.Count == 1);
        Assert.Contains(_loginCredential, options.authenticationData.bruteForceCredentials);
        Assert.True(options.exitCode == null);
    }

    [Theory]
    [InlineData("-b")]
    [InlineData("--brute-force-credential")]
    public void ParseArgs_BruteIsAddedToAuthdataMultiple(string flag)
    {
        string[] args = { flag, $"{_loginCredential.Item1}:{_loginCredential.Item2}", flag, $"{_loginCredential2.Item1}:{_loginCredential2.Item2}" };
        Argparser argparser = new(args);

        Options options = argparser.parseArgs();

        Assert.True(options.authenticationData.bruteForceCredentials.Count == 2);
        Assert.Contains(_loginCredential, options.authenticationData.bruteForceCredentials);
        Assert.Contains(_loginCredential2, options.authenticationData.bruteForceCredentials);
        Assert.True(options.exitCode == null);
    }

    [Theory]
    [InlineData("-L")]
    [InlineData("--login-credential-file")]
    public void ParseArgs_LoginIsAddedToAuthdataFromFile(string flag)
    {
        // arrange
        string[] args = { flag, _path };

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
        Assert.True(options.exitCode == null);
    }

    [Theory]
    [InlineData("-L")]
    [InlineData("--login-credential-file")]
    public void ParseArgs_LoginIsAddedToAuthdataFromFileMultiple(string flag)
    {
        // arrange
        string[] args = { flag, _path };

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
        Assert.True(options.exitCode == null);
    }

    [Theory]
    [InlineData("-L")]
    [InlineData("--login-credential-file")]
    public void ParseArgs_LoginFileException_ResultsInError(string flag)
    {
        // arrange
        string[] args = { flag, _path };

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
        string[] args = { flag, _path };
        string invalidLine = "asddasdasda";

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Returns(new List<string> { invalidLine });
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
        string[] args = { flag, _path };

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
        Assert.True(options.exitCode == null);
    }

    [Theory]
    [InlineData("-B")]
    [InlineData("--brute-force-credential-file")]
    public void ParseArgs_BruteIsAddedToAuthdataFromFileMultiple(string flag)
    {
        // arrange
        string[] args = { flag, _path };

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
        Assert.True(options.exitCode == null);
    }

    [Theory]
    [InlineData("-B")]
    [InlineData("--brute-force-credential-file")]
    public void ParseArgs_BruteFileException_ResultsInError(string flag)
    {
        // arrange
        string[] args = { flag, _path };

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Throws(new System.Exception());
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // asser
        Assert.True(options.exitCode == ExitCodes.Error);
    }

    [Theory]
    [InlineData("-B")]
    [InlineData("--brute-force-credential-file")]
    public void ParseArgs_BruteFileWithInvalidLine_ResultsInError(string flag)
    {
        // arrange
        string[] args = { flag, _path };
        string invalidLine = "asddasdasda";

        _fileUtilMock.Setup(x => x.ReadFileToList(_path)).Returns(new List<string> { invalidLine });
        Argparser argparser = new(args, _fileUtilMock.Object);

        // act
        Options options = argparser.parseArgs();

        // assert
        Assert.True(options.exitCode == ExitCodes.Error);
    }
}
