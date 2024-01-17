using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Controller
{
    public interface IVersionCheckController
    {
        bool IsUpToDate { get; }
        void CheckVersion();
    }

    public class VersionCheckController(ILogger<VersionCheckController> logger) : IVersionCheckController
    {

        private readonly Uri _versionUri = new("https://opalopc.com/VERSION.txt");
        public bool IsUpToDate { get; private set; } = true;

        // Check what is the latest version, if not same as the current version, warn
        // if no network connection - just generate trace message
        public void CheckVersion()
        {
            string thisVersion = Util.VersionUtil.AppAssemblyVersion!.ToString();

#if DEBUG
            ProductInfoHeaderValue commentValue = new($"(DEBUG; {GetOSString()}; +https://opalopc.com)");
#else
            ProductInfoHeaderValue commentValue = new ProductInfoHeaderValue($"({GetOSString()}; +https://opalopc.com)");
#endif

            ProductInfoHeaderValue productValue = new("OpalOPC", thisVersion);


            using var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            HttpResponseMessage response;
            client.Timeout = System.TimeSpan.FromSeconds(2.5);
            client.DefaultRequestHeaders.UserAgent.Add(productValue);
            client.DefaultRequestHeaders.UserAgent.Add(commentValue);

            try
            {
                response = client.GetAsync(_versionUri).Result;
                response.EnsureSuccessStatusCode();
            }
            catch (System.Exception)
            {
                logger.LogWarning("Unable to check latest OpalOPC version");
                return;
            }

            string latestVersion = response.Content.ReadAsStringAsync().Result.TrimEnd();

            if (latestVersion != thisVersion)
            {
                IsUpToDate = false;
                logger.LogWarning("{Message}", $"Using outdated OpalOPC version {thisVersion} (the latest is {latestVersion})");
                return;
            }

            logger.LogTrace("Using latest version");
        }

        private static string GetOSString()
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
