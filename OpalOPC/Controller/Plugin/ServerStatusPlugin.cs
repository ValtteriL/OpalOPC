using System.Text.Json;
using Microsoft.Extensions.Logging;
using Model;
using Opc.Ua;
using Opc.Ua.Client;

namespace Plugin
{
    public class ServerStatusPlugin(ILogger logger) : PostAuthPlugin(logger, s_pluginId, s_category, s_issueTitle, s_severity)
    {
        // check if auditing disabled
        private static readonly PluginId s_pluginId = PluginId.ServerStatus;
        private static readonly string s_category = PluginCategories.Reconnaissance;
        private static readonly string s_issueTitle = "ServerStatus";
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        // Info
        private static readonly double s_severity = 0;

        public override Issue? Run(IList<ISession> sessions)
        {
            ISession firstSession = sessions.First();
            _logger.LogTrace("{Message}", $"Checking ServerStatus on {firstSession.Endpoint.EndpointUrl}");

            // try checking ServerStatus with each session until success
            foreach (ISession session in sessions)
            {
                try
                {
                    // check ServerStatus
                    ServerStatusDataType? serverStatusDataType = (ServerStatusDataType)session.ReadValue(Util.WellKnownNodes.Server_ServerStatus, typeof(ServerStatusDataType));

                    // if serverStatusDataType is null, return null
                    if (serverStatusDataType == null)
                    {
                        _logger.LogTrace("{Message}", "ServerStatus is null");
                        return null;
                    }

                    return CreateIssue(serverStatusDataType);
                }
                catch (ServiceResultException)
                {
                    // ignore errors like BadUserAccessDenied
                }
            }

            return null;
        }

        private Issue CreateIssue(ServerStatusDataType serverStatusDataType)
        {
            // create empty json object
            string issueDescription = $"ServerStatus: {JsonSerializer.Serialize(
                new ServerStatusJson(serverStatusDataType),
                _jsonSerializerOptions)}";
            return new Issue(s_pluginId, issueDescription, _severity);
        }

        private class ServerStatusJson(ServerStatusDataType serverStatusDataType)
        {
            public string SoftwareVersion { get; set; } = serverStatusDataType.BuildInfo.SoftwareVersion;
            public string BuildNumber { get; set; } = serverStatusDataType.BuildInfo.BuildNumber;
            public DateTime BuildDate { get; set; } = serverStatusDataType.BuildInfo.BuildDate;
            public string ManufacturerName { get; set; } = serverStatusDataType.BuildInfo.ManufacturerName;
            public string ProductName { get; set; } = serverStatusDataType.BuildInfo.ProductName;
            public string ProductUri { get; set; } = serverStatusDataType.BuildInfo.ProductUri;
            public DateTime CurrentTime { get; set; } = serverStatusDataType.CurrentTime;
            public DateTime StartTime { get; set; } = serverStatusDataType.StartTime;
            public string State { get; set; } = serverStatusDataType.State.ToString();
            public uint SecondsTillShutdown { get; set; } = serverStatusDataType.SecondsTillShutdown;
            public string ShutdownReason { get; set; } = serverStatusDataType.ShutdownReason?.Text ?? "";
        }
    }
}
