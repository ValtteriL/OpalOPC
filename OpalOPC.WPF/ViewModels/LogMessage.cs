using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
