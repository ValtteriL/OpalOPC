using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using Xunit;

namespace Tests.GUI
{
    public class GuiTest : GuiTestBase
    {
        private const string AddTargetsTextBox = "AddTargetsTextBox";
        private const string AddTargetButton = "AddTargetButton";
        private const string NormalVerbosityRadioButton = "NormalVerbosityRadioButton";
        private const string DebugVerbosityRadioButton = "DebugVerbosityRadioButton";
        private const string TraceVerbosityRadioButton = "TraceVerbosityRadioButton";
        private const string StartButton = "StartButton";
        private const string StopButton = "StopButton";
        private const string NavbarAbout = "NavbarAbout";
        private const string NavbarConfiguration = "NavbarConfiguration";
        private const string NavbarScan = "NavbarScan";

        private const string ApplicationCertfileTextBox = "ApplicationCertfileTextBox";
        private const string ApplicationCertPrivateKeyTextBox = "ApplicationCertPrivateKeyTextBox";
        private const string AddApplicationCertificateButton = "AddApplicationCertificateButton";
        private const string UserAuthenticationUsernameTextBox = "UserAuthenticationUsernameTextBox";
        private const string UserAuthenticationPasswordTextBox = "UserAuthenticationPasswordTextBox";
        private const string AddUserAuthenticationButton = "AddUserAuthenticationButton";
        private const string UserAuthenticationCertificateTextBox = "UserAuthenticationCertificateTextBox";
        private const string UserAuthenticationPrivateKeyTextBox = "UserAuthenticationPrivateKeyTextBox";
        private const string AddUserCertificateButton = "AddUserCertificateButton";
        private const string BruteforceUsernameTextBox = "BruteforceUsernameTextBox";
        private const string BruteforcePasswordTextBox = "BruteforcePasswordTextBox";
        private const string AddBruteforceCredentialsButton = "AddBruteforceCredentialsButton";
        private const string OutputLocationTextBox = "OutputLocationTextBox";


        private const string DeleteApplicationCertificateButton = "DeleteApplicationCertificateButton";
        private const string DeleteUserAuthenticationCredentialsButton = "DeleteUserAuthenticationCredentialsButton";
        private const string DeleteUserAuthenticationCertificateButton = "DeleteUserAuthenticationCertificateButton";
        private const string DeleteBruteforceCredentialsButton = "DeleteBruteforceCredentialsButton";

        private const string AboutWindow = "AboutWindow";

        private string paste = OpenQA.Selenium.Keys.Control + "v";

        private void AddTarget(string target)
        {
            AppSession.FindElementByAccessibilityId(AddTargetsTextBox).SendKeys(target);
            AppSession.FindElementByAccessibilityId(AddTargetButton).Click();
        }

        private void RemoveTarget(string target)
        {
            string deleteButtonAutomationId = target;
            if (!target.StartsWith("opc.tcp://"))
                deleteButtonAutomationId = "opc.tcp://" + target;

            AppSession.FindElementByAccessibilityId(deleteButtonAutomationId).Click();
        }

        private void SetValue(WindowsElement element, string value)
        {
            SetClipboardText(value);
            element.SendKeys(paste);
        }

        private void AddApplicationAuthentication(string certFile, string privkeyFile)
        {
            SetValue(AppSession.FindElementByAccessibilityId(ApplicationCertfileTextBox), certFile);
            SetValue(AppSession.FindElementByAccessibilityId(ApplicationCertPrivateKeyTextBox), privkeyFile);

            WindowsElement addApplicationCertificateButton = AppSession.FindElementByAccessibilityId(AddApplicationCertificateButton);
            addApplicationCertificateButton.Click();
            addApplicationCertificateButton.Click();
        }

        private void AddUserAuthentication(string username, string password)
        {
            SetValue(AppSession.FindElementByAccessibilityId(UserAuthenticationUsernameTextBox), username);
            SetValue(AppSession.FindElementByAccessibilityId(UserAuthenticationPasswordTextBox), password);

            WindowsElement addUserAuthenticationButton = AppSession.FindElementByAccessibilityId(AddUserAuthenticationButton);
            addUserAuthenticationButton.Click();
            addUserAuthenticationButton.Click();
        }

        private void AddUserAuthenticationCertificate(string certFile, string privkeyFile)
        {
            SetValue(AppSession.FindElementByAccessibilityId(UserAuthenticationCertificateTextBox), certFile);
            SetValue(AppSession.FindElementByAccessibilityId(UserAuthenticationPrivateKeyTextBox), privkeyFile);

            WindowsElement addUserCertificateButton = AppSession.FindElementByAccessibilityId(AddUserCertificateButton);
            addUserCertificateButton.Click();
            addUserCertificateButton.Click();
        }

        private void AddBruteforceCredentials(string username, string password)
        {
            SetValue(AppSession.FindElementByAccessibilityId(BruteforceUsernameTextBox), username);
            SetValue(AppSession.FindElementByAccessibilityId(BruteforcePasswordTextBox), password);

            WindowsElement addBruteforceCredentialsButton = AppSession.FindElementByAccessibilityId(AddBruteforceCredentialsButton);
            addBruteforceCredentialsButton.Click();
            addBruteforceCredentialsButton.Click();
        }

        private void SetClipboardText(string text)
        {
            Thread thread = new Thread(() => Clipboard.SetText(text));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
        }

        [Fact]
        public void EndToEnd()
        {

            // Maximize window
            AppSession.Keyboard.SendKeys(Keys.Command + Keys.ArrowUp + Keys.Command);

            // add targets
            string target = "kekekeke";
            string realTarget = "echo:53530";
            AddTarget(target);
            AddTarget(realTarget);
            RemoveTarget(target);

            // toggle all verbosity buttons
            AppSession.FindElementByAccessibilityId(NormalVerbosityRadioButton).Click();
            AppSession.FindElementByAccessibilityId(DebugVerbosityRadioButton).Click();
            AppSession.FindElementByAccessibilityId(TraceVerbosityRadioButton).Click();

            // add report location
            string outputLocation = "opalopc-report-guitest.html";
            SetClipboardText(outputLocation);
            WindowsElement outputLocationTextBox = AppSession.FindElementByAccessibilityId(OutputLocationTextBox);
            outputLocationTextBox.Clear();
            outputLocationTextBox.SendKeys(paste);

            // open & close about
            AppSession.FindElementByAccessibilityId(NavbarAbout).Click();

            // close about window
            DesktopSession.FindElementByAccessibilityId(AboutWindow).SendKeys(OpenQA.Selenium.Keys.Alt + OpenQA.Selenium.Keys.F4);


            // Navigate to configuration
            AppSession.FindElementByAccessibilityId(NavbarConfiguration).Click();

            // set application authentication (delete & re-add)
            string certFile = "C:\\Users\\valtteri\\source\\repos\\opc-ua-security-scanner\\test-resources\\sample-certificate.pem";
            string privkeyFile = "C:\\Users\\valtteri\\source\\repos\\opc-ua-security-scanner\\test-resources\\sample-privatekey.pem";
            AddApplicationAuthentication(certFile, privkeyFile);
            AppSession.FindElementByAccessibilityId(DeleteApplicationCertificateButton).Click();
            AddApplicationAuthentication(certFile, privkeyFile);

            // set user authentication (delete & re-add)
            string username = "username";
            string password = "password";
            AddUserAuthentication(username, password);
            AppSession.FindElementByAccessibilityId(DeleteUserAuthenticationCredentialsButton).Click();
            AddUserAuthentication(username, password);

            // set user authentication certificate (delete & re-add)
            AddUserAuthenticationCertificate(certFile, privkeyFile);
            AppSession.FindElementByAccessibilityId(DeleteUserAuthenticationCertificateButton).Click();
            AddUserAuthenticationCertificate(certFile, privkeyFile);

            // set brute force (delete & re-add)
            AddBruteforceCredentials(username, password);
            AppSession.FindElementByAccessibilityId(DeleteBruteforceCredentialsButton).Click();
            AddBruteforceCredentials(username, password);

            // navigate to scan
            AppSession.FindElementByAccessibilityId(NavbarScan).Click();

            // run scan
            AppSession.FindElementByAccessibilityId(StartButton).Click();

            // TODO: wait for scan to finish
            System.Threading.Thread.Sleep(10 * 1000);

            // validate report
            Assert.True(System.IO.File.Exists(outputLocation));
            File.Delete(outputLocation);

            // TODO: parse report & validate
        }
    }
}
