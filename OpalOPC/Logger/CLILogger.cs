using Microsoft.Extensions.Logging;
using Util;

namespace Logger
{
    internal class CLILogger(LogLevel minimumLogLevel) : ILogger
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
            Console.WriteLine(message);
        }
    }
}
