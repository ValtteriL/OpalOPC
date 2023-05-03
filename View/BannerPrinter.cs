using System.Globalization;
using Opc.Ua;

namespace View
{
    public interface IBannerPrinter
    {
        public void printBanner();
        public void printLibraryVersion();
    }

    public class BannerPrinter : IBannerPrinter
    {
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