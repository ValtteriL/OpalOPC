﻿using System.Collections.ObjectModel;
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
using OpalOPC.WPF.Models;
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

        _scanViewModelUtil.CheckVersion(logger);

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

        // scan
        try
        {
            using Stream outputStream = _fileUtil.Create(_outputfile);
            _authenticationData = _scanViewModelUtil.GetAuthenticationData();
            ScanController scanController = new(logger, targetUris, outputStream, generateGUICommandInReport(), _authenticationData, token);

            await Task.Run(() =>
            {
                scanController.Scan();
                logger.LogInformation("{Message}", $"Report saved to {_outputfile} (Use browser to view it)");
            }, token);

            ScanCompletedSuccessfully = true;
        }
        catch (Exception e) when (e is UnauthorizedAccessException || e is IOException || e is PathTooLongException || e is DirectoryNotFoundException)
        {
            logger.LogError("{Message}", $"Error opening report file for writing: {e.Message}");
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

        if (Targets.Contains(modifiedTarget))
        {
            logger.LogWarning("{Message}", $"\"{modifiedTarget}\" is already a target. Skipping");
        }
        else
        {
            try
            {
                _ = new Uri(modifiedTarget);
            }
            catch (System.Exception)
            {
                logger.LogError("{Message}", $"\"{target}\" is invalid target");
                return;
            }

            Targets.Add(modifiedTarget);
        }
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

        return $"Launched via GUI with following settings: (" +
            $"Verbosity: {verbosityInReport} " +
            $"| Output: {OutputFileLocation} " +
            $"| Targets: {string.Join(", ", Targets)} " +
            $"| User authentication certificates: {_authenticationData.userCertificates} " +
            $"| User authentication credentials: {_authenticationData.loginCredentials} " +
            $"| Application authentication: {_authenticationData.applicationCertificates} " +
            $"| Brute force credentials: {_authenticationData.bruteForceCredentials} " +
            $")";
    }
}
