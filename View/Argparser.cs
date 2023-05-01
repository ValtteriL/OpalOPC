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
                { "o|output=", "output XML report filename", ox => options.xmlOutputFile = ox },
                { "v", "verbose output", v => options.verbose = v != null },
                { "d", "debug output", d => options.debug = d != null },
                { "h|help", "show this message and exit", h => options.shouldShowHelp = h != null },
            };

            this.args = args;
        }

        private Options readTargetFile(string path)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(path);
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
            }
            catch (OptionException e)
            {
                // output some error message
                Console.Write($"{programName}: ");
                Console.WriteLine(e.Message);
                Console.WriteLine($"Try `{programName} --help' for more information.");
                Environment.Exit(Util.ExitCodes.Error);
            }

            if (options.shouldShowHelp)
            {
                printHelp();
                Environment.Exit(Util.ExitCodes.Success);
            }

            return options;
        }
    }
}