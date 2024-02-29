using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model;
using ScannerApplication;
using Tests.Helpers;
using Xunit;

namespace OpalOPC.Tests.E2E
{
    public class ScannerApplicationTest
    {

        [Fact]
        [Trait("Category", "E2E")]
        public void ScanEchoTest()
        {
            // arrange
            Stream outputStream = new MemoryStream();
            using Options options = new();
            options.targets.Add(new Uri("opc.tcp://echo:53530"));
            options.OutputStream = outputStream;
            options.logLevel = LogLevel.Information;
            options.authenticationData = new AuthenticationData();
            options.commandLine = string.Empty;

            IHost _host = AppConfigurer.ConfigureApplication(options);
            IWorker worker = _host.Services.GetRequiredService<IWorker>();

            // act
            worker.Run(options);

            //convert stream to string
            outputStream.Position = 0;
            string report = new StreamReader(outputStream).ReadToEnd();

            ParsedReport parsedReport = new(report);

            // assert
            Assert.True(parsedReport.NumberOfTargets == 1);
            Assert.True(parsedReport.IssueIds.Count == parsedReport.NumberOfIssues);
            ExpectedTargetResult.Echo.validateWithParsedReport(parsedReport);

        }
    }
}
