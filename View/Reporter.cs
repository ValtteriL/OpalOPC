using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;
using Model;
using Opc.Ua;

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
            serializer.Serialize(outputStream, report);
        }
    }
}