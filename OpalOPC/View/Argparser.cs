using Microsoft.Extensions.Logging;
using Model;
using Mono.Options;

namespace View
{
    public class Argparser
    {
        private string[] args;
        private OptionSet optionSet;
        private Options options = new Options();
        private string programName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

        public Argparser(string[] args)
        {
            optionSet = new OptionSet {
                { "i|input-file=", "input targets from list of discovery uris", il => options = readTargetFile(il) },
                { "o|output=", "output XHTML report filename", ox => options = setOutputFile(ox) },
                { "v", "increase verbosity (can be specified up to 2 times)", v => options.logLevel = (v == null) ? options.logLevel : options.logLevel == LogLevel.Information ? LogLevel.Debug : LogLevel.Trace },
                { "h|help", "show this message and exit", h => options.shouldShowHelp = h != null },
                { "s", "silence output (useful with -o -)", s => options.logLevel = (s == null) ? options.logLevel : LogLevel.None },
            };

            this.args = args;
        }

        private Options setOutputFile(string path)
        {
            if (path == "-")
            {
                // stdout
                options.xmlOutputStream = Console.OpenStandardOutput();
            }
            else
            {
                // the path
                options.xmlOutputReportName = path;

                try
                {
                    options.xmlOutputStream = File.OpenWrite(path);
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

            return options;
        }

        private Options readTargetFile(string path)
        {
            List<string> lines = new List<string>();

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
                    lines = File.ReadAllLines(path).ToList();
                }
            }
            catch (System.Exception e)
            {
                throw new OptionException(e.Message, "");
            }

            foreach (string line in lines)
            {
                appendTarget(line);
            }

            return options;
        }

        private void appendTarget(string target)
        {
            Uri uri;

            try
            {
                uri = new Uri(target);
            }
            catch (System.Exception)
            {
                throw new OptionException($"\"{target}\" is invalid target", "");
            }

            options.targets.Add(uri);
        }

        private void printHelp()
        {
            Console.WriteLine($"Opal OPC {Util.VersionUtil.AppAssemblyVersion} ( https://opalopc.com )");
            Console.WriteLine($"Usage: {programName} [Options] [Target ...]");
            optionSet.WriteOptionDescriptions(Console.Out);
        }

        public Options parseArgs()
        {
            try
            {
                // parse the command line
                List<string> extra = optionSet.Parse(args);
                extra.ForEach(e => appendTarget(e));
                if (options.xmlOutputStream == null)
                {
                    options.xmlOutputReportName = new Util.ArgUtil().DefaultReportName();
                    options.xmlOutputStream = File.OpenWrite(options.xmlOutputReportName);
                }
            }
            catch (OptionException e)
            {
                // output some error message
                Console.Write($"{programName}: ");
                Console.WriteLine(e.Message);
                Console.WriteLine($"Try `{programName} --help' for more information.");
                options.exitCode = Util.ExitCodes.Error;
            }

            // no arguments at all - show help
            if (args.Length == 0)
            {
                options.shouldShowHelp = true;
            }

            if (options.shouldShowHelp)
            {
                if (options.xmlOutputReportName != null)
                {
                    options.xmlOutputStream!.Close();
                    File.Delete(options.xmlOutputReportName);
                }

                printHelp();
                options.exitCode = Util.ExitCodes.Success;
            }

            return options;
        }
    }
}