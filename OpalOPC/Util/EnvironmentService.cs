namespace Util
{
    public interface IEnvironmentService
    {
        string? GetEnvironmentVariable(string variable);
    }
    public class EnvironmentService : IEnvironmentService
    {
        public string? GetEnvironmentVariable(string variable) => Environment.GetEnvironmentVariable(variable);
    }
}
