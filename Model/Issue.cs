using Opc.Ua;

namespace Model
{
    public class Issue
    {
        public string? Title { get; set; }
        public string? Description { get; set; }

        // parameterless constructor for XML serializer
        internal Issue()
        { }

        public Issue(string title, string description)
        {
            this.Title = title;
            this.Description = description;
        }
    }

    public static class Issues
    {
        public static Issue SecurityModeNone = new Issue("SECURITY MODE NONE", @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
         It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.");
        public static Issue SecurityModeInvalid = new Issue("SECURITY MODE INVALID", @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
         It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.");
        public static Issue SecurityPolicyBasic128 = new Issue("SECURITY POLICY BASIC 128", @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
         It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.");
        public static Issue SecurityPolicyBasic256 = new Issue("SECURITY POLICY BASIC 256", @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
         It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.");
        public static Issue SecurityPolicyNone = new Issue("SECURITY POLICY NONE", @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
         It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.");
        public static Issue SelfSignedCertificateAccepted = new Issue("SELF SIGNED CERTIFICATE ACCEPTED", @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
         It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.");
        public static Issue NotRBACCapable = new Issue("NOT CAPABLE OF RBAC", @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
         It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.");
        public static Issue AuditingDisabled = new Issue("AUDITING IS DISABLED", @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
         It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.");
        public static Issue AnonymousAuthentication = new Issue("ANONYMOUS AUTHENTICATION", @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
         It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.");
        public static CommonCredentialsIssue CommonCredentials(string username, string password, NodeIdCollection roleIds)
            => new CommonCredentialsIssue(username, password, roleIds, "COMMON CREDENTIALS", @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
         It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.");

    }
}