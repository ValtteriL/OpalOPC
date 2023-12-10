#if BUILT_FOR_WINDOWS
using OpalOPC.WPF.ViewModels;
using Xunit;

namespace Tests;
public class MainWindowViewModel_Tests
{

    // initial values when starting
    [Fact]
    public void Constructor()
    {
        MainWindowViewModel model = new();

        Assert.True(model.Title != null);
        Assert.True(model.ViewCollection != null);
        Assert.Contains(model.ViewCollection, v => v.ViewType == OpalOPC.WPF.Models.ViewType.ScanView);
        Assert.Contains(model.ViewCollection, v => v.ViewType == OpalOPC.WPF.Models.ViewType.ConfigurationView);
        Assert.True(model.SelectedView != null);
        Assert.True(model.SelectedView.ViewType == OpalOPC.WPF.Models.ViewType.ScanView);
    }
}
#endif
