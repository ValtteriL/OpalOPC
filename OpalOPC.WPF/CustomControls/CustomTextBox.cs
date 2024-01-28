using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace OpalOPC.WPF.CustomControls;

public class CustomTextBox : TextBox
{
    public string HintText
    {
        get => (string)GetValue(HintTextProperty);
        set => SetValue(HintTextProperty, value);
    }

    public static readonly DependencyProperty HintTextProperty =
        DependencyProperty.Register(nameof(HintText), typeof(string), typeof(CustomTextBox), new PropertyMetadata(""));


    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(CustomTextBox), new PropertyMetadata(""));

    public string AutomationId
    {
        get => (string)GetValue(AutomationIdProperty);
        set => SetValue(AutomationIdProperty, value);
    }

    public static readonly DependencyProperty AutomationIdProperty =
        DependencyProperty.Register(nameof(AutomationId), typeof(string), typeof(CustomTextBox), new PropertyMetadata(""));


    static CustomTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTextBox),
            new FrameworkPropertyMetadata(typeof(CustomTextBox)));
    }
}
