using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.E2E
{
    public class CliTest
    {

        private const string ApplicationPath = @"C:\Users\valtteri\source\repos\opc-ua-security-scanner\OpalOPC.WPF\bin\Debug\net8.0-windows\win-x64\opalopc.exe";

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

            // assert
            Assert.True(process.ExitCode == 0);
            Assert.True(File.Exists(reportname));
            File.Delete(reportname);
            // TODO: parse report & validate
        }

        [Fact]
        public void ScanInvalidTargetTest()
        {
            // scan opc.tcp://thisdoesnotexistsfafasfada:53530

            // arrange
            string reportname = "opalopc-report-invalidtarget.html";

            // act
            Process process = RunCommand($"{ApplicationPath} opc.tcp://thisdoesnotexistsfafasfada:53530 -vv --output {reportname}");

            // assert
            Assert.True(process.ExitCode == 0);
            Assert.True(File.Exists(reportname));
            File.Delete(reportname);
            // TODO: parse report & validate
        }

        [Fact]
        public void ScanMultipleTargetsTest()
        {
            // scan echo, golf, india, scanme.opalopc.com

            // arrange
            string reportname = "opalopc-report-multipletargets.html";

            // act
            Process process = RunCommand($"{ApplicationPath} opc.tcp://echo:53530 opc.tcp://golf:53530 opc.tcp://india:53530 opc.tcp://scanme.opalopc.com:53530 -vv --output {reportname}");

            // assert
            Assert.True(process.ExitCode == 0);
            Assert.True(File.Exists(reportname));
            File.Delete(reportname);
            // TODO: parse report & validate
        }

    }
}
