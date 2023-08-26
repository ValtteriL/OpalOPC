using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using OpalOPC.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpalOPC.WPF.Logger
{
    public class GUILogger : ILogger
    {
        private LogLevel minimumLogLevel;

        public GUILogger(LogLevel _minimumLogLevel) {
            minimumLogLevel = _minimumLogLevel;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            return minimumLogLevel <= logLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string message = $"{GetTimestamp()} {logLevel}: {state}{exception}";

            WeakReferenceMessenger.Default.Send(new LogMessage(message));
        }

        private string GetTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss").Replace(".", ":");
        }
    }
}
