using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Model;

namespace View
{
    public interface IReporter
    {
        public void PrintXHTMLReport(Report report);
    }

    public class Reporter : IReporter
    {
        private readonly Stream outputStream;

        public Reporter(Stream outputStream)
        {
            this.outputStream = outputStream;
        }

        public void PrintXHTMLReport(Report report)
        {
            // serialize report to stream
            MemoryStream stream = new();
            using XmlWriter tempXmlWriter = XmlWriter.Create(stream);
            XmlSerializer serializer = new(report.GetType());
            serializer.Serialize(tempXmlWriter, report);

            // read XSLT, and use its settings to create XmlWriter to outputStream
            XslCompiledTransform xslCompiledTransform = getXSLT();
            using XmlWriter finalXmlWriter = XmlWriter.Create(outputStream, xslCompiledTransform.OutputSettings);

            // transform report to xhtml with XSLT
            // and write to outputStream
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

            Stream finalstream = stream;

#if !DEBUG
            // Replace debug paths in XSLT
            StreamReader reader = new(stream);
            string xsltString = reader.ReadToEnd();
            xsltString = xsltString.Replace(Util.XmlResources.DebugResourcePath, Util.XmlResources.ProdResourcePath);

            // Write corrected XSLT
            // and adjust stream for reader
            MemoryStream ms = new();
            StreamWriter writer = new(ms)
            {
                AutoFlush = true
            };
            writer.Write(xsltString);

            ms.Seek(0, SeekOrigin.Begin);
            finalstream = ms;
#endif


            XslCompiledTransform xslCompiledTransform = new();
            xslCompiledTransform.Load(XmlReader.Create(finalstream));

            return xslCompiledTransform;
        }
    }
}
