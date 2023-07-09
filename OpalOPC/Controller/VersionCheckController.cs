using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;

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
            string thisVersion = Util.VersionUtil.AppAssemblyVersion!.ToString();

            ProductInfoHeaderValue productValue = new ProductInfoHeaderValue("OpalOPC", thisVersion);
            ProductInfoHeaderValue commentValue = new ProductInfoHeaderValue($"({GetOSString()}; +https://opalopc.com)");


            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                HttpResponseMessage response;
                client.Timeout = System.TimeSpan.FromSeconds(2.5);
                client.DefaultRequestHeaders.UserAgent.Add(productValue);
                client.DefaultRequestHeaders.UserAgent.Add(commentValue);

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

                if (latestVersion != thisVersion)
                {
                    _logger.LogWarning($"Using outdated OpalOPC version {thisVersion} (the latest is {latestVersion})");
                    return;
                }

                _logger.LogTrace("Using latest version");
            }
        }

        private string GetOSString()
        {
            string os;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                os = "Windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                os = "Macintosh";
            }
            else
            {
                os = "Unknown";
            }

            return $"{os} {RuntimeInformation.ProcessArchitecture}";
        }
    }
}