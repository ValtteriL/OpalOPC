namespace Tests;
using View;
using Model;
using Xunit;

public class Argparser_parseArgsShould
{
    [Fact]
    public void ParseArgs_InputOutputFileNameMatchesOptionsReportName()
    {
        const string filename = "a";
        string[] args = {"-o", filename};
        Argparser argparser = new Argparser(args);
        
        Options options = argparser.parseArgs();

        Assert.True(options.xmlOutputReportName == filename);
    }
}