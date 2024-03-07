using CommunityToolkit.Mvvm.Messaging.Messages;

namespace OpalOPCWPF.ViewModels
{
    public class LogMessage(string value) : ValueChangedMessage<string>(value)
    {
    }
}
