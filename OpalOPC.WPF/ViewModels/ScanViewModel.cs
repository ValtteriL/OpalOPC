using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Controller;
using Microsoft.Extensions.Logging;
using Model;
using OpalOPC.WPF.GuiUtil;
using OpalOPC.WPF.Logger;
using Util;


namespace OpalOPC.WPF.ViewModels;

public partial class ScanViewModel : ObservableObject, IRecipient<LogMessage>
{
    [ObservableProperty]
    private string _targetsLabel = "Targets";

    [ObservableProperty]
    private string _outputFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";

    [ObservableProperty]
    private string _targetToAdd = string.Empty;

    [ObservableProperty] private ObservableCollection<string> _targets = new();

    [ObservableProperty]
    private string? _log = string.Empty;

    [ObservableProperty]
    private LogLevel _verbosity = LogLevel.Information;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenReportCommand))]
    private bool _scanCompletedSuccessfully = false;

    private string _outputfile = string.Empty;
    const string Protocol = "opc.tcp://";

    private readonly IFileUtil _fileUtil;
    private readonly IMessageBoxUtil _messageBoxUtil;

    public ScanViewModel() : this(new FileUtil(), new MessageBoxUtil())
    {
    }

    public ScanViewModel(IFileUtil fileUtil, IMessageBoxUtil messageBoxUtil)
    {
        // register to log messages from logger
        WeakReferenceMessenger.Default.Register<LogMessage>(this);

        _fileUtil = fileUtil;
        _messageBoxUtil = messageBoxUtil;
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

    public void SetTargetToAdd(string target)
    {
        TargetToAdd = target;
    }


    [RelayCommand(IncludeCancelCommand = true)]
    private async Task Scan(CancellationToken token)
    {
        Log = string.Empty;

        ScanCompletedSuccessfully = false;
        ILogger logger = new GUILogger(Verbosity);

        VersionCheckController versionCheckController = new(logger);
        versionCheckController.CheckVersion();

        // create URI list of targets
        List<Uri> targetUris = new();
        foreach (string target in Targets)
        {
            try
            {
                targetUris.Add(new Uri(target));
            }
            catch (System.Exception)
            {
                logger.LogError("{Message}", $"\"{target}\" is invalid target");
            }
        }

        // check if output file specified
        // if path points to dir => use generated output filename
        // if path points to file => use it
        // if neither => try to create new file
        if (Directory.Exists(OutputFileLocation) || OutputFileLocation == string.Empty)
        {
            _outputfile = System.IO.Path.Combine(OutputFileLocation, Util.ArgUtil.DefaultReportName());
        }
        else
        {
            _outputfile = OutputFileLocation;
        }

        // check that can write to file
        FileStream outputStream;
        try
        {
            outputStream = File.Create(_outputfile);
        }
        catch (UnauthorizedAccessException)
        {
            logger.LogError("{Message}", $"Not authorized to open \"{_outputfile}\" for writing");
            return;
        }
        catch
        {
            logger.LogError("{Message}", $"Unable to open \"{_outputfile}\" for writing");
            return;
        }

        AuthenticationData authenticationData = new(); // TODO: add support for authentication
        ScanController scanController = new(logger, targetUris, outputStream, generateGUICommandInReport(), authenticationData, token);

        // scan
        try
        {
            await Task.Run(() =>
            {
                scanController.Scan();
                logger.LogInformation("{Message}", $"Report saved to {_outputfile} (Use browser to view it)");
                outputStream.Close();
            }, token);

            ScanCompletedSuccessfully = true;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("{Message}", $"Scan canceled");
        }
        catch (Exception ex)
        {
            logger.LogCritical("{Message}", $"Unhandled exception: {ex}");
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

        // add protocol if missing
        string target = TargetToAdd;
        if (!target.StartsWith(Protocol))
        {
            target = Protocol + target;
        }

        if (Targets.Contains(target))
        {
            logger.LogWarning("{Message}", $"\"{target}\" is already a target. Skipping");
        }
        else
        {
            Targets.Add(target);
        }

        TargetToAdd = string.Empty;
        updateTargetsLabel();
    }


    [RelayCommand(CanExecute = nameof(canOpenReport))]
    private void OpenReport()
    {
        Process.Start(new ProcessStartInfo(_outputfile) { UseShellExecute = true });
    }

    private bool canOpenReport()
    {
        return ScanCompletedSuccessfully;
    }

    public void SetOutputFileLocation(string fullPath)
    {
        OutputFileLocation = fullPath;
    }

    public void DeleteTarget(string target)
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
                string target = line;

                if (!target.StartsWith(Protocol))
                {
                    target = Protocol + target;
                }

                if (Targets.Contains(target))
                {
                    logger.LogWarning("{Message}", $"\"{target}\" is already a target. Skipping");
                }
                else
                {
                    Targets.Add(target);
                }

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
        string verbosityInReport;
        switch (Verbosity)
        {
            case LogLevel.Debug:
                verbosityInReport = "Debug";
                break;
            case LogLevel.Trace:
                verbosityInReport = "Trace";
                break;
            case LogLevel.Information:
                verbosityInReport = "Normal";
                break;
            default:
                throw new NotImplementedException();
        }


        return $"Launched via GUI with following settings: (Verbosity: {verbosityInReport} | Output: {OutputFileLocation} | Targets: {string.Join(", ", Targets)})";
    }
}
