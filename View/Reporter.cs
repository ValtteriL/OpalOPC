using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;
using Model;
using Opc.Ua;

namespace View
{
    public interface IReporter
    {
        public void printJSONReport(Report report, string filename);
        public void printXMLReport(Report report, string filename);
        public void printBanner();
        public void printLibraryVersion();
    }

    public class Reporter : IReporter
    {
        private Stream outputStream;

        public Reporter(Stream outputStream)
        {
            this.outputStream = outputStream;
        }

        public void printXMLReport(Report report, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(report.GetType());
            serializer.Serialize(outputStream, report);
        }

        public void printJSONReport(Report report, string filename)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            Console.WriteLine(JsonSerializer.Serialize(report, options));
        }

        public void printBanner()
        {
            Console.WriteLine("Opal OPC 1.00 ( https://opalopc.app )");
        }

        public void printLibraryVersion()
        {
            Console.WriteLine("OPC UA library: "
            + $"{Utils.GetAssemblyBuildNumber()} @ "
            + $"{Utils.GetAssemblyTimestamp().ToString("G", CultureInfo.InvariantCulture)} -- "
            + $"{Utils.GetAssemblySoftwareVersion()}");
        }
    }
}