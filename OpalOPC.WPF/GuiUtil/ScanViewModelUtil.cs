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
    }

    public class ScanViewModelUtil : IScanViewModelUtil
    {
        public AuthenticationData GetAuthenticationData()
        {
            return WeakReferenceMessenger.Default.Send<AuthenticationDataRequestMessage>();
        }
    }
}
