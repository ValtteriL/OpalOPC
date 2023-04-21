using Model;
using Opc.Ua;

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
            IEnumerable<OpcTarget.Endpoint> NoneSecurityModeEndpoints = opcTarget.GetEndpointsBySecurityMode(MessageSecurityMode.None);
            IEnumerable<OpcTarget.Endpoint> invalidSecurityModeEndpoints = opcTarget.GetEndpointsBySecurityMode(MessageSecurityMode.Invalid);

            foreach (OpcTarget.Endpoint endpoint in NoneSecurityModeEndpoints)
            {
                endpoint.Issues.Add(Issues.SecurityModeNone);
            }
            foreach (OpcTarget.Endpoint endpoint in invalidSecurityModeEndpoints)
            {
                endpoint.Issues.Add(Issues.SecurityModeInvalid);
            }

            // Basic256 and Basic128Rsa15 are deprecated - https://profiles.opcfoundation.org/profilefolder/474
            IEnumerable<OpcTarget.Endpoint> Basic128Rsa15Endpoints = opcTarget.GetEndpointsBySecurityPolicyUri(SecurityPolicies.Basic128Rsa15);
            IEnumerable<OpcTarget.Endpoint> Basic256Endpoints = opcTarget.GetEndpointsBySecurityPolicyUri(SecurityPolicies.Basic256);

            foreach (OpcTarget.Endpoint endpoint in Basic128Rsa15Endpoints)
            {
                endpoint.Issues.Add(Issues.SecurityPolicyBasic128);
            }
            foreach (OpcTarget.Endpoint endpoint in Basic256Endpoints)
            {
                endpoint.Issues.Add(Issues.SecurityPolicyBasic256);
            }

            // If securitypolicy is none, application authentication is disabled (clients do not use certificate)
            // https://opcfoundation.org/forum/opc-certification-and-interoperability-testing/allowing-anonymous-user-access-a-security-breach-in-opc-ua-conversation/
            // https://opcfoundation.org/forum/opc-ua-implementation-stacks-tools-and-samples/rationale-for-server-authenticating-client-certificates/
            IEnumerable<OpcTarget.Endpoint> NoneEndpoints = opcTarget.GetEndpointsBySecurityPolicyUri(SecurityPolicies.None);
            foreach (OpcTarget.Endpoint endpoint in NoneEndpoints)
            {
                endpoint.Issues.Add(Issues.SecurityPolicyNone);
            }

            return opcTarget;
        }


        // populate opcTarget with auth test results
        private static OpcTarget TestAuth(OpcTarget opcTarget)
        {
            // "′anonymous′ should be used only for accessing non-critical UA server resources"
            //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            // try anonymous authentication
            IEnumerable<OpcTarget.Endpoint> anonymousEndpoints = opcTarget.GetEndpointsByUserTokenType(UserTokenType.Anonymous);
            foreach (OpcTarget.Endpoint endpoint in anonymousEndpoints)
            {
                endpoint.Issues.Add(Issues.AnonymousAuthentication);
            }

            // "self-signed certificates should not be trusted automatically"
            //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            // check if self-signed certificates are accepsted
            foreach (OpcTarget.Endpoint endpoint in opcTarget.TargetServers
                .SelectMany(s => s.Endpoints)
                .Where(e => e.SecurityPolicyUri != SecurityPolicies.None))
            {
                if (SelfSignedCertAccepted(endpoint.EndpointDescription).Result)
                {
                    endpoint.Issues.Add(Issues.SelfSignedCertificateAccepted);
                }
            }

            // The following checks only make sense if application authentication is disabled
            //      OR if the server accepts self-signed certificates

            // brute username - pass
            IEnumerable<OpcTarget.Endpoint> bruteEndpoints = opcTarget.GetEndpointsByUserTokenType(UserTokenType.UserName)
                .Where(e => e.SecurityPolicyUri == SecurityPolicies.None
                    || e.Issues.Contains(Issues.SelfSignedCertificateAccepted));
            foreach (OpcTarget.Endpoint endpoint in bruteEndpoints)
            {
                foreach ((string username, string password) in CommonCredentials())
                {
                    if (IdentityCanLogin(endpoint.EndpointDescription, new UserIdentity(username, password)).Result)
                    {
                        endpoint.Issues.Add(Issues.CommonCredentials);
                    }
                }
            }

            return opcTarget;
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
                var session = await Util.ConnectionUtil.StartSession(endpointDescription, new UserIdentity());
                session.Close();
                session.Dispose();
            }
            catch (System.Exception)
            {
                throw;
            }
            return true;
        }

        private async static Task<bool> IdentityCanLogin(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            try
            {
                var session = await Util.ConnectionUtil.StartSession(endpointDescription, userIdentity);
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

        private static List<(string username, string password)> CommonCredentials()
        {
            return new List<(string username, string password)> {

                // from https://github.com/COMSYS/msf-opcua/blob/master/credentials/opcua_credentials_sources.txt
                (username: "HTTPS", password: "password"), // Turck TBEN-L4/5
                (username: "User", password: "Siemens.1"), // SIMATIC S7-1500
                (username: "opcuauser", password: "password"), // Ignition
                (username: "username", password: "password"), // Traeger .NET SDK
                (username: "OpcUaClient", password: "OpcUaClient"), // Siemens Sinumerik Driver
                (username: "john", password: "password1"), // Unified Automation
                (username: "root", password: "secret"), // Unified Automation
                (username: "RD81OPC96", password: "MITSUBISHI"), // MELSEC iQ-R OPC UA
                (username: "simatic", password: "100simatic"), // SIMATIC HMI S7-1500
                (username: "User1", password: "1"), // Beckhof TC3
                (username: "userName", password: "password"), // process-informatik .NET
                (username: "username1", password: "password1"),
                (username: "username2", password: "password2"),
                (username: "username3", password: "password3"),
                (username: "admin", password: "admin"), // IBH Link
                (username: "admin", password: "Admin"), // HEIDENHAIN StateMonitor
                (username: "MyUser", password: "MyPassword"),
                (username: "Administrator", password: "Administrator"), // Bosch Building Integration System
                (username: "user1", password: "password"), // open62541
                (username: "user2", password: "password1"),
                (username: "appuser", password: "demo"), // UA-.NETStandard
                (username: "appadmin", password: "demo"),
                (username: "sysadmin", password: "demo"),
                (username: "user1", password: "password1"), // node-opcua
                (username: "user2", password: "password2"),
                (username: "admin", password: "pass"), // python-opcua
            };

        }
    }

}