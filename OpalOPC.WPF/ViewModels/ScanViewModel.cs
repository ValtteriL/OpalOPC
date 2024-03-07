using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Controller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model;
using OpalOPCWPF.GuiUtil;
using OpalOPCWPF.Logger;
using ScannerApplication;
using Util;


namespace OpalOPCWPF.ViewModels;

public partial class ScanViewModel : ObservableObject, IRecipient<LogMessage>
{
    [ObservableProperty]
    private string _targetsLabel = "Targets";

    [ObservableProperty]
    private string _outputFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";

    [ObservableProperty]
    private string _targetToAdd = string.Empty;

    [ObservableProperty]
    private int _networkDiscoverySeconds = 5;

    [ObservableProperty] private ObservableCollection<Uri> _targets = [];

    [ObservableProperty]
    private string? _log = string.Empty;

    [ObservableProperty]
    private LogLevel _verbosity = LogLevel.Information;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenReportCommand))]
    private bool _scanCompletedSuccessfully = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NetworkDiscoveryCommand))]
    private bool _networkDiscoveryOnGoing = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NetworkDiscoveryCommand))]
    private bool _scanOnGoing = false;

    private string _outputfileBasename = string.Empty;
    private string _htmlOutputReportName => _outputfileBasename + ".html";
    private string _sarifOutputReportName => _outputfileBasename + ".sarif";
    const string Protocol = "opc.tcp://";

    private readonly IFileUtil _fileUtil;
    private readonly IMessageBoxUtil _messageBoxUtil;
    private readonly IScanViewModelUtil _scanViewModelUtil;
    private AuthenticationData _authenticationData = new();

    public ScanViewModel() : this(new FileUtil(), new MessageBoxUtil(), new ScanViewModelUtil())
    {
    }

    public ScanViewModel(IFileUtil fileUtil, IMessageBoxUtil messageBoxUtil, IScanViewModelUtil scanViewModelUtil)
    {
        // register to log messages from logger
        WeakReferenceMessenger.Default.Register<LogMessage>(this);

        _fileUtil = fileUtil;
        _messageBoxUtil = messageBoxUtil;
        _scanViewModelUtil = scanViewModelUtil;
    }

    [RelayCommand(CanExecute = nameof(canRunNetworkDiscovery))]
    private async Task NetworkDiscovery(int timeout)
    {
        NetworkDiscoveryOnGoing = true;
        Log = string.Empty;
        GUILoggerProvider loggerProvider = new(Verbosity);
        ILogger logger = loggerProvider.CreateLogger(string.Empty);

        logger.LogInformation("{Message}", "Starting network discovery");

        try
        {
            // configure application just to get the NetworkDiscoveryController
            IHost _host = AppConfigurer.ConfigureApplication(new Options() { logLevel = Verbosity }, loggerProvider);
            INetworkDiscoveryController networkDiscoveryController = _host.Services.GetRequiredService<INetworkDiscoveryController>();


            IList<Uri> discoveredTargets = await Task.Run(() =>
            {
                return networkDiscoveryController.MulticastDiscoverTargets(timeout);

            });

            // add discovered targets to targets
            foreach (Uri target in discoveredTargets)
            {
                AddTarget(target.AbsoluteUri, logger);
            }

            // let user know how many targets were added
            logger.LogInformation("{Message}", $"Discovered {discoveredTargets.Count} targets on network");
        }
        catch (Exception e)
        {
            logger.LogCritical("{Message}", $"Unhandled exception: {e}");
        }
        finally
        {
            NetworkDiscoveryOnGoing = false;
        }
    }

    [RelayCommand]
    private void NormalVerbosity()
    {
        Verbosity = LogLevel.Information;
    }

    [RelayCommand]
    private void VerboseVerbosity()
    {
        Verbosity = LogLevel.Debug;
    }

    [RelayCommand]
    private void TraceVerbosity()
    {
        Verbosity = LogLevel.Trace;
    }

    public void SetTargetToAdd(Uri target)
    {
        TargetToAdd = target.AbsoluteUri;
    }


    [RelayCommand(IncludeCancelCommand = true)]
    private async Task Scan(CancellationToken token)
    {
        Log = string.Empty;

        ScanCompletedSuccessfully = false;
        ScanOnGoing = true;
        GUILoggerProvider loggerProvider = new(Verbosity);

        // check if output file specified
        // if path points to dir => use generated output filename
        // if path points to file => use it
        // if neither => try to create new file
        if (Directory.Exists(OutputFileLocation) || OutputFileLocation == string.Empty)
        {
            _outputfileBasename = Path.Combine(OutputFileLocation, Util.ArgUtil.DefaultReportName());
        }
        else
        {
            _outputfileBasename = OutputFileLocation;
        }

        // scan
        try
        {
            using Stream htmlOutputStream = _fileUtil.Create(_htmlOutputReportName);
            using Stream sarifOutputStream = _fileUtil.Create(_sarifOutputReportName);
            _authenticationData = _scanViewModelUtil.GetAuthenticationData();

            Options options = new()
            {
                authenticationData = _authenticationData,
                targets = [.. Targets],
                commandLine = generateGUICommandInReport(),
                HtmlOutputReportName = _htmlOutputReportName,
                SarifOutputReportName = _sarifOutputReportName,
                HtmlOutputStream = htmlOutputStream,
                SarifOutputStream = sarifOutputStream,
            };

            await Task.Run(() =>
            {

                IHost _host = AppConfigurer.ConfigureApplication(options, loggerProvider);

                // set token for cancellation
                ITaskUtil taskUtil = _host.Services.GetRequiredService<ITaskUtil>();
                taskUtil.token = token;

                // run
                IWorker worker = _host.Services.GetRequiredService<IWorker>();
                worker.Run(options);

            }, token);

            ScanCompletedSuccessfully = true;
        }
        catch (Exception e) when (e is UnauthorizedAccessException || e is IOException || e is PathTooLongException || e is DirectoryNotFoundException)
        {
            loggerProvider.CreateLogger(string.Empty).LogError("{Message}", $"Error opening report file for writing: {e.Message}");
        }
        catch (OperationCanceledException)
        {
            loggerProvider.CreateLogger(string.Empty).LogWarning("{Message}", $"Scan canceled");
        }
        catch (Exception ex)
        {
            loggerProvider.CreateLogger(string.Empty).LogCritical("{Message}", $"Unhandled exception: {ex}");
        }
        finally
        {
            ScanOnGoing = false;
        }
    }


    [RelayCommand]
    private void AddTarget()
    {
        ILogger logger = new GUILogger(Verbosity);

        if (TargetToAdd == string.Empty)
        {
            return;
        }

        AddTarget(TargetToAdd, logger);

        TargetToAdd = string.Empty;
        updateTargetsLabel();
    }

    private void AddTarget(string target, ILogger logger)
    {
        string modifiedTarget = target;

        if (!modifiedTarget.StartsWith(Protocol))
        {
            modifiedTarget = Protocol + target;
        }

        Uri uri;
        try
        {
            uri = new Uri(modifiedTarget);
        }
        catch (System.Exception)
        {
            logger.LogError("{Message}", $"\"{modifiedTarget}\" is invalid target");
            return;
        }


        if (Targets.Contains(uri))
        {
            logger.LogWarning("{Message}", $"\"{uri}\" is already a target. Skipping");
        }
        else
        {
            Targets.Add(uri);
        }
    }


    [RelayCommand(CanExecute = nameof(canOpenReport))]
    private void OpenReport()
    {
        Process.Start(new ProcessStartInfo(_htmlOutputReportName) { UseShellExecute = true });
    }

    private bool canOpenReport()
    {
        return ScanCompletedSuccessfully;
    }

    private bool canRunNetworkDiscovery()
    {
        return !NetworkDiscoveryOnGoing && !ScanOnGoing;
    }

    public void SetOutputFileLocation(string fullPath)
    {
        OutputFileLocation = fullPath;
    }

    public void DeleteTarget(Uri target)
    {
        Targets.Remove(target);
        updateTargetsLabel();
    }

    public void AddTargetsFromFile(string path)
    {
        try
        {
            ILogger logger = new GUILogger(Verbosity);
            _fileUtil.ReadFileToList(path).ToList().ForEach(line =>
            {
                AddTarget(line, logger);

            });
        }
        catch (Exception e)
        {
            _messageBoxUtil.Show(e.Message);
        }
        updateTargetsLabel();

    }

    private void updateTargetsLabel()
    {
        TargetsLabel = $"Targets ({Targets.Count})";
    }

    public void Receive(LogMessage message)
    {
        Log = Log + message.Value + Environment.NewLine;
    }

    private string generateGUICommandInReport()
    {
        string verbosityInReport = Verbosity switch
        {
            LogLevel.Debug => "Debug",
            LogLevel.Trace => "Trace",
            LogLevel.Information => "Normal",
            LogLevel.Warning => "Warning",
            LogLevel.Error => "Error",
            LogLevel.Critical => "Critical",
            LogLevel.None => "None",
            _ => "Unknown",
        };
        string userCertificates = FormatList(_authenticationData.userCertificates);
        string loginCredentials = FormatList(_authenticationData.loginCredentials);
        string applicationCertificates = FormatList(_authenticationData.applicationCertificates);
        string bruteForceCredentials = FormatList(_authenticationData.bruteForceCredentials);

        return $"Launched via GUI with following settings: (" +
            $"Verbosity: {verbosityInReport} " +
            $"| Output: {OutputFileLocation} " +
            $"| Targets: {string.Join(", ", Targets)} " +
            $"| User authentication certificates: {userCertificates} " +
            $"| User authentication credentials: {loginCredentials} " +
            $"| Application authentication: {applicationCertificates} " +
            $"| Brute force credentials: {bruteForceCredentials} " +
            $")";
    }

    private static string FormatList<T>(IEnumerable<T> list)
    {
        return string.Join(", ", list);
    }
}
