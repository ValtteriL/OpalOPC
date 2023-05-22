namespace Tests;
using Model;
using Xunit;

public class Error_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        Error error = new Error("");

        Assert.True(error != null);
    }

    [Fact]
    public void constructor_SetsProperties()
    {
        const string message = "a";
        Error error = new Error(message);

        Assert.True(error.Message == message);
    }
}