using Microsoft.Extensions.Logging;
using Model;
using Util;

namespace Controller
{
    public interface ILicensingController : IDisposable
    {
        bool IsLicensed();
    }
    public class LicensingController(ILogger<LicensingController> logger, IKeygenApiUtil keygenApiUtil, IFileUtil fileUtil) : ILicensingController
    {
        private string? _machineId;
        private string? _licenseKey;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isSuccessfullyLicensed = false;
        public static readonly string s_licenseKeyEnv = "OPALOPC_LICENSE_KEY";
        public static readonly string s_licenseFileName = "license.txt";

        public bool IsLicensed()
        {
            _licenseKey = GetLicenseKey();
            if (_licenseKey == null)
            {
                logger.LogError("{Message}", "Missing software license key");
                return false;
            }

            try
            {
                _isSuccessfullyLicensed = ValidateLicense();
            }
            catch (Exception e)
            {
                TelemetryUtil.TrackException(e);
            }

            return _isSuccessfullyLicensed;
        }

        private string? GetLicenseKey()
        {
            // read environment variable "key"
            // if not found, read from file
            // if not found, return null

            string? envLicenseKey = Environment.GetEnvironmentVariable(s_licenseKeyEnv);
            if (envLicenseKey != null)
            {
                logger.LogTrace("{msg}", $"Using license key from environment variable {s_licenseKeyEnv}");
                return envLicenseKey;
            }

            if (fileUtil.FileExistsInAppdata(s_licenseFileName))
            {
                logger.LogTrace("{msg}", $"Using license key from file {s_licenseFileName}");
                return fileUtil.ReadFileInAppdataToList(s_licenseFileName).FirstOrDefault();
            }

            return null;
        }

        private bool ValidateLicense()
        {
            logger.LogTrace("Testing license key");
            LicenseValidationResponse validationReponse = keygenApiUtil.ValidateLicenseKey(_licenseKey!);
            if (validationReponse.IsValid)
            {
                logger.LogTrace("License is valid");
                return true;
            }
            else if (validationReponse.ShouldActivateMachine)
            {
                logger.LogTrace("Node is not activated");
                return ActivateMachine(validationReponse);
            }
            else if (validationReponse.IsMachineLimitExceeded)
            {
                string message = "License node limit exceeded";
                logger.LogCritical("{msg}", message);
                throw new Exception(message);
            }
            else if (validationReponse.IsInvalid)
            {
                string message = $"Invalid license. Reported as {validationReponse.code}";
                logger.LogCritical("{msg}", message);
                throw new Exception(message);
            }
            else
            {
                string message = "Licensing API returned an unexpected response";
                logger.LogCritical("{msg}", message);
                throw new Exception(message);
            }
        }

        private bool ActivateMachine(LicenseValidationResponse validationResponse)
        {
            logger.LogTrace("Activating node");
            MachineActivationResponse activationResponse = keygenApiUtil.ActivateMachine(validationResponse);
            _machineId = activationResponse.MachineId;
            StartHeartbeat();
            logger.LogTrace("License is valid");
            return true;
        }

        private void StartHeartbeat()
        {
            // send single heartbeat for tests
            keygenApiUtil.Heartbeat(_licenseKey!, _machineId!);

            // start a heartbeat every 10 minutes in background
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(10 * 1000);
                    logger.LogTrace("Sending licensing heartbeat");
                    keygenApiUtil.Heartbeat(_licenseKey!, _machineId!);
                }
            }, _cancellationTokenSource.Token);
        }

        private void DeactivateMachine()
        {
            logger.LogTrace("Deactivating machine");
            keygenApiUtil.DeactivateMachine(_machineId!);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            if (_isSuccessfullyLicensed && _machineId != null)
            {
                DeactivateMachine();
            }
        }
    }
}
