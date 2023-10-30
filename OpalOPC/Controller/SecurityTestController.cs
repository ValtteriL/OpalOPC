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
            _logger.LogTrace($"Loaded {_securityTestPlugins.Count} security test plugins");
            _logger.LogTrace($"Plugins: {string.Join(", ", _securityTestPlugins.Select(p => (int)p.pluginId))}");

            _logger.LogDebug($"Starting security tests of {opcTargets.Count} targets");

            foreach (Target target in opcTargets)
            {
                _logger.LogDebug($"Testing {target.ApplicationName} ({target.ProductUri})");

                try
                {
                    foreach (Endpoint endpoint in GetAllTargetEndpoints(target))
                    {
                        _logger.LogTrace($"Testing endpoint {endpoint} of {target.ApplicationName}");
                        TestEndpointSecurity(endpoint);
                    }

                }
                catch (Exception e)
                {
                    string msg = $"Unknown exception scanning {target.ApplicationName}: {e}";
                    _logger.LogError(msg);

                    if (target.Servers.Any())
                    {
                        target.Servers.First().AddError(new Error(msg));
                    }
                }
            }

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

        private Endpoint TestEndpointSecurity(Endpoint endpoint)
        {
            // run first pre-auth plugins, and save opened sessions
            // run then post-auth plugins
            // ConnectionUtil takes care of closing the sessions

            List<ISession> sessions = new();

            _logger.LogTrace($"Starting pre-authentication tests");
            foreach (IPreAuthPlugin preauthPlugin in _securityTestPlugins.Where(p => p is PreAuthPlugin).Cast<IPreAuthPlugin>())
            {
                TaskUtil.CheckForCancellation(_token);
                (Issue? preauthissue, ICollection<ISession> preauthsessions) = preauthPlugin.Run(endpoint);

                sessions.AddRange(preauthsessions);
                if (preauthissue != null) endpoint.Issues.Add(preauthissue);
            }
            _logger.LogTrace($"Finished pre-authentication tests");

            if (!sessions.Any())
            {
                _logger.LogWarning($"Cannot authenticate to {endpoint}. Skipping post-authentication tests");
                return endpoint;
            }

            _logger.LogTrace($"Starting post-authentication tests");
            foreach (IPostAuthPlugin postAuthPlugin in _securityTestPlugins.Where(p => p is PreAuthPlugin).Cast<IPostAuthPlugin>())
            {
                TaskUtil.CheckForCancellation(_token);
                Issue? postauthIssue = postAuthPlugin.Run(sessions.First());
                if (postauthIssue != null) endpoint.Issues.Add(postauthIssue);
            }
            _logger.LogTrace($"Finished post-authentication tests");

            return endpoint;
        }
    }
}
