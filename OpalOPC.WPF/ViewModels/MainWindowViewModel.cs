using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Controller;
using Microsoft.Extensions.Logging;
using OpalOPC.WPF.Logger;
using OpalOPC.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpalOPC.WPF;

public partial class MainWindowViewModel : ObservableObject, IRecipient<LogMessage>
{
    [ObservableProperty]
    private string title = $"OpalOPC {Util.VersionUtil.AppAssemblyVersion}";

    [ObservableProperty]
    private string targetsLabel = "Targets";

    [ObservableProperty]
    private string outputFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";

    [ObservableProperty]
    private string targetToAdd = string.Empty;

    [ObservableProperty]
    private string[] targets = Array.Empty<string>();

    [ObservableProperty]
    private string? log = string.Empty;

    [ObservableProperty]
    private LogLevel verbosity = LogLevel.Information;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenReportCommand))]
    private bool scanCompletedSuccessfully = false;

    private string outputfile = string.Empty;
    const string protocol = "opc.tcp://";

    public MainWindowViewModel()
    {
        // register to log messages from logger
        WeakReferenceMessenger.Default.Register<LogMessage>(this);
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

        // create URI list of targets
        List<Uri> targetUris = new List<Uri>();
        foreach (String target in Targets)
        {
            try
            {
                targetUris.Add(new Uri(target));
            }
            catch (System.Exception)
            {
                logger.LogError($"\"{target}\" is invalid target", "");
            }
        }

        // check if output file specified
        outputfile = OutputFileLocation;
        if (!File.Exists(OutputFileLocation))
        {
            outputfile = Path.Combine(OutputFileLocation, new Util.ArgUtil().DefaultReportName());
        }
        FileStream outputStream = File.OpenWrite(outputfile);


        ScanController scanController = new ScanController(logger, targetUris, outputStream, generateGUICommandInReport(), token);

        // scan
        try
        {
            await Task.Run(() => {
                scanController.Scan();
                logger.LogInformation($"Report saved to {outputfile} (Use browser to view it)");
                outputStream.Close();
            }, token);

            ScanCompletedSuccessfully = true;
        }
        catch(OperationCanceledException)
        {
            logger.LogWarning($"Scan canceled");
        }
        catch(Exception ex)
        {
            logger.LogCritical($"Unhandled exception: {ex}");
        }
    }


    [RelayCommand]
    private void AddTarget()
    {
        if (TargetToAdd == string.Empty)
        {
            return;
        }

        // add protocol if missing
        string target = TargetToAdd;
        if (!target.StartsWith(protocol))
        {
            target = protocol + target;
        }

        Targets = Targets.Append(target).ToHashSet().ToArray();
        TargetToAdd = string.Empty;
        updateTargetsLabel();
    }


    [RelayCommand(CanExecute = nameof(canOpenReport))]
    private void OpenReport()
    {
        Process.Start(new ProcessStartInfo(outputfile) { UseShellExecute = true });
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
        var targetsSet = Targets.ToHashSet();
        targetsSet.Remove(target);
        Targets = targetsSet.ToArray();
        updateTargetsLabel();
    }

    public void AddTargetsFromFile(string path)
    {
        string[] nonEmptyLines = File.ReadAllLines(path).ToList().Where(t => t != String.Empty).ToArray();

        for (int i = 0; i < nonEmptyLines.Length; i++)
        {
            if (!nonEmptyLines[i].StartsWith(protocol))
            {
                nonEmptyLines[i] = protocol + nonEmptyLines[i];
            }
        }

        Targets = Targets.Union(nonEmptyLines).Where(t => t != String.Empty).ToArray();
        updateTargetsLabel();
    }

    private void updateTargetsLabel()
    {
        TargetsLabel = $"Targets ({Targets.Length})";
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
            default:
                verbosityInReport = "Normal";
                break;
        }


        return $"Launched via GUI with following settings: (Verbosity: {verbosityInReport} | Output: {OutputFileLocation} | Targets: {String.Join(", ", Targets)})";
    }
}
