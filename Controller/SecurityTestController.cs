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
                Console.WriteLine($"SECURITY MODE NONE: {endpoint.EndpointUrl}");
            }
            foreach (OpcTarget.Endpoint endpoint in invalidSecurityModeEndpoints)
            {
                Console.WriteLine($"SECURITY MODE INVALID: {endpoint.EndpointUrl}");
            }

            // Basic256 and Basic128Rsa15 are deprecated - https://profiles.opcfoundation.org/profilefolder/474
            IEnumerable<OpcTarget.Endpoint> Basic128Rsa15Endpoints = opcTarget.GetEndpointsBySecurityPolicyUri("http://opcfoundation.org/UA/SecurityPolicy#Basic128Rsa15");
            IEnumerable<OpcTarget.Endpoint> Basic256Endpoints = opcTarget.GetEndpointsBySecurityPolicyUri("http://opcfoundation.org/UA/SecurityPolicy#Basic256");

            foreach (OpcTarget.Endpoint endpoint in Basic128Rsa15Endpoints)
            {
                Console.WriteLine($"Basic 128 endpoint: {endpoint.EndpointUrl}");
            }
            foreach (OpcTarget.Endpoint endpoint in Basic256Endpoints)
            {
                Console.WriteLine($"Basic 256 endpoint: {endpoint.EndpointUrl}");
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
                Console.WriteLine($"ANONYMOUS {endpoint.EndpointUrl}");
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
    }

}