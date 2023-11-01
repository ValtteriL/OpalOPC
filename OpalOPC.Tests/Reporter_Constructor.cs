using View;
using Xunit;

namespace Tests;
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
