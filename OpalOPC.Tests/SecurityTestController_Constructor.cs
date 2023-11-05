using Controller;
using Microsoft.Extensions.Logging;
using Plugin;
using Xunit;

namespace Tests;
public class SecurityTestController_Constructor
{
    [Fact]
    public void constructor_DoesNotReturnNull()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
        ILogger logger = loggerFactory.CreateLogger<AuditingDisabledPluginTest>();
        SecurityTestController securityTestController = new(logger, new List<IPlugin>());

        Assert.True(securityTestController != null);
    }

}
