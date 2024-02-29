using Microsoft.Extensions.Logging;

namespace Logger
{
    internal class CLILoggerProvider(LogLevel logLevel) : ILoggerProvider
    {
        private readonly ILogger _logger = new CLILogger(logLevel);

        public ILogger CreateLogger(string categoryName) => _logger;
        public void Dispose() => throw new NotImplementedException();
    }
}
