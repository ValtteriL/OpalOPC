namespace Tests;
using Model;
using Xunit;

public class Target_Constructor
{
    [Fact]
    public void constructor_NullInputResultsinNullReferenceException()
    {
        Opc.Ua.ApplicationDescription description = new Opc.Ua.ApplicationDescription();

        try
        {
            Target target = new Target(description);   
        }
        catch (System.NullReferenceException)
        {
            Assert.True(true);
            return;
        }

        Assert.True(false);
    }

    [Fact]
    public void constructor_SetsProperties()
    {
        Opc.Ua.ApplicationDescription description = new Opc.Ua.ApplicationDescription();
        string applicationName = "a";
        string applicationUri = "b";
        string productUri = "c";
        description.ApplicationType = Opc.Ua.ApplicationType.Server;
        description.ApplicationName = applicationName;
        description.ApplicationUri = applicationUri;
        description.ProductUri = productUri;

        Target target = new Target(description);

        Assert.True(target.Servers != null);
        Assert.True(target.ApplicationName == applicationName);
        Assert.True(target.Type == Opc.Ua.ApplicationType.Server);
        Assert.True(target.ApplicationUri == applicationUri);
        Assert.True(target.ProductUri == productUri);
    }
}