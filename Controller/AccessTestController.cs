using Model;
using Opc.Ua;

namespace Controller
{

    public static class AccessTestController
    {


        // populate opcTarget with auth test results
        public static OpcTarget TestAuth(OpcTarget opcTarget)
        {
            // TODO: CHECK ANONYMOUS AUTH
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
            // try authentication with self-signed certificate
            IEnumerable<OpcTarget.Endpoint> certificateEndpoints = opcTarget.GetEndpointsByUserTokenType(UserTokenType.Certificate);
            foreach (OpcTarget.Endpoint endpoint in certificateEndpoints)
            {
                Console.WriteLine($"CERTIFICATE {endpoint.EndpointUrl}");
            }

            return null;
        }

        // populate opcTarget with access control results
        public static OpcTarget TestAccessControl(OpcTarget opcTarget)
        {
            // TODO: CHECK FOR READ/WRITE ACCESS for all users that are able to authenticate
            return null;
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