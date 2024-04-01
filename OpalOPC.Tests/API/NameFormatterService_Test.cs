using KnownVulnerabilityAPI.Services;
using Xunit;

namespace Tests.API
{
    public class NameFormatterServiceTest()
    {

        public static IEnumerable<object[]> TestData =>
            [
                ["Vendor Name", new object[] { "Vendor", "Name" }],
                ["Vendor!@#$%^&*()_+Name", new object[] { "Vendor", "Name" }],
                ["hello123", new string[] { "hello123" }],
                ["vendorName", new string[] { "vendorName" }],
                ["VendorName", new string[] { "VendorName" }],
                ["Prosys OPC Ltd.", new string[] { "Prosys", "OPC", "\"Ltd.\"" }],
                ["SimulationServer@echo", new string[] { "SimulationServer", "echo" }],
                ["this is too long string 123 kek", new string[] { "this", "is", "too", "long", "string" }],
            ];

        [Theory]
        [MemberData(nameof(TestData))]
        public void CorrectResult(string input, string[] expectedOutput)
        {
            string[] result = NameFormatter.FormatName(input);
            Assert.Equal(expectedOutput, result);
        }
    }
}
