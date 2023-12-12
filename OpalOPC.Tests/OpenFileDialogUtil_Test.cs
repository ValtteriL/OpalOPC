using Microsoft.Extensions.Logging;
using Model;
using Moq;
using OpalOPC.WPF.GuiUtil;
using OpalOPC.WPF.Models;
using Opc.Ua;
using Plugin;
using Util;
using Xunit;

namespace Tests;
public class OpenFileDialogUtilTest
{

    private readonly Mock<IOpenFileDialog> _openFileDialog;
    private readonly Mock<IFilePathUtil> _filePathUtil;

    public OpenFileDialogUtilTest()
    {
        _openFileDialog = new Mock<IOpenFileDialog>();
        _filePathUtil = new Mock<IFilePathUtil>();
    }

    [Fact]
    public void ReturnsChosenFilename()
    {
        // arrange
        _openFileDialog.Setup(x => x.ShowDialog()).Returns(true);
        string filename = "C:\\Users\\user\\Desktop\\test.pem";
        _openFileDialog.Setup(x => x.FileName).Returns(filename);
        _filePathUtil.Setup(x => x.GetFullPath(filename)).Returns(filename);
        OpenFileDialogUtil openFileDialogUtil = new(_filePathUtil.Object);

        // act
        string path = openFileDialogUtil.GetFilePathFromUser(_openFileDialog.Object, "PEM files (*.pem)|*.pem");

        // assert
        Assert.Equal(filename, path);
    }

    [Fact]
    public void ReturnsEmptyIfShowDialogIsFalse()
    {
        // arrange
        _openFileDialog.Setup(x => x.ShowDialog()).Returns(false);
        string filename = "C:\\Users\\user\\Desktop\\test.pem";
        _openFileDialog.Setup(x => x.FileName).Returns(filename);
        _filePathUtil.Setup(x => x.GetFullPath(filename)).Returns(filename);
        OpenFileDialogUtil openFileDialogUtil = new(_filePathUtil.Object);

        // act
        string path = openFileDialogUtil.GetFilePathFromUser(_openFileDialog.Object, "PEM files (*.pem)|*.pem");

        // assert
        Assert.Equal(string.Empty, path);
    }

    [Fact]
    public void ReturnsEmptyIfFilenameEmpty()
    {
        // arrange
        _openFileDialog.Setup(x => x.ShowDialog()).Returns(false);
        string filename = string.Empty;
        _openFileDialog.Setup(x => x.FileName).Returns(filename);
        _filePathUtil.Setup(x => x.GetFullPath(filename)).Returns(filename);
        OpenFileDialogUtil openFileDialogUtil = new(_filePathUtil.Object);

        // act
        string path = openFileDialogUtil.GetFilePathFromUser(_openFileDialog.Object, "PEM files (*.pem)|*.pem");

        // assert
        Assert.Equal(string.Empty, path);
    }

    [Fact]
    public void ExceptionInFilePathUtilIsPropagated()
    {
        // arrange
        _openFileDialog.Setup(x => x.ShowDialog()).Returns(true);
        string filename = "C:\\Users\\user\\Desktop\\test.pem";
        _openFileDialog.Setup(x => x.FileName).Returns(filename);
        _filePathUtil.Setup(x => x.GetFullPath(filename)).Throws(new Exception());
        OpenFileDialogUtil openFileDialogUtil = new(_filePathUtil.Object);

        // act
        try
        {
            openFileDialogUtil.GetFilePathFromUser(_openFileDialog.Object, "PEM files (*.pem)|*.pem");
        }
        catch (Exception)
        {
            // assert
            Assert.True(true);
            return;
        }

        Assert.True(false);
    }

}
