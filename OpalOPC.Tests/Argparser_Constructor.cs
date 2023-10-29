namespace Tests;
using View;
using Model;
using Xunit;

public class Argparser_Constructor
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        string[] args = { "-v" };
        Argparser argparser = new(args);

        Assert.True(argparser != null);
    }

}