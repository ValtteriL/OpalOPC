namespace Tests;
using View;
using Model;
using Plugin;
using Xunit;
using Microsoft.Extensions.Logging;
using Controller;

public class SecurityTestController_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        var loggerFactory = LoggerFactory.Create(builder => {});
        ILogger logger = loggerFactory.CreateLogger<AuditingDisabledPluginTest>();
        SecurityTestController securityTestController = new(logger, new List<IPlugin>());

        Assert.True(securityTestController != null);
    }

}