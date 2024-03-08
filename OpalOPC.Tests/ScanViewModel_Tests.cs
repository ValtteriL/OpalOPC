#if BUILT_FOR_WINDOWS
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using OpalOPCWPF.GuiUtil;
using OpalOPCWPF.ViewModels;
using Util;
using Xunit;

namespace Tests;
public class ScanViewModel_Tests
{
    private readonly Mock<IScanViewModelUtil> _scanViewModelUtilMock;
    private readonly Mock<IFileUtil> _fileUtilMock;
    private readonly Mock<IMessageBoxUtil> _messageBoxUtilMock;

    public ScanViewModel_Tests()
    {
        _scanViewModelUtilMock = new Mock<IScanViewModelUtil>();
        _fileUtilMock = new Mock<IFileUtil>();
        _messageBoxUtilMock = new Mock<IMessageBoxUtil>();
    }

    // initial values when starting
    [Fact]
    public void Constructor()
    {
        ScanViewModel model = new();

        Assert.True(model.TargetsLabel != null);
        Assert.True(model.OutputFileLocation != null);
        Assert.True(model.TargetToAdd != null);
        Assert.True(model.Targets != null);
        Assert.True(model.Log != null);
        Assert.True(model.Verbosity == LogLevel.Information);
        Assert.True(model.ScanCompletedSuccessfully == false);
    }

    // adding single target
    [Fact]
    public void Add_Single_Target()
    {
        ScanViewModel model = new();
        string target = "asd";
        model.TargetToAdd = target;

        model.AddTargetCommand.Execute(null);

        Assert.True(model.TargetToAdd == string.Empty);
        Assert.Contains(new Uri($"opc.tcp://{target}"), model.Targets);
    }

    // adding multiple targets
    [Fact]
    public void Add_Multiple_Targets()
    {
        ScanViewModel model = new();
        string target1 = "asd";
        string target2 = "opc.tcp://eee";
        string[] targets = [target1, target2, ""];

        string tempfile = Path.GetTempFileName();
        File.WriteAllLines(tempfile, targets);

        model.AddTargetsFromFile(tempfile);

        Assert.True(model.Targets.Count == 3);
        Assert.Contains(new Uri($"opc.tcp://{target1}"), model.Targets);
        Assert.Contains(new Uri($"{target2}"), model.Targets);

        File.Delete(tempfile);
    }

    // choosing verbosity
    [Fact]
    public void Choose_Verbosity()
    {
        ScanViewModel model = new();

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
        ScanViewModel model = new();
        Uri target = new("opc.tcp://asdasd");
        model.Targets.Add(target);

        model.DeleteTarget(target);

        Assert.True(model.Targets.Count == 0);
    }

    // selecting target
    [Fact]
    public void Select_Target()
    {
        ScanViewModel model = new();
        Uri target = new("opc.tcp://asdasd");
        model.Targets.Add(target);

        model.SetTargetToAdd(target);

        Assert.True(model.TargetToAdd == target.AbsoluteUri);
    }

    // scanning
    [Fact]
    public async void Scan()
    {
        _scanViewModelUtilMock.Setup(x => x.GetAuthenticationData()).Returns(new AuthenticationData());
        _fileUtilMock.Setup(x => x.Create(It.IsAny<string>())).Returns(new MemoryStream());

        ScanViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object, _scanViewModelUtilMock.Object);
        string tempfile = Path.GetTempFileName();
        model.OutputFileLocation = tempfile;

        await model.ScanCommand.ExecuteAsync(null);

        Assert.True(model.ScanCompletedSuccessfully);
        Assert.True(File.Exists(tempfile));

        File.Delete(tempfile);
    }

    // scanning and canceling
    [Fact]
    public void Scan_And_Cancel()
    {
        ScanViewModel model = new();
        string tempfile = Path.GetTempFileName();
        model.OutputFileLocation = tempfile;
        Uri target1 = new("opc.tcp://opcuaserver.com:48010");
        Uri target2 = new("opc.tcp://opcuaserver.com:4840");
        model.Targets.Add(target1);
        model.Targets.Add(target2);

        model.ScanCommand.Execute(null);
        model.ScanCancelCommand.Execute(null);

        Assert.True(!model.ScanCompletedSuccessfully);
    }

    // scanning with empty output
    [Fact]
    public async void ScanWithEmptyOutputFilePath()
    {
        _scanViewModelUtilMock.Setup(x => x.GetAuthenticationData()).Returns(new AuthenticationData());
        _fileUtilMock.Setup(x => x.Create(It.IsAny<string>())).Returns(new MemoryStream());

        ScanViewModel model = new(_fileUtilMock.Object, _messageBoxUtilMock.Object, _scanViewModelUtilMock.Object)
        {
            OutputFileLocation = string.Empty
        };

        await model.ScanCommand.ExecuteAsync(null);

        Assert.True(model.ScanCompletedSuccessfully);
    }

    // network discovery
    [Fact]
    public async void NetworkDiscovery()
    {
        ScanViewModel model = new();

        await model.NetworkDiscoveryCommand.ExecuteAsync(5);

        Assert.True(!model.NetworkDiscoveryOnGoing);

    }

}
#endif
