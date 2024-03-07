using Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;
using ScannerApplication;
using Tests.Helpers;
using Xunit;

namespace Tests.E2E
{
    public class ScannerApplicationTest
    {

        [Fact]
        [Trait("Category", "E2E")]
        public void ScanEchoTest()
        {
            // arrange
            Stream htmlOutputStream = new MemoryStream();
            Stream sarifOutputStream = new MemoryStream();
            using Options options = new()
            {
                HtmlOutputStream = htmlOutputStream,
                SarifOutputStream = sarifOutputStream,
                authenticationData = new AuthenticationData(),
                commandLine = string.Empty,
            };
            options.targets.Add(new Uri("opc.tcp://echo:53530"));

            CLILoggerProvider loggerProvider = new(options.logLevel);

            IHost _host = AppConfigurer.ConfigureApplication(options, loggerProvider);
            IWorker worker = _host.Services.GetRequiredService<IWorker>();

            // act
            worker.Run(options);

            //convert stream to string
            htmlOutputStream.Position = 0;
            string report = new StreamReader(htmlOutputStream).ReadToEnd();

            ParsedReport parsedReport = new(report);

            // assert
            Assert.True(parsedReport.NumberOfTargets == 1);
            Assert.True(parsedReport.IssueIds.Count == parsedReport.NumberOfIssues);
            ExpectedTargetResult.Echo.validateWithParsedReport(parsedReport);

        }
    }
}
