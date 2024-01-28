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

        private void AddApplicationAuthentication(string certFile, string privkeyFile)
        {
            AppSession.FindElementByAccessibilityId(ApplicationCertfileTextBox).SendKeys(certFile);
            AppSession.FindElementByAccessibilityId(ApplicationCertPrivateKeyTextBox).SendKeys(privkeyFile);
            AppSession.FindElementByAccessibilityId(AddApplicationCertificateButton).Click();
        }

        private void AddUserAuthentication(string username, string password)
        {
            AppSession.FindElementByAccessibilityId(UserAuthenticationUsernameTextBox).SendKeys(username);
            AppSession.FindElementByAccessibilityId(UserAuthenticationPasswordTextBox).SendKeys(password);
            AppSession.FindElementByAccessibilityId(AddUserAuthenticationButton).Click();
        }

        private void AddUserAuthenticationCertificate(string certFile, string privkeyFile)
        {
            AppSession.FindElementByAccessibilityId(UserAuthenticationCertificateTextBox).SendKeys(certFile);
            AppSession.FindElementByAccessibilityId(UserAuthenticationPrivateKeyTextBox).SendKeys(privkeyFile);
            AppSession.FindElementByAccessibilityId(AddUserCertificateButton).Click();
        }

        private void AddBruteforceCredentials(string username, string password)
        {
            AppSession.FindElementByAccessibilityId(BruteforceUsernameTextBox).SendKeys(username);
            AppSession.FindElementByAccessibilityId(BruteforcePasswordTextBox).SendKeys(password);
            AppSession.FindElementByAccessibilityId(AddBruteforceCredentialsButton).Click();
        }

        [Fact]
        public void EndToEnd()
        {
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
            AppSession.FindElementByAccessibilityId(OutputLocationTextBox).SendKeys(outputLocation);

            // open & close about
            AppSession.FindElementByAccessibilityId(NavbarAbout).Click();
            AppSession.FindElementByAccessibilityId("Close").Click(); // TODO : fix this

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

            // open report
            AppSession.FindElementByAccessibilityId("OpenReportButton").Click();
            AppSession.FindElementByAccessibilityId("Close").Click();

            // validate report
            Assert.True(System.IO.File.Exists(outputLocation));
            File.Delete(outputLocation);

            // TODO: parse report & validate
        }
    }
}
