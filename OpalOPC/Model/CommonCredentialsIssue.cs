using Opc.Ua;

namespace Model
{
    public class CommonCredentialsIssue : Issue
    {
        public ICollection<(string, string)>? Credentials { get; }

        // parameterless constructor for XML serializer
        internal CommonCredentialsIssue()
        { }

        public CommonCredentialsIssue(int pluginId, string name, double severity, ICollection<(string, string)> credentials) : base(pluginId, name, severity)
        {
            Credentials = credentials;
        }
    }
}