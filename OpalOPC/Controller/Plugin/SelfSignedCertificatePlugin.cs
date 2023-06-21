using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Util;

namespace Plugin
{
    public class SelfSignedCertificatePlugin : Plugin
    {
        // "self-signed certificates should not be trusted automatically"
        //      - https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/
        // check if self-signed certificates are accepted
        private static PluginId _pluginId = PluginId.SelfSignedCertificate;
        private static string _category = PluginCategories.Authentication;
        private static string _issueTitle = "Self signed client certificates trusted";

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:L/A:N
        private static double _severity = 5.4;

        public SelfSignedCertificatePlugin(ILogger logger) : base(logger, _pluginId, _category, _issueTitle, _severity) { }

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing if {target.ApplicationName} accepts self signed certificate");

            Parallel.ForEach(target.GetEndpointsBySecurityPolicyUriNot(SecurityPolicies.None), endpoint =>
                {
                    if (SelfSignedCertAccepted(endpoint.EndpointDescription).Result)
                    {
                        _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} accepts self-signed client certificates");
                        endpoint.Issues.Add(CreateIssue());
                    }
                });

            return target;
        }

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

    }
}