namespace OpalOPC.WPF.Models;

public class View
{
    public ViewType ViewType { get; set; }
    public string Title { get; set; }
    public string IconPath { get; set; }
    public string AutomationId { get; internal set; }
}
