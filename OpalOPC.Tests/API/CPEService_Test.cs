using KnownVulnerabilityAPI.Models;
using KnownVulnerabilityAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace OpalOPC.Tests.API
{
    public class CPEServiceTest
    {
        private readonly Mock<INVDAPIService> _mockNvdAPI = new();
        private readonly INameFormatterService _nameFormatter = new NameFormatterService();
        private readonly Mock<ILogger<CPEService>> _mockLogger = new();
        private readonly Mock<ICacheService> _mockCacheService = new();
        private readonly CPEService _cpeService;

        private static readonly StrictBuildInfo s_buildInfo = new()
        {
            Manufacturer = "prosysopc",
            ProductName = "simulationserver",
            SoftwareVersion = "1.0.0"
        };
        private static readonly string s_validCpeName = $"cpe:2.3:a:${s_buildInfo.Manufacturer}:${s_buildInfo.ProductName}:{s_buildInfo.SoftwareVersion}:*:*:*:*:*:*:*";

        private readonly CPEResult _validCpeResult = new()
        {
            format = "doesnotmatter",
            version = "doesnotmatter",
            totalResults = 1,
            products = [new() { cpe = new Cpe {
                cpeName = s_validCpeName,
                cpeNameId = "doesnotmatter",
            } }]
        };

        private readonly CPEResult _emptyCpeResult = new()
        {
            format = "doesnotmatter",
            version = "doesnotmatter",
            totalResults = 0,
            products = []
        };


        public CPEServiceTest()
        {
            _cpeService = new CPEService(_mockNvdAPI.Object, _nameFormatter, _mockLogger.Object, _mockCacheService.Object);
        }


        [Fact]
        public async Task ConstructCPENameReturnsCorrectCPE()
        {
            // Arrange
            _mockNvdAPI.Setup(x => x.QueryCPE(It.IsAny<string>())).ReturnsAsync(_validCpeResult);

            // Act
            string result = await _cpeService.ConstructCPEName(s_buildInfo);

            // Assert
            Assert.Equal(s_validCpeName, result);
            _mockCacheService.Verify(x => x.TryGetCPE(It.IsAny<string>()), Times.Exactly(2));
            _mockCacheService.Verify(x => x.cacheCPE(It.IsAny<string>(), It.IsAny<CPEResult>()), Times.Exactly(2));
            _mockNvdAPI.Verify(x => x.QueryCPE(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ConstructCPENameThrowsVendorNotFoundException()
        {
            // Arrange
            _mockNvdAPI.Setup(x => x.QueryCPE(It.IsAny<string>())).ReturnsAsync(_emptyCpeResult);

            // Act & Assert
            await Assert.ThrowsAsync<VendorNotFoundException>(() => _cpeService.ConstructCPEName(s_buildInfo));
        }

        [Fact]
        public async Task ConstructCPENameThrowsProductNotFoundException()
        {
            // Arrange
            _mockNvdAPI.SetupSequence(x => x.QueryCPE(It.IsAny<string>()))
                .ReturnsAsync(_validCpeResult)
                .ReturnsAsync(_emptyCpeResult);

            // Act & Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(() => _cpeService.ConstructCPEName(s_buildInfo));
        }

        [Fact]
        public async Task ConstructCPENameUsesCache()
        {
            // Arrange
            _mockCacheService.Setup(x => x.TryGetCPE(It.IsAny<string>())).Returns(_validCpeResult);

            // Act
            string result = await _cpeService.ConstructCPEName(s_buildInfo);

            // Assert
            Assert.Equal(s_validCpeName, result);
            _mockNvdAPI.Verify(x => x.QueryCPE(It.IsAny<string>()), Times.Never());
        }
    }
}
