using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using OpalOPCWPF.ViewModels;
using Util;

namespace OpalOPCWPF.Logger
{
    public class GUILogger(LogLevel minimumLogLevel) : ILogger
    {
        private readonly LogLevel _minimumLogLevel = minimumLogLevel;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => _minimumLogLevel <= logLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string message = LoggingUtil.ConstructLogMessage($"{state}", logLevel, exception);
            WeakReferenceMessenger.Default.Send(new LogMessage(message));
        }
    }
}
