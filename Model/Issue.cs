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
        public static Issue SecurityModeNone = new Issue("Security mode None", @"The application allows connections without any transport security. Without transport security all data is sent
         in plaintext. This means that an attacker with access to the traffic can read and modify its contents at will.");
        public static Issue SecurityModeInvalid = new Issue("Security mode Invalid", @"The application reports support for invalid security mode.");
        public static Issue SecurityPolicyBasic128 = new Issue("Security policy Basic 128", @"The application allows Basic 128 based client authentication. Basic 128 has been deprecated
         and it is not recommended to use it.");
        public static Issue SecurityPolicyBasic256 = new Issue("Security policy Basic 256", @"The application allows Basic 256 based client authentication. Basic 256 has been deprecated
         and it is not recommended to use it.");
        public static Issue SecurityPolicyNone = new Issue("Security policy NONE", @"The application supports connections without client authentication. This makes it
         possible to use unauthorized client applications to access the resources.");
        public static Issue SelfSignedCertificateAccepted = new Issue("Self signed client certificate accepted", @"The application trusts self-signed client certificates. This makes it
         possible to use unauthorized client applications to access the resources.");
        public static Issue NotRBACCapable = new Issue("RBAC not supported", @"The application is not capable of Role Basec Access Control (RBAC). This means that all authenticated users
         have the same level of access.");
        public static Issue AuditingDisabled = new Issue("Auditing is disabled", @"Auditing is disabled on the application. This means no audit trail is being generated. Without the trail,
         tracking of activities and abnormal behavior on the application may not be possible.");
        public static Issue AnonymousAuthentication = new Issue("Anonymous authentication", @"Anonymous authenticaton was successful. Anyone can access the application and it is not possible
         to trace actions back to users. It should only be available for accessing non-critical UA application resources.");
        public static CommonCredentialsIssue CommonCredentials(string username, string password, NodeIdCollection roleIds)
            => new CommonCredentialsIssue(username, password, roleIds, "Common credentials", @"The application uses well-known credentials. These are easy for attackers to guess.
         Details: Authentication with credentials {0}:{1} was successful. {2}");

    }
}