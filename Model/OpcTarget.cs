namespace Model
{
    public class OpcTarget
    {

        ApplicationType Type;
        string Name;
        string ApplicationUri;
        string ProductUri;
        collection<OpcEndpoint> endpoints;
    }

    public class OpcEndpoint
    {
        string SecurityPolicyUri;
        string SecurityMode;
        collection <UserTokenPolicy> UserTokenPolicies;
        Collection<OpcAccessControl> AccessControlPolicies;
    }

    public class OpcAccessControl
    {
        AccessControlPolicy policy;
        <string, boolean, boolean> <-- property, read, write

    }
}