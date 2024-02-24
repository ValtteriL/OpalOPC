using KnownVulnerabilityAPI.Services;
using Xunit;

namespace OpalOPC.Tests.API
{
    public class NameFormatterServiceTest(NameFormatterService nameFormatterService)
    {

        [Theory]
        [InlineData("Vendor Name", new string[] { "Vendor", "Name" })]
        [InlineData("Vendor!@#$%^&*()_+Name", new string[] { "Vendor", "Name" })]
        [InlineData("hello123", new string[] { "hello", "123" })]
        [InlineData("vendorName", new string[] { "vendor", "Name" })]
        [InlineData("VendorName", new string[] { "Vendor", "Name" })]
        [InlineData("Prosys OPC Ltd.", new string[] { "Prosys", "OPC", "Ltd" })]
        [InlineData("SimulationServer@echo", new string[] { "Simulation", "Server", "echo" })]
        [InlineData("this is too long string 123 kek", new string[] { "this", "is", "too", "long", "string" })]
        public void CorrectResult(string input, string[] expectedOutput)
        {
            string[] result = nameFormatterService.FormatName(input);
            Assert.Equal(expectedOutput, result);
        }
    }
}
