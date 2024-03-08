using KnownVulnerabilityAPI.Models;
using KnownVulnerabilityAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.API
{
    public class CVEServiceTest
    {
        private readonly Mock<INVDAPIService> _mockNvdAPI = new();
        private readonly Mock<ILogger<CVEService>> _mockLogger = new();
        private readonly Mock<ICacheService> _mockCacheService = new();
        private readonly CVEService _cveService;

        private readonly CVEResult _cVEResult = new()
        {
            format = "doesnotmatter",
            version = "doesnotmatter",
            totalResults = 1,
            vulnerabilities = []
        };

        public CVEServiceTest()
        {
            _cveService = new CVEService(_mockNvdAPI.Object, _mockCacheService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCVEsReturnsCorrectCVEs()
        {
            // Arrange
            _mockNvdAPI.Setup(x => x.QueryCVE(It.IsAny<string>())).ReturnsAsync(_cVEResult);

            // Act
            CVEResult result = await _cveService.GetCVEs(string.Empty);

            // Assert
            Assert.Equal(_cVEResult, result);
            _mockNvdAPI.Verify(x => x.QueryCVE(It.IsAny<string>()), Times.Once);
            _mockCacheService.Verify(x => x.TryGetCVE(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetCVEsThrowsIfNVDApiThrows()
        {
            // Arrange
            _mockNvdAPI.Setup(x => x.QueryCVE(It.IsAny<string>())).Throws<Exception>();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _cveService.GetCVEs(string.Empty));
        }

        [Fact]
        public async void GetCVEsReturnsCachedResult()
        {
            // Arrange
            _mockCacheService.Setup(x => x.TryGetCVE(It.IsAny<string>())).Returns(_cVEResult);

            // Act
            CVEResult result = await _cveService.GetCVEs(string.Empty);

            // Assert
            Assert.Equal(_cVEResult, result);
            _mockNvdAPI.Verify(x => x.QueryCVE(It.IsAny<string>()), Times.Never);
            _mockCacheService.Verify(x => x.TryGetCVE(It.IsAny<string>()), Times.Once);
        }
    }
}
