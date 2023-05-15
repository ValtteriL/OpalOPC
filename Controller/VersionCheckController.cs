using System.Net;
using Microsoft.Extensions.Logging;
using Model;
using View;

namespace Controller
{
    public class VersionCheckController
    {

        ILogger _logger;
        private Uri versionUri = new Uri("https://opalopc.com/VERSION.txt");

        public VersionCheckController(ILogger logger)
        {
            _logger = logger;
        }

        // Check what is the latest version, if not same as the current version, warn
        // if no network connection - just generate trace message
        public void CheckVersion()
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                HttpResponseMessage response;
                client.Timeout = System.TimeSpan.FromSeconds(2.5);

                try
                {
                    response = client.GetAsync(versionUri).Result;
                    response.EnsureSuccessStatusCode();
                }
                catch (System.Exception)
                {
                    _logger.LogWarning("Unable to check latest OpalOPC version");
                    return;
                }

                string latestVersion = response.Content.ReadAsStringAsync().Result.TrimEnd();

                if (latestVersion != Util.VersionUtil.AppAssemblyVersion!.ToString())
                {
                    _logger.LogWarning($"Using outdated OpalOPC version {Util.VersionUtil.AppAssemblyVersion} (the latest is {latestVersion})");
                    return;
                }

                _logger.LogTrace("Using latest version");
            }
        }
    }
}