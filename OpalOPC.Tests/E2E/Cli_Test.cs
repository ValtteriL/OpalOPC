using System.Diagnostics;
using Microsoft.CodeAnalysis.Sarif;
using Tests.Helpers;
using Xunit;

namespace Tests.E2E
{
    public class CliTest
    {

        private const string ApplicationPath = @"C:\Users\valtteri\source\repos\opc-ua-security-scanner\OpalOPC.WPF\bin\Debug\net8.0-windows\win-x64\opalopc.exe";

        private readonly string _reportBaseName = "opalopc-report";
        private string _htmlReport => _reportBaseName + ".html";
        private string _sarifReport => _reportBaseName + ".sarif";

        // run with license key env
        // run with invalid license key env
        // run with no license key env

        private static ProcessStartInfo BuildProcessStartInfo(string command)
        {
            return new ProcessStartInfo
            {
                FileName = "cmd.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                Arguments = "/c " + command,
                CreateNoWindow = true,
                WorkingDirectory = string.Empty,
            };
        }

        private static Process RunCommand(string command)
        {
            ProcessStartInfo processStartInfo = BuildProcessStartInfo(command);
            return RunProcess(processStartInfo);
        }

        private static Process RunProcess(ProcessStartInfo processStartInfo)
        {
            Process process = new()
            {
                StartInfo = processStartInfo
            };
            process.Start();
            process.WaitForExit();
            return process;
        }


        [Fact]
        [Trait("Category", "E2E")]
        public void ScanEchoTest()
        {
            // scan echo server, validate report

            // act
            Process process = RunCommand($"{ApplicationPath} opc.tcp://echo:53530 -vv --output {_reportBaseName}");
            ParsedReport parsedReport = new(File.ReadAllText(_htmlReport));
            SarifLog sarifLog = SarifLog.Load(_sarifReport);

            // assert
            Assert.True(process.ExitCode == 0);
            Assert.True(parsedReport.NumberOfTargets == 1);
            Assert.True(parsedReport.IssueIds.Count == parsedReport.NumberOfIssues);

            ExpectedTargetResult.Echo.validateWithParsedReport(parsedReport);
            ExpectedTargetResult.Echo.validateWithSarifReport(sarifLog);

            // cleanup
            File.Delete(_htmlReport);
            File.Delete(_sarifReport);
        }

        [Fact]
        [Trait("Category", "E2E")]
        public void ScanInvalidTargetTest()
        {
            // scan opc.tcp://thisdoesnotexistsfafasfada:53530

            // act
            Process process = RunCommand($"{ApplicationPath} opc.tcp://thisdoesnotexistsfafasfada:53530 -vv --output {_reportBaseName}");
            ParsedReport parsedReport = new(File.ReadAllText(_htmlReport));
            SarifLog sarifLog = SarifLog.Load(_sarifReport);

            // assert
            Assert.True(process.ExitCode == 0);
            Assert.True(parsedReport.IssueIds.Count == parsedReport.NumberOfIssues);
            ExpectedTargetResult.Invalid.validateWithParsedReport(parsedReport);
            ExpectedTargetResult.Invalid.validateWithSarifReport(sarifLog);

            // cleanup
            File.Delete(_htmlReport);
            File.Delete(_sarifReport);
        }

        [Fact]
        [Trait("Category", "E2E")]
        public void ScanMultipleTargetsTest()
        {
            // scan echo, scanme.opalopc.com

            // act
            Process process = RunCommand($"{ApplicationPath} opc.tcp://echo:53530 opc.tcp://scanme.opalopc.com:53530 -vv --output {_reportBaseName}");
            ParsedReport parsedReport = new(File.ReadAllText(_htmlReport));
            SarifLog sarifLog = SarifLog.Load(_sarifReport);

            // assert
            Assert.True(process.ExitCode == 0);
            Assert.True(parsedReport.NumberOfTargets == 2);
            Assert.True(parsedReport.NumberOfEndpoints == ExpectedTargetResult.Echo.NumberOfEndpoints + ExpectedTargetResult.Scanme.NumberOfEndpoints);
            Assert.True(parsedReport.IssueIds.Count == parsedReport.NumberOfIssues);

            ExpectedTargetResult.Echo.validateWithParsedReport(parsedReport);
            ExpectedTargetResult.Echo.validateWithSarifReport(sarifLog);
            ExpectedTargetResult.Scanme.validateWithParsedReport(parsedReport);
            ExpectedTargetResult.Scanme.validateWithSarifReport(sarifLog);

            // cleanup
            File.Delete(_htmlReport);
            File.Delete(_sarifReport);
        }

        [Fact]
        [Trait("Category", "E2E")]
        public void NetworkDiscovery()
        {

            // act
            Process process = RunCommand($"{ApplicationPath} -d"); // discovery shall work without license key

            // assert
            Assert.True(process.ExitCode == 0);
        }

    }
}
