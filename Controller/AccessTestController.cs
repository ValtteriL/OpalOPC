using Model;
using Opc.Ua;

namespace Controller
{

    public static class AccessTestController
    {


        // populate opcTarget with auth test results
        public static OpcTarget TestAuth(OpcTarget opcTarget)
        {
            IEnumerable<OpcTarget.Endpoint> kkk = opcTarget.GetEndpointsBySecurityMode(MessageSecurityMode.None);
            foreach (OpcTarget.Endpoint f in kkk)
            {
                Console.WriteLine($"SecurityMode NONE {f.EndpointUrl}");
            }

            IEnumerable<OpcTarget.Endpoint> rrr = opcTarget.GetEndpointsByUserTokenType(UserTokenType.Anonymous);
            foreach (OpcTarget.Endpoint f in rrr)
            {
                Console.WriteLine($"ANONYMOUS {f.EndpointUrl}");
            }

            IEnumerable<OpcTarget.Endpoint> www = opcTarget.GetEndpointsBySecurityPolicyUri(SecurityPolicies.None);
            foreach (OpcTarget.Endpoint f in www)
            {
                Console.WriteLine($"Securitypolicy NONE {f.EndpointUrl}");
            }


            // TODO: CHECK COMMON CREDENTIALS - if username-pass
            // TODO: CHECK self signed cert - if certificate
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