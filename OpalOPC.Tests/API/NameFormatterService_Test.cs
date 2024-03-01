using KnownVulnerabilityAPI.Services;
using Xunit;

namespace OpalOPC.Tests.API
{
    public class NameFormatterServiceTest()
    {

        public static IEnumerable<object[]> TestData =>
            [
                ["Vendor Name", new object[] { "Vendor", "Name" }],
                ["Vendor!@#$%^&*()_+Name", new object[] { "Vendor", "Name" }],
                ["hello123", new string[] { "hello", "123" }],
                ["vendorName", new string[] { "vendor", "Name" }],
                ["VendorName", new string[] { "Vendor", "Name" }],
                ["Prosys OPC Ltd.", new string[] { "Prosys", "OPC", "Ltd" }],
                ["SimulationServer@echo", new string[] { "Simulation", "Server", "echo" }],
                ["this is too long string 123 kek", new string[] { "this", "is", "too", "long", "string" }],
            ];

        [Fact]
        public void TestNameFormatterService()
        {
            INameFormatterService nameFormatterService = new NameFormatterService();
            Assert.NotNull(nameFormatterService);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void CorrectResult(string input, string[] expectedOutput)
        {
            NameFormatterService nameFormatterService = new NameFormatterService();
            string[] result = nameFormatterService.FormatName(input);
            Assert.Equal(expectedOutput, result);
        }
    }
}
