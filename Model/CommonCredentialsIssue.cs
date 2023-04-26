using Opc.Ua;

namespace Model
{
    public class CommonCredentialsIssue : Issue
    {
        public string username { get; }
        public string password { get; }
        public NodeIdCollection roleIds { get; }

        public CommonCredentialsIssue(string username, string password, NodeIdCollection roleIds, string description) : base(description)
        {
            this.username = username;
            this.password = password;
            this.roleIds = roleIds;
        }
    }
}