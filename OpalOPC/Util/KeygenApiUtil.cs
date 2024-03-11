using Model;

namespace Util
{
    public interface IKeygenApiUtil
    {
        public LicenseValidationResponse ValidateLicenseKey(string licenseKey);
        public MachineActivationResponse ActivateMachine(LicenseValidationResponse validationResponse);
        public void Heartbeat(string licenseKey, string machineId);
        public void DeactivateMachine(string machineId);

    }
    internal class KeygenApiUtil(IHttpClientFactory clientFactory) : IKeygenApiUtil
    {
        private readonly HttpClient _httpClient = clientFactory.CreateClient("KeygenApiUtil");

        private static readonly string s_keygenAccountId = "6707f5c3-164d-4b42-8ed1-072f36a85292";

        // https://keygen.sh/docs/api/licenses/#licenses-actions-validate-key
        private readonly Uri _licenseValidationUri = new($"https://api.keygen.sh/v1/accounts/{s_keygenAccountId}/licenses/actions/validate-key");

        // https://keygen.sh/docs/api/machines/#machines-create
        private readonly Uri _machineActivationUri = new($"https://api.keygen.sh/v1/accounts/{s_keygenAccountId}/machines");

        private readonly Uri _hearbeatUri = new($"https://api.keygen.sh/v1/accounts/{s_keygenAccountId}/machines/<id>/actions/ping");

        private readonly Uri _machineDeactivationUri = new($"https://api.keygen.sh/v1/accounts/{s_keygenAccountId}/machines/<id>");

        public LicenseValidationResponse ValidateLicenseKey(string licenseKey) => throw new NotImplementedException();
        public MachineActivationResponse ActivateMachine(LicenseValidationResponse validationResponse) => throw new NotImplementedException();
        public async void Heartbeat(string licenseKey, string machineId) => throw new NotImplementedException();
        public void DeactivateMachine(string machineId) => throw new NotImplementedException();
    }
}
