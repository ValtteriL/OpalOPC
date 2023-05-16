using Opc.Ua;

namespace Model
{
    public class CommonCredentialsIssue : Issue
    {
        public string? username { get; }
        public string? password { get; }

        // parameterless constructor for XML serializer
        internal CommonCredentialsIssue()
        { }

        public CommonCredentialsIssue(int pluginId, string title, double severity, string username, string password) : base(pluginId, title, severity)
        {
            this.username = username;
            this.password = password;
        }
    }
}