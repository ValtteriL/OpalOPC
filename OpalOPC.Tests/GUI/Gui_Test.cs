using Xunit;

namespace Tests.GUI
{
    public class GuiTest : TestsBase, IDisposable
    {
        public GuiTest() : base() { }

        public void Dispose()
        {
            Cleanup();
            StopWinappDriver();
        }

        [Fact]
        public void EndToEnd()
        {
            // arrange

            // act

            // assert
            Assert.True(true);
        }
    }
}
