using Util;
using Xunit;

namespace Tests;
public class TelemetryUtilTest
{

    public TelemetryUtilTest()
    {

    }

    [Fact]
    public void IsEnabledByDefault()
    {
        // arrange

        // act

        // assert
        Assert.True(TelemetryUtil.Enabled);
    }

}
