namespace Tests;
using View;
using Model;
using Xunit;

public class Argparser_parseArgsShould
{
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
}