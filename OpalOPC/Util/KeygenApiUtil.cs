using System.Net.Http.Json;
using System.Text.Json;
using Model;

namespace Util
{
    public interface IKeygenApiUtil
    {
        public Task<LicenseValidationResponse> ValidateLicenseKey(string licenseKey);
        public Task<MachineActivationResponse> ActivateMachine(string licenseKey, string licenseId);
        public Task Heartbeat(string licenseKey, string machineId);
        public Task DeactivateMachine(string licenseKey, string machineId);

    }
    internal class KeygenApiUtil(IHttpClientFactory clientFactory) : IKeygenApiUtil
    {
        private readonly HttpClient _httpClient = clientFactory.CreateClient("KeygenApiUtil");
        private static readonly string s_keygenAccountId = "6707f5c3-164d-4b42-8ed1-072f36a85292";
        private readonly string _fingerprint = Guid.NewGuid().ToString();

        // ignore values in JSON that don't map to properties in the class
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip };

        public async Task<LicenseValidationResponse> ValidateLicenseKey(string licenseKey)
        {
            // https://keygen.sh/docs/api/licenses/#licenses-actions-validate-key
            Uri licenseValidationUri = new($"https://api.keygen.sh/v1/accounts/{s_keygenAccountId}/licenses/actions/validate-key");

            var validationMessage = new { meta = new { key = licenseKey } };
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(licenseValidationUri, validationMessage);
            response.EnsureSuccessStatusCode();
            KeygenLicenseValidationResponse? content = await response.Content.ReadFromJsonAsync<KeygenLicenseValidationResponse>(_jsonSerializerOptions);
            return new LicenseValidationResponse(content!.data.id, Enum.Parse<LicenseValidationResponse.LicenseValidationCode>(content!.meta.code));
        }

        public async Task<MachineActivationResponse> ActivateMachine(string licenseKey, string licenseId)
        {
            // https://keygen.sh/docs/api/machines/#machines-create
            Uri machineActivationUri = new($"https://api.keygen.sh/v1/accounts/{s_keygenAccountId}/machines");

            var activationMessage = new
            {
                data = new
                {
                    type = "machines",
                    attributes = new
                    {
                        fingerprint = _fingerprint,
                        hostname = Environment.MachineName,
                        platform = Environment.OSVersion.Platform.ToString(),
                        cores = Environment.ProcessorCount,
                    },
                    relationships = new
                    {
                        license = new
                        {
                            data = new
                            {
                                type = "licenses",
                                id = licenseId
                            }
                        }
                    }
                },
            };

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"License {licenseKey}");
            _httpClient.DefaultRequestHeaders.Accept.Add(new("application/vnd.api+json"));

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(machineActivationUri, activationMessage);
            response.EnsureSuccessStatusCode();
            KeygenMachineActivationResponse? content = await response.Content.ReadFromJsonAsync<KeygenMachineActivationResponse>(_jsonSerializerOptions);
            return new MachineActivationResponse(content!.data.id);
        }
        public async Task Heartbeat(string licenseKey, string machineId)
        {
            Uri hearbeatUri = new($"https://api.keygen.sh/v1/accounts/{s_keygenAccountId}/machines/{machineId}/actions/ping");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"License {licenseKey}");
            HttpResponseMessage response = await _httpClient.PostAsync(hearbeatUri, null);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeactivateMachine(string licenseKey, string machineId)
        {
            Uri machineDeactivationUri = new($"https://api.keygen.sh/v1/accounts/{s_keygenAccountId}/machines/{machineId}");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"License {licenseKey}");
            HttpResponseMessage response = await _httpClient.DeleteAsync(machineDeactivationUri);
            response.EnsureSuccessStatusCode();
        }

        public class KeygenMachineActivationResponse
        {
            public required Data data { get; set; }

            public class Data
            {
                public required string id { get; set; }
            }
        }

        public class KeygenLicenseValidationResponse
        {

            public required Data data { get; set; }
            public required Meta meta { get; set; }

            public class Meta
            {
                public required string code { get; set; }
            }

            public class Data
            {
                public required string id { get; set; }
            }
        }
    }
}
