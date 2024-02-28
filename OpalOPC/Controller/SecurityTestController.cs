using Microsoft.Extensions.Logging;
using Model;
using Plugin;
using Util;

namespace Controller
{

    public interface ISecurityTestController
    {
        ICollection<Target> TestTargetSecurity(ICollection<Target> opcTargets, AuthenticationData authenticationData);
    }

    public class SecurityTestController(ILogger<ISecurityTestController> logger, ITaskUtil taskUtil, IPluginRepository pluginRepository) : ISecurityTestController
    {
        private ICollection<IPlugin> _securityTestPlugins = [];


        // Run all security tests and return result-populated opcTarget
        public ICollection<Target> TestTargetSecurity(ICollection<Target> opcTargets, AuthenticationData authenticationData)
        {
            _securityTestPlugins = pluginRepository.GetAll(authenticationData);

            logger.LogTrace("{Message}", $"Loaded {_securityTestPlugins.Count} security test plugins");
            logger.LogTrace("{Message}", $"Plugins: {string.Join(", ", _securityTestPlugins.Select(p => (int)p.pluginId))}");

            logger.LogDebug("{Message}", $"Starting security tests of {opcTargets.Count} targets");

            Parallel.ForEach(opcTargets, new ParallelOptions { MaxDegreeOfParallelism = 10 }, target =>
            {
                logger.LogDebug("{Message}", $"Testing {target.ApplicationName} ({target.ProductUri})");

                try
                {
                    foreach (Server server in target.Servers)
                    {
                        logger.LogTrace("{Message}", $"Testing endpoint {server.DiscoveryUrl} of {target.ApplicationName}");
                        TestEndpointSecurity(server);

                        if (server.securityTestSessions.Count == 0)
                        {
                            logger.LogWarning("{Message}", $"Cannot authenticate to {target.ApplicationName}. Skipping post-authentication tests");
                        }
                    }
                }
                catch (Exception e)
                {
                    string msg = $"Unknown exception scanning {target.ApplicationName}: {e}";
                    logger.LogError("{Message}", msg);

                    if (target.Servers.Count != 0)
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

            if (server.securityTestSessions.Count != 0)
            {
                TestEndpointPostAuth(server);
            }

            logger.LogTrace("{Message}", $"Closing sessions");
            foreach (ISecurityTestSession securityTestSession in server.securityTestSessions)
            {
                securityTestSession.Dispose();
            }
        }

        private List<ISecurityTestSession> TestEndpointPreauth(Server server)
        {
            List<ISecurityTestSession> securityTestSessions = [];

            logger.LogTrace("{Message}", $"Starting pre-authentication tests");
            foreach (IPreAuthPlugin preauthPlugin in GetPluginsByType(Plugintype.PreAuthPlugin).Cast<IPreAuthPlugin>())
            {
                taskUtil.CheckForCancellation();
                (Issue? preauthissue, ICollection<ISecurityTestSession> preauthsessions) = preauthPlugin.Run(server.DiscoveryUrl, server.EndpointDescriptions);

                securityTestSessions.AddRange(preauthsessions);
                if (preauthissue != null) server.AddIssue(preauthissue);
            }
            logger.LogTrace("{Message}", $"Finished pre-authentication tests");

            return securityTestSessions;
        }

        private void TestEndpointPostAuth(Server server)
        {
            logger.LogTrace("{Message}", $"Starting post-authentication tests");

            TestEndpointSessionCredentials(server);

            foreach (IPostAuthPlugin postAuthPlugin in GetPluginsByType(Plugintype.PostAuthPlugin).Cast<IPostAuthPlugin>())
            {
                taskUtil.CheckForCancellation();
                Issue? postauthIssue = postAuthPlugin.Run(server.securityTestSessions.First().Session);
                if (postauthIssue != null) server.AddIssue(postauthIssue);
            }

            foreach (IMultipleIssuesPostAuthPlugin multipleIssuesPostAuthPlugin in GetPluginsByType(Plugintype.PostAuthMultipleIssuesPlugin).Cast<IMultipleIssuesPostAuthPlugin>())
            {
                taskUtil.CheckForCancellation();
                ICollection<Issue> postauthIssues = multipleIssuesPostAuthPlugin.Run(server.securityTestSessions.First().Session).Result;
                foreach (Issue issue in postauthIssues)
                {
                    server.AddIssue(issue);
                }
            }

            logger.LogTrace("{Message}", $"Finished post-authentication tests");
        }

        private void TestEndpointSessionCredentials(Server server)
        {
            foreach (ISessionCredentialPlugin sessionCredentialPlugin in GetPluginsByType(Plugintype.SessionCredentialPlugin).Cast<ISessionCredentialPlugin>())
            {
                taskUtil.CheckForCancellation();
                Issue? postauthIssue = sessionCredentialPlugin.Run(server.securityTestSessions);
                if (postauthIssue != null) server.AddIssue(postauthIssue);
            }
        }

        private List<IPlugin> GetPluginsByType(Plugintype plugintype) => _securityTestPlugins.Where(p => p.Type == plugintype).ToList();
    }
}
