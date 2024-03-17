using System.Net;
using System.Text.Json;
using Model;
using Moq;
using RichardSzalay.MockHttp;
using Util;
using Xunit;

namespace Tests
{
    public class KeygenApiUtilTest : IDisposable
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
        private readonly MockHttpMessageHandler _mockHttpMessageHandler = new();
        private readonly KeygenApiUtil _keygenApiUtil;
        private static readonly string s_licenseKey = "this-is-license-key";
        private static readonly string s_licenseId = "this-is-license-id";

        public KeygenApiUtilTest()
        {
            _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(_mockHttpMessageHandler));
            _keygenApiUtil = new KeygenApiUtil(_httpClientFactory.Object);
        }

        [Theory]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.VALID)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.NO_MACHINE)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.NO_MACHINES)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.OVERDUE)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.EXPIRED)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.TOO_MANY_MACHINES)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.NOT_FOUND)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.BANNED)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.SUSPENDED)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.FINGERPRINT_SCOPE_MISMATCH)]
        public async Task ValidateLicenseKeyRetursLicenseValidationResponseWhenResponseIsLegal(LicenseValidationResponse.LicenseValidationCode code)
        {
            // Arrange
            _mockHttpMessageHandler.Expect(HttpMethod.Post, "*").Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(ConstructKeygenLicenseValidationResponse(s_licenseId, code.ToString())));

            // Act
            LicenseValidationResponse response = await _keygenApiUtil.ValidateLicenseKey(s_licenseKey);

            // Assert
            Assert.Equal(code, response.code);
            Assert.Equal(s_licenseId, response.LicenseId);
        }


        [Theory]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Conflict)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task ValidateLicenseKeyThrowsErrorIfNonsucessfulStatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            _mockHttpMessageHandler.Expect(HttpMethod.Post, "*").Respond(statusCode);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(() => _keygenApiUtil.ValidateLicenseKey(s_licenseKey));
        }

        [Fact]
        public async Task ValidateLicenseKeyThrowsErrorIfNotAllRequiredPropertiesInResponse()
        {
            // Arrange
            _mockHttpMessageHandler.Expect(HttpMethod.Post, "*").Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(new
            {
                data = new { id = "code will be missing" },
                meta = new { }
            }));

            // Act
            await Assert.ThrowsAsync<JsonException>(() => _keygenApiUtil.ValidateLicenseKey(s_licenseKey));
        }

        private static KeygenApiUtil.KeygenLicenseValidationResponse ConstructKeygenLicenseValidationResponse(string id, string code)
        {
            return new()
            {
                data = new KeygenApiUtil.KeygenLicenseValidationResponse.Data
                {
                    id = id
                },
                meta = new KeygenApiUtil.KeygenLicenseValidationResponse.Meta
                {
                    code = code
                }
            };
        }

        [Fact]
        public async Task ActivateMachineReturnsMachineActivationResponseWhenResponseIsLegal()
        {
            // Arrange
            string machineId = "machine-id";
            _mockHttpMessageHandler.Expect(HttpMethod.Post, "*").Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(ConstructKeygenMachineActivationResponse(machineId)));

            // Act
            MachineActivationResponse response = await _keygenApiUtil.ActivateMachine(s_licenseKey, s_licenseId);

            // Assert
            Assert.Equal(machineId, response.MachineId);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Conflict)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task ActivateMachineThrowsErrorIfNonsucessfulStatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            _mockHttpMessageHandler.Expect(HttpMethod.Post, "*").Respond(statusCode);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(() => _keygenApiUtil.ActivateMachine(s_licenseKey, s_licenseId));
        }

        [Fact]
        public async Task ActivateMachineThrowsErrorIfNotAllRequiredPropertiesInResponse()
        {
            // Arrange
            _mockHttpMessageHandler.Expect(HttpMethod.Post, "*").Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(new
            {
                data = new { }, // id is missing
            }));

            // Act
            await Assert.ThrowsAsync<JsonException>(() => _keygenApiUtil.ActivateMachine(s_licenseKey, s_licenseId));
        }

        private static KeygenApiUtil.KeygenMachineActivationResponse ConstructKeygenMachineActivationResponse(string id)
        {
            return new()
            {
                data = new KeygenApiUtil.KeygenMachineActivationResponse.Data
                {
                    id = id
                },
            };
        }

        [Fact]
        public async Task HeartbeatReturnsWhenResponseIsLegal()
        {
            // Arrange
            _mockHttpMessageHandler.Expect(HttpMethod.Post, "*").Respond(HttpStatusCode.OK);

            // Act
            await _keygenApiUtil.Heartbeat(s_licenseKey, "machine-id");

            // Assert
        }

        [Theory]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Conflict)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task HeartbeatThrowsErrorIfNonsucessfulStatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            _mockHttpMessageHandler.Expect(HttpMethod.Post, "*").Respond(statusCode);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(() => _keygenApiUtil.Heartbeat(s_licenseKey, "machine-id"));
        }


        [Fact]
        public async Task DeactivateMachineReturnsWhenResponseIsLegal()
        {
            // Arrange
            _mockHttpMessageHandler.Expect(HttpMethod.Delete, "*").Respond(HttpStatusCode.OK);

            // Act
            await _keygenApiUtil.DeactivateMachine(s_licenseKey, "machine-id");

            // Assert
        }

        [Theory]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Conflict)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task DeactivateMachineThrowsErrorIfNonsucessfulStatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            _mockHttpMessageHandler.Expect(HttpMethod.Delete, "*").Respond(statusCode);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(() => _keygenApiUtil.DeactivateMachine(s_licenseKey, "machine-id"));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            // verify that all expected requests were made 
            _mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }
    }
}
