namespace Tests;
using View;
using Model;
using Xunit;

public class Reporter_printXHTMLReportShould
{
    [Fact]
    public void printXHTMLReport_NullReportCausesNullReferenceException()
    {
        StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
        sw.AutoFlush = true;
        Reporter reporter = new Reporter(sw.BaseStream);
        Report? report = null;

        try
        {
            reporter.printXHTMLReport(report!);
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
        StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
        sw.AutoFlush = true;
        Reporter reporter = new Reporter(sw.BaseStream);
        Report report = new Report(new List<Target>(), DateTime.Now, DateTime.Now, string.Empty);

        reporter.printXHTMLReport(report);
    }

    [Fact]
    public void printXHTMLReport_DtdValidationSucceeds()
    {
        StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
        sw.AutoFlush = true;
        Reporter reporter = new Reporter(sw.BaseStream);
        Report report = new Report(new List<Target>(), DateTime.Now, DateTime.Now, string.Empty);

        reporter.printXHTMLReport(report);

        // TODO
        Assert.True(false);
    }
}