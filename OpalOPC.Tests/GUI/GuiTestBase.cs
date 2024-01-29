using System.Diagnostics;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using Xunit;

namespace Tests.GUI
{
    public class GuiTestBase : IDisposable, IClassFixture<WinAppDriverFixture>
    {
        // this class has fixture for WinAppDriver, so winappdriver.exe should be running at 127.0.0.1:4723

        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string ApplicationPath = @"C:\Users\valtteri\source\repos\opc-ua-security-scanner\OpalOPC.WPF\bin\Debug\net8.0-windows\win-x64\opalopc-gui.exe";
        private const string DeviceName = "WindowsPC";

        protected WindowsDriver<WindowsElement> AppSession { get; private set; }
        protected WindowsDriver<WindowsElement> DesktopSession { get; private set; }

        public GuiTestBase()
        {
            var appiumOptions = new AppiumOptions();
            appiumOptions.AddAdditionalCapability("app", ApplicationPath);
            appiumOptions.AddAdditionalCapability("deviceName", DeviceName);

            AppSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appiumOptions);

            var desktopOptions = new AppiumOptions();
            desktopOptions.AddAdditionalCapability("app", "Root");
            desktopOptions.AddAdditionalCapability("deviceName", DeviceName);

            DesktopSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), desktopOptions);
        }

        public void Dispose()
        {
            // Close the desktop session
            if (DesktopSession != null)
            {
                DesktopSession.Close();
                DesktopSession.Quit();
            }

            // Close the app session
            if (AppSession != null)
            {
                AppSession.Close();
                AppSession.Quit();
            }
        }
    }
}
