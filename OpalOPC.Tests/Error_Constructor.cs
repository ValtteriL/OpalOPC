using Model;
using Xunit;

namespace Tests;
public class Error_Constructor
{
    [Fact]
    public void constructor_SetsProperties()
    {
        const string message = "a";
        Error error = new(message);

        Assert.True(error.Message == message);
    }
}
