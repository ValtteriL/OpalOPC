using Model;
using Opc.Ua;
using Util;

namespace Controller
{

    public static class SecurityTestController
    {
        // Run all security tests and return result-populated opcTarget
        public static OpcTarget TestOpcTargetSecurity(OpcTarget opcTarget)
        {
            return TestAccessControl(TestAuth(TestTransportSecurity(opcTarget)));
        }

        // populate opcTarget with transport test results
        private static OpcTarget TestTransportSecurity(OpcTarget opcTarget)
        {

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
            return opcTarget;
        }

        // populate opcTarget with auth test results
        private static OpcTarget TestAuth(OpcTarget opcTarget)
        {
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
            Parallel.ForEach(GetBruteableEndpoints(opcTarget), endpoint =>
            {
                foreach ((string username, string password) in Util.Credentials.CommonCredentials)
                {
                    if (IdentityCanLogin(endpoint.EndpointDescription, new UserIdentity(username, password)).Result)
                    {
                        endpoint.Issues.Add(Issues.CommonCredentials);
                    }
                }
            });

            return opcTarget;
        }

        // Get bruteable endpoints = application authentication is disabled OR self-signed certificates accepted
        private static IEnumerable<Endpoint> GetBruteableEndpoints(OpcTarget opcTarget)
        {
            return opcTarget.GetEndpointsByUserTokenType(UserTokenType.UserName)
                .Where(e => e.Issues.Contains(Issues.SecurityModeNone)
                    || e.Issues.Contains(Issues.SelfSignedCertificateAccepted));
        }

        // populate opcTarget with access control results
        private static OpcTarget TestAccessControl(OpcTarget opcTarget)
        {
            // TODO: CHECK FOR READ/WRITE ACCESS for all users that are able to authenticate
            return opcTarget;
        }

        private async static Task<bool> SelfSignedCertAccepted(EndpointDescription endpointDescription)
        {
            try
            {
                Console.WriteLine($"Trying {endpointDescription.EndpointUrl}...");
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

        private async static Task<bool> IdentityCanLogin(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            try
            {
                ConnectionUtil util = new ConnectionUtil();
                var session = await util.StartSession(endpointDescription, userIdentity);
                bool result = false;
                if (session.Connected)
                {
                    result = true;
                }

                session.Close();
                session.Dispose();

                return result;
            }
            catch (Opc.Ua.ServiceResultException)
            {
                return false;
            }
        }
    }

}