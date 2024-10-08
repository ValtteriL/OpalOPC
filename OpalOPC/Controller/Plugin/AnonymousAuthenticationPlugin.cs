using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Plugin
{
    public class AnonymousAuthenticationPlugin(ILogger logger, IConnectionUtil connectionUtil, AuthenticationData authenticationData) : PreAuthPlugin(logger, s_pluginId, s_category, s_issueTitle, s_severity)
    {
        // "′anonymous′ should be used only for accessing non-critical UA server resources"
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
        // try anonymous authentication
        private static readonly PluginId s_pluginId = PluginId.AnonymousAuthentication;
        private static readonly string s_category = PluginCategories.Authentication;
        private static readonly string s_issueTitle = "Anonymous authentication enabled";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:L
        private static readonly double s_severity = 7.3;

        public override (Issue?, ICollection<ISecurityTestSession>) Run(string discoveryUrl, EndpointDescriptionCollection endpointDescriptions)
        {
            _logger.LogTrace("{Message}", $"Testing {discoveryUrl} for anonymous access");

            List<EndpointDescription> anonymousEndpoints = endpointDescriptions.FindAll(e => e.UserIdentityTokens.Any(t => t.TokenType == UserTokenType.Anonymous));
            EndpointDescription? anonymousEndpointNoApplicationAuthentication = anonymousEndpoints.Find(e => e.SecurityPolicyUri == SecurityPolicies.None);
            EndpointDescription? anonymousEndpointWithApplicationAuthentication = anonymousEndpoints.Find(e => e.SecurityPolicyUri != SecurityPolicies.None);

            List<ISecurityTestSession> sessions = [];

            EndpointDescription? endPointToTryWithoutProvidedAppCertificates = anonymousEndpointNoApplicationAuthentication ?? anonymousEndpointWithApplicationAuthentication;
            if (endPointToTryWithoutProvidedAppCertificates == null)
            {
                return (null, sessions);
            }

            // try connecting anonymously without or with self-signed app cert
            try
            {
                ISecurityTestSession session = connectionUtil.StartSession(endPointToTryWithoutProvidedAppCertificates, new UserIdentity()).Result;
                sessions.Add(session);
                return (CreateIssue(), sessions);
            }
            catch (Exception)
            {

            }

            // if not working, try application certificates one by one until one works or they run out
            if (anonymousEndpointWithApplicationAuthentication == null)
            {
                return (null, sessions);
            }

            foreach (Opc.Ua.CertificateIdentifier applicationCertificate in authenticationData.applicationCertificates)
            {
                // Open a session - swallow exceptions - endpoint messagesecuritymode may be incompatible for this specific
                try
                {
                    ISecurityTestSession session = connectionUtil.StartSession(anonymousEndpointWithApplicationAuthentication, new UserIdentity(), applicationCertificate).Result;
                    sessions.Add(session);
                    return (CreateIssue(), sessions);
                }
                catch (Exception)
                {

                }
            }

            // if still no sessions, return not vulnerable despite seeing anonymous endpoint
            return (null, sessions);
        }


    }
}
