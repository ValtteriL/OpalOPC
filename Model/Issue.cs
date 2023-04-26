namespace Model
{
    public class Issue
    {
        public string Description { get; }

        public Issue(string description)
        {
            this.Description = description;
        }
    }

    public static class Issues
    {
        public static Issue AnonymousAuthentication = new Issue("ANONYMOUS");
        public static Issue SecurityModeNone = new Issue("SECURITY MODE NONE");
        public static Issue SecurityModeInvalid = new Issue("SECURITY MODE INVALID");
        public static Issue SecurityPolicyBasic128 = new Issue("SECURITY POLICY BASIC 128");
        public static Issue SecurityPolicyBasic256 = new Issue("SECURITY POLICY BASIC 256");
        public static Issue SecurityPolicyNone = new Issue("SECURITY POLICY NONE");
        public static Issue SelfSignedCertificateAccepted = new Issue("SELF SIGNED CERTIFICATE ACCEPTED");
        public static Issue CommonCredentials = new Issue("COMMON CREDENTIALS");
        public static Issue NotRBACCapable = new Issue("NOT CAPABLE OF RBAC");
        public static Issue AuditingDisabled = new Issue("AUDITING IS DISABLED");

    }
}