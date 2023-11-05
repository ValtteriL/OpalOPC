using View;
using Xunit;

namespace Tests;
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
