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
        private PluginId _pluginId = PluginId.SelfSignedCertificate;
        private string _category = PluginCategories.Authentication;

        // https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:L/A:N
        private double _severity = 5.4;

        public SelfSignedCertificatePlugin(ILogger logger) : base(logger) { }

        public override Target Run(Target target)
        {
            _logger.LogTrace($"Testing {target.ApplicationName} for Self Signed Certificate acceptance");

            Parallel.ForEach(target.GetEndpointsBySecurityPolicyUriNot(SecurityPolicies.None), endpoint =>
                {
                    if (SelfSignedCertAccepted(endpoint.EndpointDescription).Result)
                    {
                        _logger.LogTrace($"Endpoint {endpoint.EndpointUrl} accepts self-signed client certificates");
                        endpoint.Issues.Add(Issues.SelfSignedCertificateAccepted);
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