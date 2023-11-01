using CommunityToolkit.Mvvm.Messaging.Messages;

namespace OpalOPC.WPF.ViewModels
{
    public class LogMessage : ValueChangedMessage<string>
    {
        public LogMessage(string value) : base(value)
        {
        }
    }
}
