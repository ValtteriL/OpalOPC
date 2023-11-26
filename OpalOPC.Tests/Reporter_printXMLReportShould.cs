using Model;
using View;
using Xunit;

namespace Tests;
public class Reporter_printXHTMLReportShould
{
    private readonly Reporter _reporter;
    private readonly MemoryStream _memoryStream;

    public Reporter_printXHTMLReportShould()
    {
        _memoryStream = new();

        StreamWriter sw = new(_memoryStream)
        {
            AutoFlush = true
        };
        _reporter = new(sw.BaseStream);
    }

    [Fact]
    public void printXHTMLReport_NullReportCausesNullReferenceException()
    {
        Report? report = null;

        try
        {
            _reporter.PrintXHTMLReport(report!);
        }
        catch (System.NullReferenceException)
        {
            Assert.True(true);
            return;
        }

        Assert.True(false);
    }

    [Fact]
    public void printXHTMLReport_NonNullReportSucceeds()
    {
        Report report = new(new List<Target>(), DateTime.Now, DateTime.Now, string.Empty);

        _reporter.PrintXHTMLReport(report);
    }

    [Fact]
    public void printXHTMLReport_ReportResourcesReplacedInTestingReports()
    {
        Report report = new(new List<Target>(), DateTime.Now, DateTime.Now, string.Empty);

        _reporter.PrintXHTMLReport(report);

        _memoryStream.Seek(0, SeekOrigin.Begin);
        StreamReader reader = new(_memoryStream);
        string reportString = reader.ReadToEnd();

        Assert.Contains(Util.XmlResources.DebugResourcePath, reportString);
        Assert.DoesNotContain(Util.XmlResources.ProdResourcePath, reportString);
    }
}
