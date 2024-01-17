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
        private Options _options = new();
        private readonly string _programName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        private readonly IFileUtil _fileUtil;

        public Argparser(string[] args)
        {
            _optionSet = new Mono.Options.OptionSet {
                { "i|input-file=", "input targets from list of discovery uris", il => _options = readTargetFile(il) },
                { "o|output=", "output XHTML report filename", ox => _options = setOutputFile(ox) },
                { "v", "increase verbosity (can be specified up to 2 times)", v => _options.logLevel = (v == null) ? _options.logLevel : _options.logLevel == LogLevel.Information ? LogLevel.Debug : LogLevel.Trace },
                { "h|help", "show this message and exit", h => _options.shouldShowHelp = h != null },
                { "s", "silence output (useful with -o -)", s => _options.logLevel = (s == null) ? _options.logLevel : LogLevel.None },
                { "l|login-credential=", "username:password for user authentication", l => _options = addLoginCredential(l) },
                { "b|brute-force-credential=", "username:password for brute force attack", b => _options = addBruteForceCredential(b) },
                { "L|login-credential-file=", "import list of username:password for authentication from file", l => _options = addLoginCredentialsFromFile(l) },
                { "B|brute-force-credential-file=", "import list of username:password for brute force attack from file", b => _options = addBruteForceCredentialsFromFile(b) },
                { "c|user-certificate-and-privatekey=", "path-to-certificate:path-to-privatekey for user authentication", c => _options = addUserCertificatePrivatekey(c) },
                { "a|application-certificate-and-privatekey=", "path-to-certificate:path-to-privatekey for application authentication", a => _options = addAppCertificatePrivatekey(a) },
                { "version", "show version and exit", ver => _options.shouldShowVersion = ver != null },
                { "accepteula", "suppress the display of the license dialog", ae => _options.acceptEula = ae != null },
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

        private ICollection<string> readFileToList(string path)
        {
            List<string> lines;

            try
            {
                lines = _fileUtil.ReadFileToList(path).ToList();

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

        private Options addAppCertificatePrivatekey(string paths)
        {
            (string certpath, string privkeypath) = splitStringToTwoByColon(paths);
            _options.authenticationData.AddApplicationCertificate(readCertificate(certpath, privkeypath));
            return _options;
        }

        private Options addUserCertificatePrivatekey(string paths)
        {
            (string certpath, string privkeypath) = splitStringToTwoByColon(paths);
            _options.authenticationData.AddUserCertificate(readCertificate(certpath, privkeypath));
            return _options;
        }

        private Options addBruteForceCredentialsFromFile(string path)
        {
            foreach (string line in readFileToList(path))
            {
                addBruteForceCredential(line);
            }

            return _options;
        }

        private Options addLoginCredentialsFromFile(string path)
        {
            foreach (string line in readFileToList(path))
            {
                addLoginCredential(line);
            }

            return _options;
        }

        private Options addLoginCredential(string credential)
        {
            (string username, string password) = splitStringToTwoByColon(credential);
            _options.authenticationData.AddLoginCredential(username, password);
            return _options;
        }

        private Options addBruteForceCredential(string credential)
        {
            (string username, string password) = splitStringToTwoByColon(credential);
            _options.authenticationData.AddBruteForceCredential(username, password);
            return _options;
        }

        private Options setOutputFile(string path)
        {
            if (path == "-")
            {
                // stdout
                _options.OutputStream = Console.OpenStandardOutput();
            }
            else
            {
                // the path
                _options.OutputReportName = path;

                try
                {
                    _options.OutputStream = File.Create(path);
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

            return _options;
        }

        private Options readTargetFile(string path)
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
                    lines = _fileUtil.ReadFileToList(path).ToList();
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

            return _options;
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

            _options.targets.Add(uri);
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
            if (_options.OutputReportName != null)
            {
                _options.OutputStream!.Dispose();
                File.Delete(_options.OutputReportName);
            }
        }

        public Options parseArgs()
        {
            try
            {
                // parse the command line
                List<string> extra = _optionSet.Parse(_args);
                extra.ForEach(e => appendTarget(e));
                if (_options.OutputStream == null)
                {
                    _options.OutputReportName = ArgUtil.DefaultReportName();
                    setOutputFile(_options.OutputReportName);
                }
            }
            catch (OptionException e)
            {
                // output some error message
                Console.Write($"{_programName}: ");
                Console.WriteLine(e.Message);
                Console.WriteLine($"Try `{_programName} --help' for more information.");
                _options.exitCode = ExitCodes.Error;
            }

            // no arguments at all - show help
            if (_args.Length == 0)
            {
                _options.shouldShowHelp = true;
            }

            if (_options.shouldShowHelp)
            {
                deleteReportIfCreatedAlready();
                printHelp();
                _options.exitCode = ExitCodes.Success;
            }
            else if (_options.shouldShowVersion)
            {
                deleteReportIfCreatedAlready();
                printVersion();
                _options.exitCode = ExitCodes.Success;
            }

            return _options;
        }
    }
}
