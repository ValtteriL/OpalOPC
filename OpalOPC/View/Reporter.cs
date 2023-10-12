using System.Reflection;
using System.Xml;
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
            XmlSerializer serializer = new XmlSerializer(report.GetType());

            XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
            using (XmlWriter w = XmlWriter.Create(outputStream, settings))
            {
                w.WriteProcessingInstruction("xml-stylesheet", $"type=\"text/xsl\" href=\"#stylesheet\"");
                w.WriteDocType("Report", null, Util.XmlResources.DtdLocation, null);
                w.WriteRaw(getStylesheet()); // TODO: this must be written inside the Report object
                serializer.Serialize(w, report);
            }
        }

        private string getStylesheet()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream? stream = assembly.GetManifestResourceStream(Util.XmlResources.StylesheetLocation);

            if (stream == null)
            {
                throw new FileNotFoundException($"Cannot find XSL stylesheet {Util.XmlResources.StylesheetLocation}");
            }

            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}