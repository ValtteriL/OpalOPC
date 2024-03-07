using Microsoft.Extensions.Logging;
using Model;
using Mono.Options;
using Opc.Ua;
using Util;

namespace View
{
    public class Argparser
    {
        private readonly string[] _args;
        private readonly Mono.Options.OptionSet _optionSet;
        private readonly string _programName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        private readonly IFileUtil _fileUtil;

        private bool _shouldDiscoverAndExit = false;
        private bool _shouldShowVersion = false;
        private bool _shouldShowHelp = false;
        private LogLevel _logLevel = LogLevel.Information;
        private string? _htmlOutputReportName;
        private string? _sarifOutputReportName;
        private Stream? _htmlOutputStream;
        private Stream? _sarifOutputStream;
        private bool _shouldExit = false;
        private int _exitCode = (int)ExitCodes.Success;
        private readonly AuthenticationData _authenticationData = new();
        private readonly List<Uri> _targets = [];

        public Argparser(string[] args)
        {
            _optionSet = new Mono.Options.OptionSet {
                { "i|input-file=", "input targets from list of discovery uris", il => readTargetFile(il) },
                { "o|output=", "base name for output reports", ox => setOutputFile(ox) },
                { "v", "increase verbosity (can be specified up to 2 times)", v => _logLevel = (v == null) ? _logLevel : _logLevel == LogLevel.Information ? LogLevel.Debug : LogLevel.Trace },
                { "h|help", "show this message and exit", h => _shouldShowHelp = h != null },
                { "l|login-credential=", "username:password for user authentication", l => addLoginCredential(l) },
                { "b|brute-force-credential=", "username:password for brute force attack", b => addBruteForceCredential(b) },
                { "L|login-credential-file=", "import list of username:password for authentication from file", l => addLoginCredentialsFromFile(l) },
                { "B|brute-force-credential-file=", "import list of username:password for brute force attack from file", b => addBruteForceCredentialsFromFile(b) },
                { "c|user-certificate-and-privatekey=", "path-to-certificate:path-to-privatekey for user authentication", c => addUserCertificatePrivatekey(c) },
                { "a|application-certificate-and-privatekey=", "path-to-certificate:path-to-privatekey for application authentication", a => addAppCertificatePrivatekey(a) },
                { "d|discovery", "discover targets on network through mDNS and exit", d => _shouldDiscoverAndExit = d != null },
                { "version", "show version and exit", ver => _shouldShowVersion = ver != null },
            };

            _args = args;
            _fileUtil = new FileUtil();
        }

        // constructor for tests 
        public Argparser(string[] args, IFileUtil fileUtil) : this(args)
        {
            _fileUtil = fileUtil;
        }

        private static (string, string) splitStringToTwoByColon(string usernamePassword)
        {
            string[] splitLine = usernamePassword.Split(':', 2);
            if (splitLine.Length == 2)
            {
                return (splitLine[0], splitLine[1]);
            }
            else
            {
                throw new OptionException($"\"{usernamePassword}\" is invalid user authentication credential", "");
            }
        }

        private List<string> readFileToList(string path)
        {
            List<string> lines;

            try
            {
                lines = [.. _fileUtil.ReadFileToList(path)];

            }
            catch (Exception e)
            {
                throw new OptionException(e.Message, "");
            }

            return lines;
        }

        private CertificateIdentifier readCertificate(string certPath, string privkeyPath)
        {
            CertificateIdentifier cert;
            try
            {
                cert = _fileUtil.CreateCertificateIdentifierFromPemFile(certPath, privkeyPath);
            }
            catch (Exception e)
            {
                throw new OptionException($"Unable to process certificate {certPath} with private key {privkeyPath}: {e.Message}", "");
            }

            return cert;
        }

        private void addAppCertificatePrivatekey(string paths)
        {
            (string certpath, string privkeypath) = splitStringToTwoByColon(paths);
            _authenticationData.AddApplicationCertificate(readCertificate(certpath, privkeypath));
        }

        private void addUserCertificatePrivatekey(string paths)
        {
            (string certpath, string privkeypath) = splitStringToTwoByColon(paths);
            _authenticationData.AddUserCertificate(readCertificate(certpath, privkeypath));
        }

        private void addBruteForceCredentialsFromFile(string path)
        {
            foreach (string line in readFileToList(path))
            {
                addBruteForceCredential(line);
            }
        }

        private void addLoginCredentialsFromFile(string path)
        {
            foreach (string line in readFileToList(path))
            {
                addLoginCredential(line);
            }
        }

        private void addLoginCredential(string credential)
        {
            (string username, string password) = splitStringToTwoByColon(credential);
            _authenticationData.AddLoginCredential(username, password);
        }

        private void addBruteForceCredential(string credential)
        {
            (string username, string password) = splitStringToTwoByColon(credential);
            _authenticationData.AddBruteForceCredential(username, password);
        }

        private void setOutputFile(string path)
        {
            // assume path does not contain extension - add .html and .sarif

            _htmlOutputReportName = $"{path}.html";
            _sarifOutputReportName = $"{path}.sarif";

            _htmlOutputStream = openFile(_htmlOutputReportName);
            _sarifOutputStream = openFile(_sarifOutputReportName);
        }

        private static FileStream openFile(string path)
        {
            try
            {
                return File.Create(path);
            }
            catch (UnauthorizedAccessException)
            {
                throw new OptionException($"Not authorized to open \"{path}\" for writing", "");
            }
            catch
            {
                throw new OptionException($"Unable to open \"{path}\" for writing", "");
            }
        }

        private void readTargetFile(string path)
        {
            List<string> lines = [];

            try
            {
                if (path == "-")
                {
                    // read discoveryuris from stdin
                    string? line;
                    while ((line = Console.ReadLine()) != null && line != "")
                    {
                        lines.Add(line);
                    }
                }
                else
                {
                    lines = [.. _fileUtil.ReadFileToList(path)];
                }
            }
            catch (Exception e)
            {
                throw new OptionException(e.Message, "");
            }

            foreach (string line in lines)
            {
                appendTarget(line);
            }
        }

        private void appendTarget(string target)
        {
            Uri uri;

            try
            {
                uri = new Uri(target);
            }
            catch (Exception)
            {
                throw new OptionException($"\"{target}\" is invalid target", "");
            }

            _targets.Add(uri);
        }

        private void printHelp()
        {
            Console.WriteLine($"Opal OPC {VersionUtil.AppAssemblyVersion} ( https://opalopc.com )");
            Console.WriteLine($"Usage: {_programName} [Options] [Target ...]");
            _optionSet.WriteOptionDescriptions(Console.Out);
        }
        private static void printVersion()
        {
            Console.WriteLine(VersionUtil.AppAssemblyVersion);
        }

        private void deleteReportIfCreatedAlready()
        {
            if (_htmlOutputStream != null)
            {
                _htmlOutputStream!.Dispose();
                File.Delete(_htmlOutputReportName!);
            }

            if (_sarifOutputStream != null)
            {
                _sarifOutputStream.Dispose();
                File.Delete(_sarifOutputReportName!);
            }
        }

        public Options parseArgs()
        {
            try
            {
                // parse the command line
                List<string> extra = _optionSet.Parse(_args);
                extra.ForEach(e => appendTarget(e));
                if (_htmlOutputReportName == null || _sarifOutputReportName == null)
                {
                    setOutputFile(ArgUtil.DefaultReportName());
                }
            }
            catch (OptionException e)
            {
                // output some error message
                Console.Write($"{_programName}: ");
                Console.WriteLine(e.Message);
                Console.WriteLine($"Try `{_programName} --help' for more information.");
                _shouldExit = true;
                _exitCode = ExitCodes.Error;
            }

            // no arguments at all - show help
            if (_args.Length == 0)
            {
                _shouldShowHelp = true;
            }

            if (_shouldShowHelp)
            {
                deleteReportIfCreatedAlready();
                printHelp();
                _shouldExit = true;
            }
            else if (_shouldShowVersion)
            {
                deleteReportIfCreatedAlready();
                printVersion();
                _shouldExit = true;
            }

            return new Options()
            {
                targets = _targets,
                HtmlOutputStream = _htmlOutputStream!,
                SarifOutputStream = _sarifOutputStream!,
                HtmlOutputReportName = _htmlOutputReportName!,
                SarifOutputReportName = _sarifOutputReportName!,
                logLevel = _logLevel,
                shouldShowHelp = _shouldShowHelp,
                shouldExit = _shouldExit,
                exitCode = _exitCode,
                shouldShowVersion = _shouldShowVersion,
                shouldDiscoverAndExit = _shouldDiscoverAndExit,
                authenticationData = _authenticationData,
                commandLine = Environment.CommandLine,
            };
        }
    }
}
