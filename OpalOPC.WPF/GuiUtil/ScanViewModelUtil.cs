using CommunityToolkit.Mvvm.Messaging;
using Controller;
using Microsoft.Extensions.Logging;
using Model;
using OpalOPC.WPF.Models;

namespace OpalOPC.WPF.GuiUtil
{
    public interface IScanViewModelUtil
    {
        public AuthenticationData GetAuthenticationData();
        public void CheckVersion(ILogger logger);
    }

    public class ScanViewModelUtil : IScanViewModelUtil
    {
        public void CheckVersion(ILogger logger)
        {
            VersionCheckController versionCheckController = new(logger);
            versionCheckController.CheckVersion();
        }

        public AuthenticationData GetAuthenticationData()
        {
            return WeakReferenceMessenger.Default.Send<AuthenticationDataRequestMessage>();
        }
    }
}
