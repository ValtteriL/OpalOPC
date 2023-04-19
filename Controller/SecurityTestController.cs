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

            // "The SecurityMode should be ′Sign′ or ′SignAndEncrypt′." - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            IEnumerable<OpcTarget.Endpoint> NoneSecurityModeEndpoints = opcTarget.GetEndpointsBySecurityMode(MessageSecurityMode.None);
            IEnumerable<OpcTarget.Endpoint> invalidSecurityModeEndpoints = opcTarget.GetEndpointsBySecurityMode(MessageSecurityMode.Invalid);

            foreach (OpcTarget.Endpoint endpoint in NoneSecurityModeEndpoints)
            {
                endpoint.Issues.Add(new Issue("SECURITY MODE NONE"));
            }
            foreach (OpcTarget.Endpoint endpoint in invalidSecurityModeEndpoints)
            {
                endpoint.Issues.Add(new Issue("SECURITY MODE INVALID"));
            }

            // Basic256 and Basic128Rsa15 are deprecated - https://profiles.opcfoundation.org/profilefolder/474
            IEnumerable<OpcTarget.Endpoint> Basic128Rsa15Endpoints = opcTarget.GetEndpointsBySecurityPolicyUri("http://opcfoundation.org/UA/SecurityPolicy#Basic128Rsa15");
            IEnumerable<OpcTarget.Endpoint> Basic256Endpoints = opcTarget.GetEndpointsBySecurityPolicyUri("http://opcfoundation.org/UA/SecurityPolicy#Basic256");

            foreach (OpcTarget.Endpoint endpoint in Basic128Rsa15Endpoints)
            {
                endpoint.Issues.Add(new Issue("BASIC 128"));
            }
            foreach (OpcTarget.Endpoint endpoint in Basic256Endpoints)
            {
                endpoint.Issues.Add(new Issue("BASIC 256"));
            }

            return opcTarget;
        }


        // populate opcTarget with auth test results
        private static OpcTarget TestAuth(OpcTarget opcTarget)
        {
            // TODO: CHECK ANONYMOUS AUTH
            // "′anonymous′ should be used only for accessing non-critical UA server resources" - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            // try anonymous authentication
            IEnumerable<OpcTarget.Endpoint> anonymousEndpoints = opcTarget.GetEndpointsByUserTokenType(UserTokenType.Anonymous);
            foreach (OpcTarget.Endpoint endpoint in anonymousEndpoints)
            {
                endpoint.Issues.Add(new Issue("ANONYMOUS"));
            }

            // TODO: CHECK COMMON CREDENTIALS - if username-pass
            // brute username - pass
            IEnumerable<OpcTarget.Endpoint> usernameEndpoints = opcTarget.GetEndpointsByUserTokenType(UserTokenType.UserName);
            foreach (OpcTarget.Endpoint endpoint in usernameEndpoints)
            {
                Console.WriteLine($"USERNAME {endpoint.EndpointUrl}");
            }

            // TODO: CHECK self signed cert - if certificate
            // "self-signed certificates should not be trusted automatically" - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            // try authentication with self-signed certificate
            IEnumerable<OpcTarget.Endpoint> certificateEndpoints = opcTarget.GetEndpointsByUserTokenType(UserTokenType.Certificate);
            foreach (OpcTarget.Endpoint endpoint in certificateEndpoints)
            {
                Console.WriteLine($"CERTIFICATE {endpoint.EndpointUrl}");
            }

            return opcTarget;
        }

        // populate opcTarget with access control results
        private static OpcTarget TestAccessControl(OpcTarget opcTarget)
        {
            // TODO: CHECK FOR READ/WRITE ACCESS for all users that are able to authenticate
            return opcTarget;
        }

        private static bool IdentityCanLogin(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            var session = Util.ConnectionUtil.StartSession(endpointDescription, userIdentity).Result;
            bool result = false;
            if (session.Connected)
            {
                result = true;
            }

            session.Close();
            session.Dispose();

            return result;
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