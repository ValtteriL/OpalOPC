using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;
using Model;
using Opc.Ua;

namespace View
{
    public interface IReporter
    {
        public void printJSONReport(ICollection<OpcTarget> targets, string filename);
        public void printXMLReport(ICollection<OpcTarget> targets, string filename);
        public void printBanner();
        public void printLibraryVersion();
    }

    public class Reporter : IReporter
    {
        public void printXMLReport(ICollection<OpcTarget> targets, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(targets.GetType(), new XmlRootAttribute("AAAAA"));
            serializer.Serialize(Console.Out, targets);
        }

        public void printJSONReport(ICollection<OpcTarget> targets, string filename)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            Console.WriteLine(JsonSerializer.Serialize(targets, options));
        }

        public void printBanner()
        {
            Console.WriteLine("OPC UA Console Reference Client");
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