using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using Xunit;

namespace Tests.GUI
{
    public class TestsBase
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string ApplicationPath = @"C:\Users\valtteri\source\repos\opc-ua-security-scanner\OpalOPC.WPF\bin\Debug\net8.0-windows\win-x64\opalopc-gui.exe";
        private const string DeviceName = "WindowsPC";
        private const int WaitForAppLaunch = 5;
        private const string WinAppDriverPath = @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";
        private static Process s_winAppDriverProcess;
        public WindowsDriver<WindowsElement> AppSession { get; private set; }
        public WindowsDriver<WindowsElement> DesktopSession { get; private set; }

        public TestsBase()
        {
            s_winAppDriverProcess = StartWinAppDriver();
            var appiumOptions = new AppiumOptions();
            appiumOptions.AddAdditionalCapability("app", ApplicationPath);
            appiumOptions.AddAdditionalCapability("deviceName", DeviceName);
            appiumOptions.AddAdditionalCapability("ms:waitForAppLaunch", WaitForAppLaunch);
            AppSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appiumOptions);
            Assert.NotNull(AppSession);
            Assert.NotNull(AppSession.SessionId);
            AppSession.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
            AppiumOptions optionsDesktop = new();
            optionsDesktop.AddAdditionalCapability("app", "Root");
            optionsDesktop.AddAdditionalCapability("deviceName", DeviceName);
            DesktopSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), optionsDesktop);
        }

        private static Process StartWinAppDriver()
        {
            ProcessStartInfo psi = new(WinAppDriverPath);
            psi.UseShellExecute = true;
            //psi.Verb = "runas"; // run as administrator
            return Process.Start(psi);
        }

        public void Cleanup()
        {
            // Close the session
            if (AppSession != null)
            {
                AppSession.Close();
                AppSession.Quit();
            }
            // Close the desktopSession
            if (DesktopSession != null)
            {
                DesktopSession.Close();
                DesktopSession.Quit();
            }
        }
        public static void StopWinappDriver()
        {
            // Stop the WinAppDriverProcess
            if (s_winAppDriverProcess != null)
            {
                foreach (Process process in Process.GetProcessesByName("WinAppDriver"))
                {
                    process.Kill();
                }
            }
        }

        protected void SelectAllText()
        {
            Actions action = new(AppSession);
            action.KeyDown(Keys.Control).SendKeys("a");
            action.KeyUp(Keys.Control);
            action.Perform();
        }
        protected void PerformDelete()
        {
            Actions action = new(AppSession);
            action.SendKeys(Keys.Delete);
            action.Perform();
        }

        protected void PerformEnter()
        {
            Actions action = new(AppSession);
            action.SendKeys(Keys.Enter);
            action.Perform();
        }
        protected void WriteText(string text)
        {
            Actions action = new(AppSession);
            action.SendKeys(text);
            action.Perform();
        }

    }
}
