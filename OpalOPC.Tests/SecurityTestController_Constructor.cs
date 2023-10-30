namespace Tests;
using Controller;
using Microsoft.Extensions.Logging;
using Model;
using Plugin;
using View;
using Xunit;

public class SecurityTestController_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<AuditingDisabledPluginTest>();
        SecurityTestController securityTestController = new(logger, new List<IPlugin>());

        Assert.True(securityTestController != null);
    }

}
