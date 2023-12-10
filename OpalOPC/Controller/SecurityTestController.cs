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
                    foreach (Server server in target.Servers)
                    {
                        _logger.LogTrace("{Message}", $"Testing endpoint {server.DiscoveryUrl} of {target.ApplicationName}");
                        TestEndpointSecurity(server);

                        if (!server.securityTestSessions.Any())
                        {
                            _logger.LogWarning("{Message}", $"Cannot authenticate to {target.ApplicationName}. Skipping post-authentication tests");
                        }
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

        private void TestEndpointSecurity(Server server)
        {
            // run first pre-auth plugins, and save opened sessions
            // run then post-auth plugins
            // finally close sessions
            // return number of sessions

            ICollection<ISecurityTestSession> securityTestSessions = TestEndpointPreauth(server);
            foreach (ISecurityTestSession session in securityTestSessions)
            {
                server.AddSecurityTestSession(session);
            }

            if (server.securityTestSessions.Any())
            {
                TestEndpointPostAuth(server);
            }

            _logger.LogTrace("{Message}", $"Closing sessions");
            foreach (ISecurityTestSession securityTestSession in server.securityTestSessions)
            {
                securityTestSession.Dispose();
            }
        }

        private ICollection<ISecurityTestSession> TestEndpointPreauth(Server server)
        {
            List<ISecurityTestSession> securityTestSessions = new();

            _logger.LogTrace("{Message}", $"Starting pre-authentication tests");
            foreach (IPreAuthPlugin preauthPlugin in GetPluginsByType(Plugintype.PreAuthPlugin).Cast<IPreAuthPlugin>())
            {
                TaskUtil.CheckForCancellation(_token);
                (Issue? preauthissue, ICollection<ISecurityTestSession> preauthsessions) = preauthPlugin.Run(server.DiscoveryUrl, server.EndpointDescriptions);

                securityTestSessions.AddRange(preauthsessions);
                if (preauthissue != null) server.AddIssue(preauthissue);
            }
            _logger.LogTrace("{Message}", $"Finished pre-authentication tests");

            return securityTestSessions;
        }

        private void TestEndpointPostAuth(Server server)
        {
            _logger.LogTrace("{Message}", $"Starting post-authentication tests");

            TestEndpointSessionCredentials(server);

            foreach (IPostAuthPlugin postAuthPlugin in GetPluginsByType(Plugintype.PostAuthPlugin).Cast<IPostAuthPlugin>())
            {
                TaskUtil.CheckForCancellation(_token);
                Issue? postauthIssue = postAuthPlugin.Run(server.securityTestSessions.First().Session);
                if (postauthIssue != null) server.AddIssue(postauthIssue);
            }
            _logger.LogTrace("{Message}", $"Finished post-authentication tests");
        }

        private void TestEndpointSessionCredentials(Server server)
        {
            foreach (ISessionCredentialPlugin sessionCredentialPlugin in GetPluginsByType(Plugintype.SessionCredentialPlugin).Cast<ISessionCredentialPlugin>())
            {
                TaskUtil.CheckForCancellation(_token);
                Issue? postauthIssue = sessionCredentialPlugin.Run(server.securityTestSessions);
                if (postauthIssue != null) server.AddIssue(postauthIssue);
            }
        }

        private ICollection<IPlugin> GetPluginsByType(Plugintype plugintype)
        {
            return _securityTestPlugins.Where(p => p.Type == plugintype).ToList();
        }
    }
}
