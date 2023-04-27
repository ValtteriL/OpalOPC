using System.Globalization;
using System.Text.Json;
using Model;
using Opc.Ua;

namespace View
{
    public interface IReporter
    {
        public void printPdfReport(ICollection<OpcTarget> targets, string filename);
        public void printBanner();
        public void printLibraryVersion();
    }

    public class Reporter : IReporter
    {
        public void printPdfReport(ICollection<OpcTarget> targets, string filename)
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