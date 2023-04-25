using System.Globalization;
using Controller;
using Model;
using Opc.Ua;

namespace OpcUaSecurityScanner
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            TextWriter output = Console.Out;
            output.WriteLine("OPC UA Console Reference Client");

            output.WriteLine("OPC UA library: {0} @ {1} -- {2}",
                Utils.GetAssemblyBuildNumber(),
                Utils.GetAssemblyTimestamp().ToString("G", CultureInfo.InvariantCulture),
                Utils.GetAssemblySoftwareVersion());

            ICollection<OpcTarget> targets = DiscoveryController.DiscoverTargets(new Uri("opc.tcp://echo:53530"));
            ICollection<OpcTarget> testedTargets = new List<OpcTarget>();

            foreach (OpcTarget target in targets)
            {
                testedTargets.Add(SecurityTestController.TestOpcTargetSecurity(target));
            }

            //ReportController.GenerateReport(testedTargets);
        }
    }
}
