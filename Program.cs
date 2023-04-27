using System.Globalization;
using Controller;
using Model;
using Opc.Ua;
using View;

namespace OpcUaSecurityScanner
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IReporter reporter = new Reporter();

            reporter.printBanner();
            reporter.printLibraryVersion();

            ICollection<OpcTarget> targets = DiscoveryController.DiscoverTargets(new Uri("opc.tcp://echo:53530"));
            ICollection<OpcTarget> testedTargets = SecurityTestController.TestOpcTargetSecurity(targets);

            ReportController.GenerateReport(reporter, testedTargets);
        }
    }
}
