using System.Diagnostics;
using Tests.Helpers;
using Xunit;

namespace Tests.E2E
{
    public class CliTest
    {

        private const string ApplicationPath = @"C:\Users\valtteri\source\repos\opc-ua-security-scanner\OpalOPC.WPF\bin\Debug\net8.0-windows\win-x64\opalopc.exe";

        // TODO: create a model for storing host-specific info

        private class HostIssues(int numberOfEndpoints, int numberOfErrors, int numberOfIssues, int numberOfCriticalIssues, int numberOfHighIssues, int numberOfMediumIssues, int numberOfLowIssues, int numberOfInfoIssues, List<int> issueIds)
        {
            public int NumberOfEndpoints = numberOfEndpoints;
            private List<int> IssueIds = issueIds;
            public int NumberOfErrors = numberOfErrors;
            public int NumberOfIssues = numberOfIssues;
            private int NumberOfCriticalIssues = numberOfCriticalIssues;
            private int NumberOfHighIssues = numberOfHighIssues;
            private int NumberOfMediumIssues = numberOfMediumIssues;
            private int NumberOfLowIssues = numberOfLowIssues;
            private int NumberOfInfoIssues = numberOfInfoIssues;

            public void validateWithParsedReport(ParsedReport parsedReport)
            {
                Assert.True(parsedReport.NumberOfEndpoints >= NumberOfEndpoints);
                Assert.True(parsedReport.NumberOfIssues >= NumberOfIssues);
                Assert.True(parsedReport.NumberOfCriticalIssues >= NumberOfCriticalIssues);
                Assert.True(parsedReport.NumberOfHighIssues >= NumberOfHighIssues);
                Assert.True(parsedReport.NumberOfMediumIssues >= NumberOfMediumIssues);
                Assert.True(parsedReport.NumberOfLowIssues >= NumberOfLowIssues);
                Assert.True(parsedReport.NumberOfInfoIssues >= NumberOfInfoIssues);
                Assert.True(parsedReport.NumberOfErrors >= NumberOfErrors);

                foreach (int issueId in IssueIds)
                {
                    Assert.Contains(issueId, parsedReport.IssueIds);
                }
            }
        }

        private readonly HostIssues _echo = new(
            numberOfEndpoints: 2,
            numberOfErrors: 1,
            numberOfIssues: 7,
            numberOfCriticalIssues: 0,
            numberOfHighIssues: 1,
            numberOfMediumIssues: 5,
            numberOfLowIssues: 0,
            numberOfInfoIssues: 1,
            issueIds: [10001, 10002, 10004, 10006, 10007, 10008, 10009]
            );
        private readonly HostIssues _golf = new(
            numberOfEndpoints: 1,
            numberOfErrors: 0,
            numberOfIssues: 2,
            numberOfCriticalIssues: 0,
            numberOfHighIssues: 1,
            numberOfMediumIssues: 1,
            numberOfLowIssues: 0,
            numberOfInfoIssues: 0,
            issueIds: [10001, 10007]
            );
        private readonly HostIssues _india = new(
            numberOfEndpoints: 1,
            numberOfErrors: 0,
            numberOfIssues: 1,
            numberOfCriticalIssues: 0,
            numberOfHighIssues: 1,
            numberOfMediumIssues: 0,
            numberOfLowIssues: 0,
            numberOfInfoIssues: 0,
            issueIds: [10001]
            );
        private readonly HostIssues _scanme = new(
            numberOfEndpoints: 1,
            numberOfErrors: 0,
            numberOfIssues: 7,
            numberOfCriticalIssues: 0,
            numberOfHighIssues: 1,
            numberOfMediumIssues: 5,
            numberOfLowIssues: 0,
            numberOfInfoIssues: 1,
            issueIds: [10001, 10002, 10004, 10006, 10007, 10008, 10009]
            );
        private readonly HostIssues _invalid = new(
            numberOfEndpoints: 0,
            numberOfErrors: 0,
            numberOfIssues: 0,
            numberOfCriticalIssues: 0,
            numberOfHighIssues: 0,
            numberOfMediumIssues: 0,
            numberOfLowIssues: 0,
            numberOfInfoIssues: 0,
            issueIds: []
            );

        private static Process RunCommand(string command)
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    Arguments = "/c " + command,
                    CreateNoWindow = true,
                    WorkingDirectory = string.Empty,
                }
            };
            process.Start();
            process.WaitForExit();
            return process;
        }

        [Fact]
        public void ScanEchoTest()
        {
            // scan echo server, validate report

            // arrange
            string reportname = "opalopc-report-echo.html";

            // act
            Process process = RunCommand($"{ApplicationPath} opc.tcp://echo:53530 -vv --output {reportname}");
            ParsedReport parsedReport = new(File.ReadAllText(reportname));

            // assert
            Assert.True(process.ExitCode == 0);
            Assert.True(parsedReport.NumberOfTargets == 1);
            Assert.True(parsedReport.IssueIds.Count == parsedReport.NumberOfIssues);
            _echo.validateWithParsedReport(parsedReport);

            // cleanup
            File.Delete(reportname);
        }

        [Fact]
        public void ScanInvalidTargetTest()
        {
            // scan opc.tcp://thisdoesnotexistsfafasfada:53530

            // arrange
            string reportname = "opalopc-report-invalidtarget.html";

            // act
            Process process = RunCommand($"{ApplicationPath} opc.tcp://thisdoesnotexistsfafasfada:53530 -vv --output {reportname}");
            ParsedReport parsedReport = new(File.ReadAllText(reportname));

            // assert
            Assert.True(process.ExitCode == 0);
            Assert.True(parsedReport.IssueIds.Count == parsedReport.NumberOfIssues);
            _invalid.validateWithParsedReport(parsedReport);

            // cleanup
            File.Delete(reportname);
        }

        [Fact]
        public void ScanMultipleTargetsTest()
        {
            // scan echo, golf, india, scanme.opalopc.com

            // arrange
            string reportname = "opalopc-report-multipletargets.html";

            // act
            Process process = RunCommand($"{ApplicationPath} opc.tcp://echo:53530 opc.tcp://golf:53530 opc.tcp://india:53530 opc.tcp://scanme.opalopc.com:53530 -vv --output {reportname}");
            ParsedReport parsedReport = new(File.ReadAllText(reportname));

            // assert
            Assert.True(process.ExitCode == 0);
            Assert.True(parsedReport.NumberOfTargets == 4);
            Assert.True(parsedReport.NumberOfEndpoints == _echo.NumberOfEndpoints + _golf.NumberOfEndpoints + _india.NumberOfEndpoints + _scanme.NumberOfEndpoints);
            Assert.True(parsedReport.IssueIds.Count == parsedReport.NumberOfIssues);
            _echo.validateWithParsedReport(parsedReport);
            _golf.validateWithParsedReport(parsedReport);
            _india.validateWithParsedReport(parsedReport);
            _scanme.validateWithParsedReport(parsedReport);

            // cleanup
            File.Delete(reportname);
        }

    }
}
