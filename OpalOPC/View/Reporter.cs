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
                w.WriteProcessingInstruction("xml-stylesheet", $"type=\"text/xsl\" href=\"{Util.XmlResources.StylesheetLocation}\"");
                w.WriteDocType("Report", null, Util.XmlResources.DtdLocation, null);
                serializer.Serialize(w, report);
            }
        }
    }
}