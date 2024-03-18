using Controller;
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Util;
using Xunit;

namespace Tests
{
    public class LicensingControllerTest
    {
        private readonly Mock<ILogger<LicensingController>> _logger = new();
        private readonly Mock<IKeygenApiUtil> _keygenApiUtil = new();
        private readonly Mock<IFileUtil> _fileUtil = new();
        private readonly Mock<IEnvironmentService> _environmentService = new();
        private readonly LicensingController _licensingController;
        private static readonly string s_licenseKeyValue = "this-is-the-license-key";

        private static readonly string s_machineId = "this-is-the-machine-id";
        private static readonly string s_licenseId = "this-is-the-license-id";
        private static readonly LicenseValidationResponse s_nomachineLicenseValidationResponse = new(s_licenseId, LicenseValidationResponse.LicenseValidationCode.NO_MACHINE);
        private static readonly LicenseValidationResponse s_validLicenseValidationResponse = new(s_licenseId, LicenseValidationResponse.LicenseValidationCode.VALID);
        private static readonly MachineActivationResponse s_machineActivationResponse = new(s_machineId);

        public LicensingControllerTest()
        {
            _licensingController = new LicensingController(_logger.Object, _keygenApiUtil.Object, _fileUtil.Object, _environmentService.Object);
        }

        [Fact]
        public async Task LicensingWorksWithEnvLicenseKeyAsync()
        {

            // Arrange
            _environmentService.Setup(e => e.GetEnvironmentVariable(LicensingController.s_licenseKeyEnv)).Returns(s_licenseKeyValue);
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).ReturnsAsync(s_nomachineLicenseValidationResponse);
            _keygenApiUtil.Setup(k => k.ActivateMachine(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(s_machineActivationResponse);
            _keygenApiUtil.Setup(k => k.Heartbeat(It.IsAny<string>(), It.IsAny<string>()));

            // Act
            bool result = await _licensingController.IsLicensed();
            _licensingController.Dispose();

            // Assert (make sure machine is deactivated at end as well), make sure heartbeats are sent
            Assert.True(result);
            _keygenApiUtil.Verify(k => k.ValidateLicenseKey(s_licenseKeyValue), Times.Once);
            _keygenApiUtil.Verify(k => k.ActivateMachine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _keygenApiUtil.Verify(k => k.Heartbeat(s_licenseKeyValue, s_machineId), Times.Once);
            _keygenApiUtil.Verify(k => k.DeactivateMachine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileUtil.Verify(f => f.FileExistsInAppdata(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LicensingWorksWithFileLicenseKeyAsync()
        {
            // Arrange
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).ReturnsAsync(s_nomachineLicenseValidationResponse);
            _keygenApiUtil.Setup(k => k.ActivateMachine(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(s_machineActivationResponse);
            _keygenApiUtil.Setup(k => k.Heartbeat(It.IsAny<string>(), It.IsAny<string>()));
            _fileUtil.Setup(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName)).Returns(true);
            _fileUtil.Setup(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName)).Returns([s_licenseKeyValue]);

            // Act
            bool result = await _licensingController.IsLicensed();
            _licensingController.Dispose();

            // Assert (make sure machine is deactivated at end as well), make sure heartbeats are sent
            Assert.True(result);
            _fileUtil.Verify(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName), Times.Once);
            _fileUtil.Verify(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName), Times.Once);
            _keygenApiUtil.Verify(k => k.ValidateLicenseKey(s_licenseKeyValue), Times.Once);
            _keygenApiUtil.Verify(k => k.ActivateMachine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _keygenApiUtil.Verify(k => k.Heartbeat(s_licenseKeyValue, s_machineId), Times.Once);
            _keygenApiUtil.Verify(k => k.DeactivateMachine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task LicensingWorksIfDirectValidMessageReceivedAsync()
        {
            // Arrange
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).ReturnsAsync(s_validLicenseValidationResponse);
            _fileUtil.Setup(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName)).Returns(true);
            _fileUtil.Setup(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName)).Returns([s_licenseKeyValue]);

            // Act
            bool result = await _licensingController.IsLicensed();
            _licensingController.Dispose();

            // Assert (now no activation, heartbeat, or deactivation should be called)
            Assert.True(result);
            _fileUtil.Verify(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName), Times.Once);
            _fileUtil.Verify(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName), Times.Once);
            _keygenApiUtil.Verify(k => k.ValidateLicenseKey(s_licenseKeyValue), Times.Once);
            _keygenApiUtil.Verify(k => k.ActivateMachine(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _keygenApiUtil.Verify(k => k.Heartbeat(It.IsAny<string>(), s_machineId), Times.Never);
            _keygenApiUtil.Verify(k => k.DeactivateMachine(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LicensingFailsWhenLicenseFileIsEmptyAsync()
        {
            // Arrange
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).ReturnsAsync(s_validLicenseValidationResponse);
            _fileUtil.Setup(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName)).Returns(true);
            _fileUtil.Setup(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName)).Returns([]);

            // Act
            bool result = await _licensingController.IsLicensed();

            // Assert (now no activation, heartbeat, or deactivation should be called)
            Assert.False(result);
            _fileUtil.Verify(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName), Times.Once);
            _fileUtil.Verify(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName), Times.Once);
            _keygenApiUtil.Verify(k => k.ValidateLicenseKey(s_licenseKeyValue), Times.Never);
        }

        [Fact]
        public async Task LicensingFailsWhenNoLicenseKeyAsync()
        {
            // Arrange
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).ReturnsAsync(s_validLicenseValidationResponse);
            _fileUtil.Setup(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName)).Returns(true);
            _fileUtil.Setup(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName)).Returns([]);

            // Act
            bool result = await _licensingController.IsLicensed();

            // Assert (now no activation, heartbeat, or deactivation should be called)
            Assert.False(result);
            _fileUtil.Verify(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName), Times.Once);
            _fileUtil.Verify(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName), Times.Once);
            _keygenApiUtil.Verify(k => k.ValidateLicenseKey(s_licenseKeyValue), Times.Never);
        }

        [Theory]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.OVERDUE)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.EXPIRED)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.TOO_MANY_MACHINES)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.NOT_FOUND)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.BANNED)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.SUSPENDED)]
        [InlineData(LicenseValidationResponse.LicenseValidationCode.FINGERPRINT_SCOPE_MISMATCH)]
        public async void LicensingFailsWhenItShould(LicenseValidationResponse.LicenseValidationCode code)
        {
            // Arrange
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).ReturnsAsync(new LicenseValidationResponse(s_licenseId, code));
            _fileUtil.Setup(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName)).Returns(true);
            _fileUtil.Setup(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName)).Returns([s_licenseKeyValue]);

            // Act
            bool result = await _licensingController.IsLicensed();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task StoreLicenseWorksAsync()
        {
            // Arrange
            _fileUtil.Setup(f => f.WriteStringToFileInAppdata(LicensingController.s_licenseFileName, s_licenseKeyValue));

            // Act
            await _licensingController.StoreLicense(s_licenseKeyValue);

            // Assert
            _fileUtil.Verify(f => f.WriteStringToFileInAppdata(LicensingController.s_licenseFileName, s_licenseKeyValue), Times.Once);
        }
    }
}
