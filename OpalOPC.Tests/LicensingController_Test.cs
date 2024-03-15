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
        private readonly LicensingController _licensingController;
        private static readonly string s_licenseKeyValue = "this-is-the-license-key";

        private static readonly string s_machineId = "this-is-the-machine-id";
        private static readonly string s_licenseId = "this-is-the-license-id";
        private static readonly LicenseValidationResponse s_nomachineLicenseValidationResponse = new(s_licenseId, LicenseValidationResponse.LicenseValidationCode.NO_MACHINE);
        private static readonly LicenseValidationResponse s_validLicenseValidationResponse = new(s_licenseId, LicenseValidationResponse.LicenseValidationCode.VALID);
        private static readonly MachineActivationResponse s_machineActivationResponse = new(s_machineId);

        public LicensingControllerTest()
        {
            _licensingController = new LicensingController(_logger.Object, _keygenApiUtil.Object, _fileUtil.Object);
        }

        [Fact]
        public void LicensingWorksWithEnvLicenseKey()
        {
            try
            {
                // Arrange
                Environment.SetEnvironmentVariable(LicensingController.s_licenseKeyEnv, s_licenseKeyValue);
                _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).Returns(s_nomachineLicenseValidationResponse);
                _keygenApiUtil.Setup(k => k.ActivateMachine(It.IsAny<LicenseValidationResponse>())).Returns(s_machineActivationResponse);
                _keygenApiUtil.Setup(k => k.Heartbeat(It.IsAny<string>(), It.IsAny<string>()));

                // Act
                bool result = _licensingController.IsLicensed();
                _licensingController.Dispose();

                // Assert (make sure machine is deactivated at end as well), make sure heartbeats are sent
                Assert.True(result);
                _keygenApiUtil.Verify(k => k.ValidateLicenseKey(s_licenseKeyValue), Times.Once);
                _keygenApiUtil.Verify(k => k.ActivateMachine(s_nomachineLicenseValidationResponse), Times.Once);
                _keygenApiUtil.Verify(k => k.Heartbeat(s_licenseKeyValue, s_machineId), Times.Once);
                _keygenApiUtil.Verify(k => k.DeactivateMachine(s_machineId), Times.Once);
                _fileUtil.Verify(f => f.FileExistsInAppdata(It.IsAny<string>()), Times.Never);

            }
            finally
            {
                Environment.SetEnvironmentVariable(LicensingController.s_licenseKeyEnv, null);
            }
        }

        [Fact]
        public void LicensingWorksWithFileLicenseKey()
        {
            // Arrange
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).Returns(s_nomachineLicenseValidationResponse);
            _keygenApiUtil.Setup(k => k.ActivateMachine(It.IsAny<LicenseValidationResponse>())).Returns(s_machineActivationResponse);
            _keygenApiUtil.Setup(k => k.Heartbeat(It.IsAny<string>(), It.IsAny<string>()));
            _fileUtil.Setup(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName)).Returns(true);
            _fileUtil.Setup(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName)).Returns([s_licenseKeyValue]);

            // Act
            bool result = _licensingController.IsLicensed();
            _licensingController.Dispose();

            // Assert (make sure machine is deactivated at end as well), make sure heartbeats are sent
            Assert.True(result);
            _fileUtil.Verify(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName), Times.Once);
            _fileUtil.Verify(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName), Times.Once);
            _keygenApiUtil.Verify(k => k.ValidateLicenseKey(s_licenseKeyValue), Times.Once);
            _keygenApiUtil.Verify(k => k.ActivateMachine(s_nomachineLicenseValidationResponse), Times.Once);
            _keygenApiUtil.Verify(k => k.Heartbeat(s_licenseKeyValue, s_machineId), Times.Once);
            _keygenApiUtil.Verify(k => k.DeactivateMachine(s_machineId), Times.Once);
        }

        [Fact]
        public void LicensingWorksIfDirectValidMessageReceived()
        {
            // Arrange
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).Returns(s_validLicenseValidationResponse);
            _fileUtil.Setup(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName)).Returns(true);
            _fileUtil.Setup(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName)).Returns([s_licenseKeyValue]);

            // Act
            bool result = _licensingController.IsLicensed();
            _licensingController.Dispose();

            // Assert (now no activation, heartbeat, or deactivation should be called)
            Assert.True(result);
            _fileUtil.Verify(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName), Times.Once);
            _fileUtil.Verify(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName), Times.Once);
            _keygenApiUtil.Verify(k => k.ValidateLicenseKey(s_licenseKeyValue), Times.Once);
            _keygenApiUtil.Verify(k => k.ActivateMachine(It.IsAny<LicenseValidationResponse>()), Times.Never);
            _keygenApiUtil.Verify(k => k.Heartbeat(It.IsAny<string>(), s_machineId), Times.Never);
            _keygenApiUtil.Verify(k => k.DeactivateMachine(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void LicensingFailsWhenLicenseFileIsEmpty()
        {
            // Arrange
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).Returns(s_validLicenseValidationResponse);
            _fileUtil.Setup(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName)).Returns(true);
            _fileUtil.Setup(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName)).Returns([]);

            // Act
            bool result = _licensingController.IsLicensed();

            // Assert (now no activation, heartbeat, or deactivation should be called)
            Assert.False(result);
            _fileUtil.Verify(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName), Times.Once);
            _fileUtil.Verify(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName), Times.Once);
            _keygenApiUtil.Verify(k => k.ValidateLicenseKey(s_licenseKeyValue), Times.Never);
        }

        [Fact]
        public void LicensingFailsWhenNoLicenseKey()
        {
            // Arrange
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).Returns(s_validLicenseValidationResponse);
            _fileUtil.Setup(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName)).Returns(true);
            _fileUtil.Setup(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName)).Returns([]);

            // Act
            bool result = _licensingController.IsLicensed();

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
        public void LicensingFailsWhenItShould(LicenseValidationResponse.LicenseValidationCode code)
        {
            // Arrange
            _keygenApiUtil.Setup(k => k.ValidateLicenseKey(It.IsAny<string>())).Returns(new LicenseValidationResponse(s_licenseId, code));
            _fileUtil.Setup(f => f.FileExistsInAppdata(LicensingController.s_licenseFileName)).Returns(true);
            _fileUtil.Setup(f => f.ReadFileInAppdataToList(LicensingController.s_licenseFileName)).Returns([s_licenseKeyValue]);

            // Act
            bool result = _licensingController.IsLicensed();

            // Assert
            Assert.False(result);
        }
    }
}
