using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Model;

namespace View
{
    public interface IReporter
    {
        public void printXHTMLReport(Report report);
    }

    public class Reporter : IReporter
    {
        private Stream outputStream;

        public Reporter(Stream outputStream)
        {
            this.outputStream = outputStream;
        }

        public void printXHTMLReport(Report report)
        {
            XmlWriterSettings settings = new() { Indent = true };
            using XmlWriter finalXmlWriter = XmlWriter.Create(outputStream, settings);

            MemoryStream stream = new();
            using XmlWriter tempXmlWriter = XmlWriter.Create(stream);

            // serialize report to tempXmlWriter
            XmlSerializer serializer = new(report.GetType());
            tempXmlWriter.WriteDocType("Report", null, null, null);
            serializer.Serialize(tempXmlWriter, report);

            // transform report to xhtml with XSLT
            // and write to finalXmlWriter
            XslCompiledTransform xslCompiledTransform = getXSLT();
            stream.Seek(0, SeekOrigin.Begin);
            xslCompiledTransform.Transform(XmlReader.Create(stream, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }), finalXmlWriter);
        }


        private XslCompiledTransform getXSLT()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(Util.XmlResources.StylesheetLocation);

            if (stream == null)
            {
                throw new FileNotFoundException($"Cannot find XSL stylesheet {Util.XmlResources.StylesheetLocation}");
            }

            XslCompiledTransform xslCompiledTransform = new();
            xslCompiledTransform.Load(XmlReader.Create(stream));

            return xslCompiledTransform;
        }
    }
}