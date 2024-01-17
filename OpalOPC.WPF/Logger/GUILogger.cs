using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using OpalOPC.WPF.ViewModels;

namespace OpalOPC.WPF.Logger
{
    public class GUILogger(LogLevel minimumLogLevel) : ILogger
    {
        private readonly LogLevel _minimumLogLevel = minimumLogLevel;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            return _minimumLogLevel <= logLevel;
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

        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss").Replace(".", ":");
        }
    }
}
