namespace Tests;
using View;
using Model;
using Xunit;

public class Reporter_printXMLReportShould
{
    [Fact]
    public void printXMLReport_NullReportCausesNullReferenceException()
    {
        StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
        sw.AutoFlush = true;
        Reporter reporter = new Reporter(sw.BaseStream);
        Report? report = null;

        try
        {
            reporter.printXMLReport(report!);
        }
        catch (System.NullReferenceException)
        {
            Assert.True(true);
            return;
        }

        Assert.True(false);
    }

    [Fact]
    public void printXMLReport_NonNullReportSucceeds()
    {
        StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
        sw.AutoFlush = true;
        Reporter reporter = new Reporter(sw.BaseStream);
        Report report = new Report(new List<Target>(), DateTime.Now, DateTime.Now, string.Empty);

        reporter.printXMLReport(report);
    }

    [Fact]
    public void printXMLReport_DtdValidationSucceeds()
    {
        StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
        sw.AutoFlush = true;
        Reporter reporter = new Reporter(sw.BaseStream);
        Report report = new Report(new List<Target>(), DateTime.Now, DateTime.Now, string.Empty);

        reporter.printXMLReport(report);

        // TODO
        Assert.True(false);
    }
}