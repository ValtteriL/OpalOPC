using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Plugin
{
    public class SelfSignedUserCertificatePlugin : PreAuthPlugin
    {
        // "self-signed certificates should not be trusted automatically"
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
        // check if self-signed certificates are accepted
        private static readonly PluginId s_pluginId = PluginId.SelfSignedUserCertificate;
        private static readonly string s_category = PluginCategories.Authentication;
        private static readonly string s_issueTitle = "Self-signed client user certificates trusted";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:L/A:N
        private static readonly double s_severity = 5.4;

        private readonly ISelfSignedCertificateUtil _selfSignedCertificateUtil;
        private readonly IConnectionUtil _connectionUtil;
        private readonly AuthenticationData _authenticationData;

        public SelfSignedUserCertificatePlugin(ILogger logger, AuthenticationData authenticationData) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _selfSignedCertificateUtil = new SelfSignedCertificateUtil();
            _connectionUtil = new ConnectionUtil();
            _authenticationData = authenticationData;
        }

        public SelfSignedUserCertificatePlugin(ILogger logger, ISelfSignedCertificateUtil selfSignedCertificateUtil, IConnectionUtil connectionUtil, AuthenticationData authenticationData) : base(logger, s_pluginId, s_category, s_issueTitle, s_severity)
        {
            _selfSignedCertificateUtil = selfSignedCertificateUtil;
            _connectionUtil = connectionUtil;
            _authenticationData = authenticationData;
        }

        public override (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions)
        {
            _logger.LogTrace("{Message}", $"Testing if {discoveryUrl} accepts self-signed user certificate");

            List<EndpointDescription> userCertificateEndpoints = endpointDescriptions.FindAll(e => e.UserIdentityTokens.Any(t => t.TokenType == UserTokenType.Certificate));
            EndpointDescription? userCertificateEndpointNoApplicationAuthentication = userCertificateEndpoints.Find(e => e.SecurityPolicyUri == SecurityPolicies.None);
            EndpointDescription? userCertificateEndpointWithApplicationAuthentication = userCertificateEndpoints.Find(e => e.SecurityPolicyUri != SecurityPolicies.None);

            List<ISecurityTestSession> sessions = [];

            EndpointDescription? endpointToTryWithoutOrWithSelfSignedAppCertificate = userCertificateEndpointNoApplicationAuthentication ?? userCertificateEndpointWithApplicationAuthentication;

            UserIdentity userIdentityWithCertificate = new(_selfSignedCertificateUtil.GetCertificate());

            if (endpointToTryWithoutOrWithSelfSignedAppCertificate != null)
            {
                // try with self-signed cert, if not working, try application certificates one by one until one works or they run out
                // Open a session - swallow exceptions - endpoint messagesecuritymode may be incompatible for this specific
                try
                {
                    ISecurityTestSession session = _connectionUtil.StartSession(endpointToTryWithoutOrWithSelfSignedAppCertificate, userIdentityWithCertificate).Result;
                    sessions.Add(session);
                    return (CreateIssue(), sessions);
                }
                catch (Exception)
                {

                }

                return (CreateIssue(), sessions);
            }

            if (userCertificateEndpointWithApplicationAuthentication != null)
            {
                foreach (Opc.Ua.CertificateIdentifier applicationCertificate in _authenticationData.applicationCertificates)
                {
                    // Open a session - swallow exceptions - endpoint messagesecuritymode may be incompatible for this specific
                    try
                    {
                        ISecurityTestSession session = _connectionUtil.StartSession(userCertificateEndpointWithApplicationAuthentication, userIdentityWithCertificate, applicationCertificate).Result;
                        sessions.Add(session);
                        return (CreateIssue(), sessions);
                    }
                    catch (Exception)
                    {

                    }
                }

                return (CreateIssue(), sessions);
            }

            return (null, sessions);
        }
    }
}
