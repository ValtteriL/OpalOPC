using Microsoft.Extensions.Logging;

namespace Util
{
    public class LoggingUtil
    {
        public static string ConstructLogMessage(string state, LogLevel logLevel, Exception? exception)
        {
            string entry = $"{GetTimestamp()} {logLevel}: {state}{exception}";

            // make sure entry is a single line
            entry = entry.Replace("\n", " ").Replace("\r", " ");

            return entry;
        }

        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss").Replace(".", ":");
        }
    }
}
