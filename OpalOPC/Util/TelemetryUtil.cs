using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Util
{
    public static class TelemetryUtil
    {
        private const string ConnectionString = "InstrumentationKey=a5ef3298-fd37-412e-bed6-9efb26963223;IngestionEndpoint=https://swedencentral-0.in.applicationinsights.azure.com/";

        private static readonly TelemetryClient s_telemetry = GetAppInsightsClient();

        public static bool Enabled { get; set; } = true;

        private static TelemetryClient GetAppInsightsClient()
        {
            TelemetryConfiguration config = new()
            {
                ConnectionString = ConnectionString,
                TelemetryChannel = new Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel.ServerTelemetryChannel(),
            };

            config.TelemetryChannel.DeveloperMode = Debugger.IsAttached;
#if DEBUG
            config.TelemetryChannel.DeveloperMode = true;
#endif
            TelemetryClient client = new(config);
            client.Context.Component.Version = VersionUtil.AppAssemblyVersion!.ToString();
            client.Context.Session.Id = Guid.NewGuid().ToString();
            client.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString(); // hashed for privacy
            client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            return client;
        }

        public static void TrackEvent(string key, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
        {
            if (Enabled)
            {
                s_telemetry.TrackEvent(key, properties, metrics);
            }
        }

        public static void TrackException(Exception ex)
        {
            if (Enabled)
            {
                var telex = new ExceptionTelemetry(ex);
                s_telemetry.TrackException(telex);
                Flush();
            }
        }

        internal static void Flush()
        {
            s_telemetry.Flush();
        }
    }
}
