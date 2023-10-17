namespace Tests;
using View;
using Model;
using Xunit;

public class Reporter_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        StreamWriter sw = new StreamWriter(new MemoryStream());
        sw.AutoFlush = true;
        Reporter reporter = new Reporter(sw.BaseStream);

        Assert.True(reporter != null);
    }

}