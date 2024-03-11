using Microsoft.Extensions.Logging;
using Model;
using Util;

namespace Controller
{
    public interface ILicensingController : IDisposable
    {
        bool IsLicensed();
    }
    internal class LicensingController(ILogger<LicensingController> logger, IKeygenApiUtil keygenApiUtil) : ILicensingController
    {
        private string? _machineId;
        private string? _licenseKey;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isSuccessfullyLicensed = false;

        public bool IsLicensed()
        {
            // get license key
            // if no key, return false
            if (!GetLicenseKey())
            {
                return false;
            }

            _isSuccessfullyLicensed = ValidateLicense();

            return _isSuccessfullyLicensed;
        }

        private bool GetLicenseKey()
        {
            // get license key from file or environment variable
            throw new NotImplementedException();
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
                string message = "License node limit exceeded.";
                logger.LogCritical("{msg}", message);
                throw new Exception(message);
            }
            else if (validationReponse.IsInvalid)
            {
                string message = $"Invalid license. Reported as {validationReponse.Code}.";
                logger.LogCritical("{msg}", message);
                throw new Exception(message);
            }
            else
            {
                string message = "Licensing API returned an unexpected response.";
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
            // start a heartbeat every 10 minutes in background
            Task.Run(() =>
            {
                while (true)
                {
                    logger.LogTrace("Sending licensing heartbeat");
                    keygenApiUtil.Heartbeat(_licenseKey!, _machineId!);
                    Thread.Sleep(10 * 1000);
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
            if (_isSuccessfullyLicensed)
            {
                DeactivateMachine();
            }
        }
    }
}
