using Util;
using View;
using Xunit;
using Moq;

namespace Tests;
public class EulaPrompterTest
{
    private readonly Mock<IFileUtil> _fileUtil;
    private readonly Mock<IConsoleUtil> _consoleUtil;

    public EulaPrompterTest()
    {
        _fileUtil = new Mock<IFileUtil>();
        _consoleUtil = new Mock<IConsoleUtil>();
    }

    [Theory]
    [InlineData("y", true)]
    [InlineData("Y", true)]
    [InlineData("n", false)]
    [InlineData("N", false)]
    public void EulaPromptReturnsCorrectValue(string input, bool correctOutput)
    {
        // arrange
        _consoleUtil.Setup(c => c.ReadLine()).Returns(input);
        _fileUtil.Setup(f => f.Create(It.IsAny<string>())).Returns(new MemoryStream());
        EulaPrompter eulaPrompter = new(_fileUtil.Object, _consoleUtil.Object);

        // act
        bool result = eulaPrompter.PromptUserForEulaAcceptance();

        // assert
        Assert.Equal(result, correctOutput);
    }

    [Fact]
    public void EulaPromptReturnsTrueIfEulaFileExists()
    {
        // arrange
        _fileUtil.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);
        _consoleUtil.Setup(c => c.ReadLine()).Returns("n");
        EulaPrompter eulaPrompter = new(_fileUtil.Object, _consoleUtil.Object);

        // act
        bool result = eulaPrompter.PromptUserForEulaAcceptance();

        // assert
        Assert.True(result);
    }

}
