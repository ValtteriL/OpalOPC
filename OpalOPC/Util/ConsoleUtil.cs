namespace Util
{
    public interface IConsoleUtil
    {
        public void Write(string message);
        public void WriteLine(string message);
        public string? ReadLine();
    }
    public class ConsoleUtil : IConsoleUtil
    {
        public string? ReadLine() => Console.ReadLine();
        public void Write(string message) => Console.Write(message);
        public void WriteLine(string message) => Console.WriteLine(message);
    }
}
