using Model;
using Opc.Ua;
using Util;

namespace Controller
{

    public static class SecurityTestController
    {


        // Run all security tests and return result-populated opcTarget
        public static ICollection<OpcTarget> TestOpcTargetSecurity(ICollection<OpcTarget> opcTargets)
        {
            foreach (OpcTarget target in opcTargets)
            {
                TestTLS(TestAuditingRBAC(TestAuth(TestTransportSecurity(target))));
            }

            return opcTargets;
        }

        // populate opcTarget with transport test results
        private static OpcTarget TestTransportSecurity(OpcTarget opcTarget)
        {

            Console.WriteLine("### Testing transport security");

            // "The SecurityMode should be ′Sign′ or ′SignAndEncrypt′."
            //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            IEnumerable<Endpoint> NoneSecurityModeEndpoints = opcTarget.GetEndpointsBySecurityMode(MessageSecurityMode.None);
            IEnumerable<Endpoint> invalidSecurityModeEndpoints = opcTarget.GetEndpointsBySecurityMode(MessageSecurityMode.Invalid);

            foreach (Endpoint endpoint in NoneSecurityModeEndpoints)
            {
                endpoint.Issues.Add(Issues.SecurityModeNone);
            }
            foreach (Endpoint endpoint in invalidSecurityModeEndpoints)
            {
                endpoint.Issues.Add(Issues.SecurityModeInvalid);
            }

            // Basic256 and Basic128Rsa15 are deprecated - https://profiles.opcfoundation.org/profilefolder/474
            IEnumerable<Endpoint> Basic128Rsa15Endpoints = opcTarget.GetEndpointsBySecurityPolicyUri(SecurityPolicies.Basic128Rsa15);
            IEnumerable<Endpoint> Basic256Endpoints = opcTarget.GetEndpointsBySecurityPolicyUri(SecurityPolicies.Basic256);

            foreach (Endpoint endpoint in Basic128Rsa15Endpoints)
            {
                endpoint.Issues.Add(Issues.SecurityPolicyBasic128);
            }
            foreach (Endpoint endpoint in Basic256Endpoints)
            {
                endpoint.Issues.Add(Issues.SecurityPolicyBasic256);
            }

            // If securitypolicy is none, application authentication is disabled (clients do not use certificate)
            // https://opcfoundation.org/forum/opc-certification-and-interoperability-testing/allowing-anonymous-user-access-a-security-breach-in-opc-ua-conversation/
            // https://opcfoundation.org/forum/opc-ua-implementation-stacks-tools-and-samples/rationale-for-server-authenticating-client-certificates/
            IEnumerable<Endpoint> NoneEndpoints = opcTarget.GetEndpointsBySecurityPolicyUri(SecurityPolicies.None);
            foreach (Endpoint endpoint in NoneEndpoints)
            {
                endpoint.Issues.Add(Issues.SecurityPolicyNone);
            }

            return opcTarget;
        }

        // populate opcTarget with TLS test results
        private static OpcTarget TestTLS(OpcTarget opcTarget)
        {
            // TODO
            Console.WriteLine("### Testing TLS");
            return opcTarget;
        }

        // populate opcTarget with auth test results
        private static OpcTarget TestAuth(OpcTarget opcTarget)
        {
            Console.WriteLine("### Testing authentication");

            // "′anonymous′ should be used only for accessing non-critical UA server resources"
            //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            // try anonymous authentication
            IEnumerable<Endpoint> anonymousEndpoints = opcTarget.GetEndpointsByUserTokenType(UserTokenType.Anonymous);
            foreach (Endpoint endpoint in anonymousEndpoints)
            {
                endpoint.Issues.Add(Issues.AnonymousAuthentication);
            }

            // "self-signed certificates should not be trusted automatically"
            //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            // check if self-signed certificates are accepted
            Parallel.ForEach(opcTarget.GetEndpointsBySecurityPolicyUriNot(SecurityPolicies.None), endpoint =>
                {
                    if (SelfSignedCertAccepted(endpoint.EndpointDescription).Result)
                    {
                        endpoint.Issues.Add(Issues.SelfSignedCertificateAccepted);
                    }
                });

            // brute username - pass
            Parallel.ForEach(opcTarget.GetBruteableEndpoints(), endpoint =>
            {
                foreach ((string username, string password) in Util.Credentials.CommonCredentials)
                {
                    if (IdentityCanLogin(endpoint.EndpointDescription, new UserIdentity(username, password), out NodeIdCollection roleIds))
                    {
                        endpoint.Issues.Add(Issues.CommonCredentials(username, password, roleIds));
                    }
                }
            });

            return opcTarget;
        }

        // populate opcTarget with access control results
        private static OpcTarget TestAuditingRBAC(OpcTarget opcTarget)
        {
            Console.WriteLine("### Testing access control");

            // take all endpoints where login is possible
            IEnumerable<Endpoint> targetEndpoints = opcTarget.GetLoginSuccessfulEndpoints();

            Parallel.ForEach(targetEndpoints, endpoint =>
            {
                UserIdentity identity;

                // use anonymous if available, otherwise first valid credential
                if (endpoint.UserTokenTypes.Contains(UserTokenType.Anonymous))
                {
                    identity = new UserIdentity();
                }
                else
                {
                    CommonCredentialsIssue credsIssue = (CommonCredentialsIssue) endpoint.Issues.First(i => i.GetType() == typeof(CommonCredentialsIssue));
                    identity = new UserIdentity(username: credsIssue.username, password: credsIssue.password);
                }

                ConnectionUtil util = new ConnectionUtil();
                var session = util.StartSession(endpoint.EndpointDescription, identity).Result;

                // check if auditing enabled
                DataValue auditingValue = session.ReadValue(Util.WellKnownNodes.Server_Auditing);
                if (!(bool)auditingValue.GetValue<System.Boolean>(false))
                {
                    endpoint.Issues.Add(Issues.AuditingDisabled);
                }

                // check if rbac supported (if its advertised in profiles or not)
                DataValue serverProfileArrayValue = session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray);
                string[] serverProfileArray = (string[])serverProfileArrayValue.GetValue<string[]>(new string[0]);
                if (!serverProfileArray.Intersect(RBAC_Profiles).Any())
                {
                    endpoint.Issues.Add(Issues.NotRBACCapable);
                }
            });

            return opcTarget;
        }

        private static ICollection<string> RBAC_Profiles = new List<string> {
                Util.WellKnownProfiles.Security_User_Access_Control_Full,
                Util.WellKnownProfileURIs.Security_User_Access_Control_Full,
                Util.WellKnownProfiles.UAFX_Controller_Server_Profile,
                Util.WellKnownProfileURIs.UAFX_Controller_Server_Profile
            };

        private async static Task<bool> SelfSignedCertAccepted(EndpointDescription endpointDescription)
        {
            try
            {
                ConnectionUtil util = new ConnectionUtil();
                var session = await util.StartSession(endpointDescription, new UserIdentity());
                session.Close();
                session.Dispose();
            }
            catch (Opc.Ua.ServiceResultException e)
            {
                if (e.Message.Contains("Bad_SecurityChecksFailed") || e.Message.Contains("BadSecureChannelClosed"))
                {
                    return false;
                }
                Console.WriteLine($"UNKNOWN EXCEPTION: {endpointDescription.EndpointUrl}");
                throw;
            }
            return true;
        }

        private static bool IdentityCanLogin(EndpointDescription endpointDescription, UserIdentity userIdentity, out NodeIdCollection roleIds)
        {
            roleIds = new NodeIdCollection();

            try
            {
                ConnectionUtil util = new ConnectionUtil();
                var session = util.StartSession(endpointDescription, userIdentity).Result;
                bool result = false;
                if (session.Connected)
                {
                    result = true;
                    roleIds = session.Identity.GrantedRoleIds;
                }

                session.Close();
                session.Dispose();

                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}