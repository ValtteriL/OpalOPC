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
        Argparser argparser = new Argparser(args);

        Options options = argparser.parseArgs();

        Assert.True(options.targets.Count == 0);
    }

    [Theory]
    [InlineData("-o", "a")]
    [InlineData("--output", "b")]
    public void ParseArgs_InputOutputFileNameMatchesOptionsReportName(string flag, string filename)
    {
        string[] args = { flag, filename };
        Argparser argparser = new Argparser(args);

        Options options = argparser.parseArgs();

        Assert.True(options.xmlOutputReportName == filename);
    }

    [Fact]
    public void ParseArgs_IncreaseVerbosity1()
    {
        string[] args = { "-v" };
        Argparser argparser = new Argparser(args);

        Options options = argparser.parseArgs();

        Assert.True(options.logLevel == Microsoft.Extensions.Logging.LogLevel.Debug);
    }

    [Fact]
    public void ParseArgs_IncreaseVerbosity2()
    {
        string[] args = { "-v", "-v" };
        Argparser argparser = new Argparser(args);

        Options options = argparser.parseArgs();

        Assert.True(options.logLevel == Microsoft.Extensions.Logging.LogLevel.Trace);
    }

    [Fact]
    public void ParseArgs_SilenceLogging()
    {
        string[] args = { "-s" };
        Argparser argparser = new Argparser(args);

        Options options = argparser.parseArgs();

        Assert.True(options.logLevel == Microsoft.Extensions.Logging.LogLevel.None);
    }
}