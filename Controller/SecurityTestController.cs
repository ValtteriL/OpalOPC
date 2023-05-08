using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Controller
{

    public class SecurityTestController
    {

        ILogger _logger;

        public SecurityTestController(ILogger logger)
        {
            _logger = logger;
        }


        // Run all security tests and return result-populated opcTarget
        public ICollection<Target> TestTargetSecurity(ICollection<Target> opcTargets)
        {
            _logger.LogDebug($"Starting security tests of {opcTargets.Count} targets");

            foreach (Target target in opcTargets)
            {
                _logger.LogDebug($"Testing {target.ApplicationName} ({target.ProductUri})");
                TestTLS(TestAuditingRBAC(TestAuth(TestTransportSecurity(target))));
            }

            return opcTargets;
        }

        // populate opcTarget with transport test results
        private Target TestTransportSecurity(Target opcTarget)
        {

            _logger.LogTrace($"Testing transport security");

            // "The SecurityMode should be ′Sign′ or ′SignAndEncrypt′."
            //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            IEnumerable<Endpoint> NoneSecurityModeEndpoints = opcTarget.GetEndpointsBySecurityMode(MessageSecurityMode.None);
            IEnumerable<Endpoint> invalidSecurityModeEndpoints = opcTarget.GetEndpointsBySecurityMode(MessageSecurityMode.Invalid);

            foreach (Endpoint endpoint in NoneSecurityModeEndpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has security mode None");
                endpoint.Issues.Add(Issues.SecurityModeNone);
            }
            foreach (Endpoint endpoint in invalidSecurityModeEndpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has invalid security mode");
                endpoint.Issues.Add(Issues.SecurityModeInvalid);
            }

            // Basic256 and Basic128Rsa15 are deprecated - https://profiles.opcfoundation.org/profilefolder/474
            IEnumerable<Endpoint> Basic128Rsa15Endpoints = opcTarget.GetEndpointsBySecurityPolicyUri(SecurityPolicies.Basic128Rsa15);
            IEnumerable<Endpoint> Basic256Endpoints = opcTarget.GetEndpointsBySecurityPolicyUri(SecurityPolicies.Basic256);

            foreach (Endpoint endpoint in Basic128Rsa15Endpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} uses Basic128Rsa15");
                endpoint.Issues.Add(Issues.SecurityPolicyBasic128);
            }
            foreach (Endpoint endpoint in Basic256Endpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} uses Basic256");
                endpoint.Issues.Add(Issues.SecurityPolicyBasic256);
            }

            // If securitypolicy is none, application authentication is disabled (clients do not use certificate)
            // https://opcfoundation.org/forum/opc-certification-and-interoperability-testing/allowing-anonymous-user-access-a-security-breach-in-opc-ua-conversation/
            // https://opcfoundation.org/forum/opc-ua-implementation-stacks-tools-and-samples/rationale-for-server-authenticating-client-certificates/
            IEnumerable<Endpoint> NoneEndpoints = opcTarget.GetEndpointsBySecurityPolicyUri(SecurityPolicies.None);
            foreach (Endpoint endpoint in NoneEndpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has security policy None");
                endpoint.Issues.Add(Issues.SecurityPolicyNone);
            }

            return opcTarget;
        }

        // populate opcTarget with TLS test results
        private Target TestTLS(Target opcTarget)
        {
            // TODO
            _logger.LogTrace($"Testing TLS");
            return opcTarget;
        }

        // populate opcTarget with auth test results
        private Target TestAuth(Target opcTarget)
        {
            _logger.LogTrace($"Testing authentication");

            // "′anonymous′ should be used only for accessing non-critical UA server resources"
            //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            // try anonymous authentication
            IEnumerable<Endpoint> anonymousEndpoints = opcTarget.GetEndpointsByUserTokenType(UserTokenType.Anonymous);
            foreach (Endpoint endpoint in anonymousEndpoints)
            {
                _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} allows anonymous authentication");
                endpoint.Issues.Add(Issues.AnonymousAuthentication);
            }

            // "self-signed certificates should not be trusted automatically"
            //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
            // check if self-signed certificates are accepted
            Parallel.ForEach(opcTarget.GetEndpointsBySecurityPolicyUriNot(SecurityPolicies.None), endpoint =>
                {
                    if (SelfSignedCertAccepted(endpoint.EndpointDescription).Result)
                    {
                        _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} accepts self-signed client certificates");
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
                        _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} uses common credentials ({username}:{password})");
                        endpoint.Issues.Add(Issues.CommonCredentials(username, password, roleIds));
                    }
                }
            });

            return opcTarget;
        }

        // populate opcTarget with access control results
        private Target TestAuditingRBAC(Target opcTarget)
        {
            _logger.LogTrace($"Testing access control");

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
                    _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} has auditing disabled");
                    endpoint.Issues.Add(Issues.AuditingDisabled);
                }

                // check if rbac supported (if its advertised in profiles or not)
                DataValue serverProfileArrayValue = session.ReadValue(Util.WellKnownNodes.Server_ServerCapabilities_ServerProfileArray);
                string[] serverProfileArray = (string[])serverProfileArrayValue.GetValue<string[]>(new string[0]);
                if (!serverProfileArray.Intersect(RBAC_Profiles).Any())
                {
                    _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} is not capable of RBAC");
                    endpoint.Issues.Add(Issues.NotRBACCapable);
                }
            });

            return opcTarget;
        }

        private ICollection<string> RBAC_Profiles = new List<string> {
                Util.WellKnownProfiles.Security_User_Access_Control_Full,
                Util.WellKnownProfileURIs.Security_User_Access_Control_Full,
                Util.WellKnownProfiles.UAFX_Controller_Server_Profile,
                Util.WellKnownProfileURIs.UAFX_Controller_Server_Profile
            };

        private async Task<bool> SelfSignedCertAccepted(EndpointDescription endpointDescription)
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
                if (e.Message.Contains("Bad_SecurityChecksFailed")
                    || e.Message.Contains("BadSecureChannelClosed")
                    || e.Message.Contains("BadCertificateUriInvalid"))
                {
                    return false;
                }
                Console.WriteLine($"UNKNOWN EXCEPTION: {endpointDescription.EndpointUrl}");
                throw;
            }
            return true;
        }

        private bool IdentityCanLogin(EndpointDescription endpointDescription, UserIdentity userIdentity, out NodeIdCollection roleIds)
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