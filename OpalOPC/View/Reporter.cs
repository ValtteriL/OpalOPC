using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Model;

namespace View
{
    public interface IReporter
    {
        public void printXMLReport(Report report);
    }

    public class Reporter : IReporter
    {
        private Stream outputStream;

        public Reporter(Stream outputStream)
        {
            this.outputStream = outputStream;
        }

        public void printXMLReport(Report report)
        {
            XmlWriterSettings settings = new() { Indent = true };
            using XmlWriter w = XmlWriter.Create(outputStream, settings);

            w.WriteProcessingInstruction("xml-stylesheet", $"type=\"text/xsl\" href=\"#stylesheet\"");
            w.WriteDocType("Report", null, null, null);

            // add stylesheet as child of report
            XElement reportElement = getReportAsXElement(report);
            XElement stylesheetElement = getStylesheet();
            reportElement.Add(stylesheetElement);

            // write report element to outputstream
            reportElement.WriteTo(w);
        }

        private XElement getReportAsXElement(Report report)
        {
            XmlSerializer serializer = new(report.GetType());
            string reportXml;

            using StringWriter textWriter = new();
            serializer.Serialize(textWriter, report);
            reportXml = textWriter.ToString();
            return XElement.Parse(reportXml);
        }

        private XElement getStylesheet()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream? stream = assembly.GetManifestResourceStream(Util.XmlResources.StylesheetLocation);

            if (stream == null)
            {
                throw new FileNotFoundException($"Cannot find XSL stylesheet {Util.XmlResources.StylesheetLocation}");
            }

            return XElement.Load(stream);
        }
    }
}