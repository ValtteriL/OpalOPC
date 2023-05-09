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
        { }

        public CommonCredentialsIssue(string username, string password, NodeIdCollection roleIds, string title, string description) : base(title, PrepareDescription(description, username, password, roleIds))
        {
            this.username = username;
            this.password = password;
            this.roleIds = roleIds;
        }

        private static string PrepareDescription(string description, string username, string password, NodeIdCollection roleIds)
        {

            string roleText = "";

            // if there are any roles, add text about them
            if (roleIds.Count > 0)
            {
                string rolesText = String.Join(", ", Array.ConvertAll(roleIds.ToArray(), i => i.ToString()));
                roleText = String.Format("The authenticated user was assigned the following roles: {0}", rolesText);
            }

            return String.Format(description, username, password, roleText);
        }
    }
}