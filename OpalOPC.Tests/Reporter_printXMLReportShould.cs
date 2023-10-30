namespace Tests;
using System.Xml;
using System.Xml.Serialization;
using Model;
using View;
using Xunit;

public class Reporter_printXHTMLReportShould
{
    [Fact]
    public void printXHTMLReport_NullReportCausesNullReferenceException()
    {
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);
        Report? report = null;

        try
        {
            reporter.PrintXHTMLReport(report!);
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
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);
        Report report = new(new List<Target>(), DateTime.Now, DateTime.Now, string.Empty);

        reporter.PrintXHTMLReport(report);
    }

    [Fact]
    public void printXHTMLReport_ReportResourcesReplacedInTestingReports()
    {
        MemoryStream ms = new();
        StreamWriter sw = new(ms)
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);
        Report report = new(new List<Target>(), DateTime.Now, DateTime.Now, string.Empty);

        reporter.PrintXHTMLReport(report);

        ms.Seek(0, SeekOrigin.Begin);
        StreamReader reader = new(ms);
        string reportString = reader.ReadToEnd();

        Assert.Contains(Util.XmlResources.DebugResourcePath, reportString);
        Assert.DoesNotContain(Util.XmlResources.ProdResourcePath, reportString);
    }
}
