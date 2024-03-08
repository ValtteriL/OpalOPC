#if BUILT_FOR_WINDOWS
using OpalOPCWPF.ViewModels;
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
        Assert.Contains(model.ViewCollection, v => v.ViewType == OpalOPCWPF.Models.ViewType.ScanView);
        Assert.Contains(model.ViewCollection, v => v.ViewType == OpalOPCWPF.Models.ViewType.ConfigurationView);
        Assert.True(model.SelectedView != null);
        Assert.True(model.SelectedView.ViewType == OpalOPCWPF.Models.ViewType.ScanView);
    }
}
#endif
