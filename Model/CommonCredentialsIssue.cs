using Opc.Ua;

namespace Model
{
    public class CommonCredentialsIssue : Issue
    {
        public string? username { get; }
        public string? password { get; }
        public NodeIdCollection? roleIds { get; }

        // parameterless constructor for XML serializer
        internal CommonCredentialsIssue()
        {}

        public CommonCredentialsIssue(string username, string password, NodeIdCollection roleIds, string title, string description) : base(title, description)
        {
            this.username = username;
            this.password = password;
            this.roleIds = roleIds;
        }
    }
}