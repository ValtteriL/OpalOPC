using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpalOPC.WPF;

public partial class MainWindowViewModel : ObservableObject
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
    private LogLevel? verbosity = LogLevel.Information;

    private bool scanCompletedSuccessfully = false;

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

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task Scan(CancellationToken token)
    {
        try {
            await Task.Delay(10_000);
        }
        catch(OperationCanceledException)
        {

        }
    }


    [RelayCommand]
    private void AddTarget()
    {
        if (TargetToAdd == string.Empty)
        {
            return;
        }

        Targets = Targets.Append(TargetToAdd).ToHashSet().ToArray();
        TargetToAdd = string.Empty;
        updateTargetsLabel();
    }


    [RelayCommand(CanExecute = nameof(canOpenReport))]
    private void OpenReport()
    {

    }

    private bool canOpenReport()
    {
        return scanCompletedSuccessfully;
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
        string[] lines = File.ReadAllLines(path);
        Targets = Targets.Union(lines).ToArray();
        updateTargetsLabel();
    }

    private void updateTargetsLabel()
    {
        TargetsLabel = $"Targets ({Targets.Length})";
    }
}
