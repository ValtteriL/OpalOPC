using Microsoft.Extensions.Logging;
using Model;
using OpalOPC.WPF;
using View;
using Xunit;

#if BUILT_FOR_WINDOWS
namespace Tests;
public class MainWindowViewModel_Tests
{

    // initial values when starting
    [Fact]
    public void Constructor()
    {
        MainWindowViewModel model = new();

        Assert.True(model.Title != null);
        Assert.True(model.TargetsLabel == "Targets");
        Assert.True(model.OutputFileLocation == Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\");
        Assert.True(model.TargetToAdd == string.Empty);
        Assert.True(model.Targets != null);
        Assert.True(model.Verbosity == LogLevel.Information);
        Assert.True(model.ScanCompletedSuccessfully == false);
    }

    // adding single target
    [Fact]
    public void Add_Single_Target()
    {
        MainWindowViewModel model = new();
        string target = "asd";
        model.TargetToAdd = target;

        model.AddTargetCommand.Execute(null);

        Assert.True(model.TargetToAdd == string.Empty);
        Assert.Contains($"opc.tcp://{target}", model.Targets);
    }

    // adding multiple targets
    [Fact]
    public void Add_Multiple_Targets()
    {
        MainWindowViewModel model = new();
        string target1 = "asd";
        string target2 = "opc.tcp://eee";
        string[] targets = { target1, target2, "" };

        string tempfile = Path.GetTempFileName();
        File.WriteAllLines(tempfile, targets);

        model.AddTargetsFromFile(tempfile);

        Assert.True(model.Targets.Length == 2); // empty should not be there
        Assert.Contains($"opc.tcp://{target1}", model.Targets);
        Assert.Contains($"{target2}", model.Targets);

        File.Delete(tempfile);
    }

    // choosing verbosity
    [Fact]
    public void Choose_Verbosity()
    {
        MainWindowViewModel model = new();

        model.NormalVerbosityCommand.Execute(null);
        Assert.True(model.Verbosity == LogLevel.Information);

        model.VerboseVerbosityCommand.Execute(null);
        Assert.True(model.Verbosity == LogLevel.Debug);

        model.TraceVerbosityCommand.Execute(null);
        Assert.True(model.Verbosity == LogLevel.Trace);
    }

    // removing target
    [Fact]
    public void Remove_Target()
    {
        MainWindowViewModel model = new();
        string target = "opc.tcp://asdasd";
        model.Targets = new string[] { target };

        model.DeleteTarget(target);

        Assert.True(model.Targets.Length == 0);
    }

    // selecting target
    [Fact]
    public void Select_Target()
    {
        MainWindowViewModel model = new();
        string target = "opc.tcp://asdasd";
        model.Targets = new string[] { target };

        model.SetTargetToAdd(target);

        Assert.True(model.TargetToAdd == target);
    }

    // scanning
    [Fact]
    public void Scan()
    {
        MainWindowViewModel model = new();
        string tempfile = Path.GetTempFileName();
        model.OutputFileLocation = tempfile;

        model.ScanCommand.Execute(null);

        Thread.Sleep(500);

        Assert.True(model.ScanCompletedSuccessfully);
        Assert.True(File.Exists(tempfile));

        File.Delete(tempfile);
    }

    // scanning and canceling
    [Fact]
    public void Scan_And_Cancel()
    {
        MainWindowViewModel model = new();
        string tempfile = Path.GetTempFileName();
        model.OutputFileLocation = tempfile;
        string target1 = "opc.tcp://opcuaserver.com:48010";
        string target2 = "opc.tcp://opcuaserver.com:4840";
        model.Targets = new string[] { target1, target2 };

        model.ScanCommand.Execute(null);
        model.ScanCancelCommand.Execute(null);

        Assert.True(!model.ScanCompletedSuccessfully);
    }

    // scanning with empty output
    [Fact]
    public void ScanWithEmptyOutputFilePath()
    {
        MainWindowViewModel model = new()
        {
            OutputFileLocation = string.Empty
        };

        model.ScanCommand.Execute(null);

        Thread.Sleep(500);
        Assert.True(model.ScanCompletedSuccessfully);
    }

}
#endif
