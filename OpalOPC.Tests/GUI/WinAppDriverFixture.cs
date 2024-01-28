using System.Diagnostics;

namespace Tests.GUI
{

    public class WinAppDriverFixture : IDisposable
    {
        // the purpose of this class is to start and stop the WinAppDriver process
        // it is a shared context between all gui tests

        private const string WinAppDriverPath = @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";
        private readonly Process _winAppDriverProcess;

        public WinAppDriverFixture()
        {
            // Start WinAppDriver
            _winAppDriverProcess = StartWinAppDriver();
        }

        public void Dispose()
        {
            // Stop the WinAppDriver Process
            if (_winAppDriverProcess != null)
            {
                foreach (Process process in Process.GetProcessesByName("WinAppDriver"))
                {
                    process.Kill();
                }
            }
        }

        private static Process StartWinAppDriver()
        {
            ProcessStartInfo psi = new(WinAppDriverPath)
            {
                UseShellExecute = true
            };

            Process? process = Process.Start(psi);
            return process ?? throw new Exception("Could not start WinAppDriver");
        }
    }
}
