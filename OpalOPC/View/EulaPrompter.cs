using Util;
using static System.Environment;

namespace View
{
    public class EulaPrompter
    {
        private readonly string _dirName = "OpalOPC";
        private readonly string _eulaFilename = "accepteula";
        private readonly IFileUtil _fileUtil;
        private readonly IConsoleUtil _consoleUtil;
        private readonly string _eulaFilePath;

        public EulaPrompter() : this(new FileUtil(), new ConsoleUtil())
        {
        }

        // constructor for tests 
        public EulaPrompter(IFileUtil fileUtil, IConsoleUtil consoleUtil)
        {
            _fileUtil = fileUtil;
            _consoleUtil = consoleUtil;

            // create directory if it doesn't exist - this is cross platform
            // see https://developers.redhat.com/blog/2018/11/07/dotnet-special-folder-api-linux#
            string certDir = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.Create), _dirName);
            Directory.CreateDirectory(certDir);
            _eulaFilePath = Path.Combine(certDir, _eulaFilename);
        }

        private bool CanSkipPromptingForEula()
        {
            return _fileUtil.FileExists(_eulaFilePath);
        }

        public void PersistAcceptChoice()
        {
            _fileUtil.Create(_eulaFilePath).Dispose();
        }

        public bool PromptUserForEulaAcceptance()
        {
            if (CanSkipPromptingForEula())
            {
                return true;
            }

            _consoleUtil.WriteLine("Do you accept the EULA available at https://opalopc.com/EULA.txt ? (y/n)");

            // keep asking until user gives valid input
            while (true)
            {
                string? input = _consoleUtil.ReadLine();

                if (input == null || input.ToLower() == "n")
                {
                    _consoleUtil.WriteLine("You must accept the EULA to use this software. Terminating.");
                    return false;
                }

                if (input.ToLower() == "y")
                {
                    break;
                }

                _consoleUtil.WriteLine("Invalid input, please try again (y/n)");
            }

            PersistAcceptChoice();
            return true;
        }
    }
}
