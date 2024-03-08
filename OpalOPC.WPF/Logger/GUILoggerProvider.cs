using Microsoft.Extensions.Logging;

namespace OpalOPCWPF.Logger
{
    internal class GUILoggerProvider(LogLevel logLevel) : ILoggerProvider
    {
        private readonly ILogger _logger = new GUILogger(logLevel);

        public ILogger CreateLogger(string categoryName) => _logger;
        public void Dispose() => throw new NotImplementedException();
    }
}
