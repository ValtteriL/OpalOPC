namespace Tests;
using Model;
using View;
using Xunit;

public class Reporter_Constructor
{
    [Fact]
    public void ConstructorDoesNotReturnNull()
    {
        StreamWriter sw = new(new MemoryStream())
        {
            AutoFlush = true
        };
        Reporter reporter = new(sw.BaseStream);

        Assert.True(reporter != null);
    }

}
