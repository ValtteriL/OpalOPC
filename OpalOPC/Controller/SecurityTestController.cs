using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua.Client;
using Plugin;
using Util;

namespace Controller
{

    public class SecurityTestController
    {

        readonly ILogger _logger;
        readonly ICollection<IPlugin> _securityTestPlugins;
        readonly CancellationToken? _token;

        public SecurityTestController(ILogger logger, ICollection<IPlugin> SecurityTestPlugins, CancellationToken? token = null)
        {
            _logger = logger;
            _securityTestPlugins = SecurityTestPlugins;
            _token = token;
        }


        // Run all security tests and return result-populated opcTarget
        public ICollection<Target> TestTargetSecurity(ICollection<Target> opcTargets)
        {
            _logger.LogTrace("{Message}", $"Loaded {_securityTestPlugins.Count} security test plugins");
            _logger.LogTrace("{Message}", $"Plugins: {string.Join(", ", _securityTestPlugins.Select(p => (int)p.pluginId))}");

            _logger.LogDebug("{Message}", $"Starting security tests of {opcTargets.Count} targets");

            Parallel.ForEach(opcTargets, new ParallelOptions { MaxDegreeOfParallelism = 10 }, target =>
            {
                _logger.LogDebug("{Message}", $"Testing {target.ApplicationName} ({target.ProductUri})");

                try
                {
                    int nSessions = 0;

                    foreach (Endpoint endpoint in GetAllTargetEndpoints(target))
                    {
                        _logger.LogTrace("{Message}", $"Testing endpoint {endpoint.EndpointUrl} of {target.ApplicationName} ({endpoint.EndpointDescription.SecurityPolicyUri})");
                        nSessions += TestEndpointSecurity(endpoint);
                    }

                    if (nSessions == 0)
                    {
                        _logger.LogWarning("{Message}", $"Cannot authenticate to {target.ApplicationName}. Skipping post-authentication tests");
                    }
                }
                catch (Exception e)
                {
                    string msg = $"Unknown exception scanning {target.ApplicationName}: {e}";
                    _logger.LogError("{Message}", msg);

                    if (target.Servers.Any())
                    {
                        target.Servers.First().AddError(new Error(msg));
                    }
                }
            });

            return opcTargets;
        }

        private static ICollection<Endpoint> GetAllTargetEndpoints(Target target)
        {
            List<Endpoint> endpoints = new();

            foreach (ICollection<Endpoint> serverEndpointList in target.Servers.Select(s => s.SeparatedEndpoints))
            {
                endpoints.AddRange(serverEndpointList);
            }

            return endpoints;
        }

        private int TestEndpointSecurity(Endpoint endpoint)
        {
            // run first pre-auth plugins, and save opened sessions
            // run then post-auth plugins
            // finally close sessions
            // return number of sessions

            ICollection<ISecurityTestSession> securityTestSessions = TestEndpointPreauth(endpoint);

            if (!securityTestSessions.Any())
            {
                return securityTestSessions.Count;
            }

            TestEndpointPostAuth(endpoint, securityTestSessions);

            _logger.LogTrace("{Message}", $"Closing sessions");
            foreach (SecurityTestSession securityTestSession in securityTestSessions.Cast<SecurityTestSession>())
            {
                securityTestSession.Dispose();
            }

            return securityTestSessions.Count;
        }

        private ICollection<ISecurityTestSession> TestEndpointPreauth(Endpoint endpoint)
        {
            List<ISecurityTestSession> securityTestSessions = new();

            _logger.LogTrace("{Message}", $"Starting pre-authentication tests");
            foreach (IPreAuthPlugin preauthPlugin in _securityTestPlugins.Where(p => p.Type == Plugintype.PreAuthPlugin).Cast<IPreAuthPlugin>())
            {
                TaskUtil.CheckForCancellation(_token);
                (Issue? preauthissue, ICollection<ISecurityTestSession> preauthsessions) = preauthPlugin.Run(endpoint);

                securityTestSessions.AddRange(preauthsessions);
                if (preauthissue != null) endpoint.Issues.Add(preauthissue);
            }
            _logger.LogTrace("{Message}", $"Finished pre-authentication tests");

            return securityTestSessions;
        }

        private void TestEndpointPostAuth(Endpoint endpoint, ICollection<ISecurityTestSession> securityTestSessions)
        {
            _logger.LogTrace("{Message}", $"Starting post-authentication tests");
            foreach (IPostAuthPlugin postAuthPlugin in _securityTestPlugins.Where(p => p.Type == Plugintype.PostAuthPlugin).Cast<IPostAuthPlugin>())
            {
                TaskUtil.CheckForCancellation(_token);
                Issue? postauthIssue = postAuthPlugin.Run(securityTestSessions.First().Session);
                if (postauthIssue != null) endpoint.Issues.Add(postauthIssue);
            }
            _logger.LogTrace("{Message}", $"Finished post-authentication tests");
        }
    }
}
