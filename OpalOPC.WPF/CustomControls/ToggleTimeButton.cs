using System.Windows;
using System.Windows.Controls;

namespace OpalOPC.WPF.CustomControls;

public class ToggleTimeButton : Button
{
    /// <summary>
    /// Enable or Disable Timer functionality
    /// Default is True
    /// </summary>
    public bool IsEnableTimer
    {
        get => (bool)GetValue(IsEnableTimerProperty);
        set => SetValue(IsEnableTimerProperty, value);
    }

    public static readonly DependencyProperty IsEnableTimerProperty =
        DependencyProperty.Register(nameof(IsEnableTimer), typeof(bool), typeof(ToggleTimeButton),
            new PropertyMetadata(true));

    /// <summary>
    /// Time to wait (in seconds)
    /// Default is 5 seconds
    /// </summary>
    public int WaitSeconds
    {
        get => (int)GetValue(WaitSecondsProperty);
        set => SetValue(WaitSecondsProperty, value);
    }

    public static readonly DependencyProperty WaitSecondsProperty =
        DependencyProperty.Register(nameof(WaitSeconds), typeof(int), typeof(ToggleTimeButton),
            new PropertyMetadata(5));

    /// <summary>
    /// if set True, it will append time to button. Otherwise it will replace the text with time
    /// Default is True
    /// </summary>
    public bool IsAppendTime
    {
        get => (bool)GetValue(IsAppendTimeProperty);
        set => SetValue(IsAppendTimeProperty, value);
    }

    public static readonly DependencyProperty IsAppendTimeProperty =
        DependencyProperty.Register(nameof(IsAppendTime), typeof(bool), typeof(ToggleTimeButton),
            new PropertyMetadata(false));

    /// <summary>
    /// TextBlock Element where you want to show the time
    /// </summary>
    public TextBlock? TimeTextBlock
    {
        get => (TextBlock)GetValue(TimeTextBlockProperty);
        set => SetValue(TimeTextBlockProperty, value);
    }

    public static readonly DependencyProperty TimeTextBlockProperty =
        DependencyProperty.Register(nameof(TimeTextBlock), typeof(TextBlock), typeof(ToggleTimeButton),
            new PropertyMetadata(null));


    /// <summary>
    /// For INTERNAL USE ONLY
    /// </summary>
    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public static readonly DependencyProperty IsBusyProperty =
        DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(ToggleTimeButton),
            new PropertyMetadata(false));


    protected override async void OnClick()
    {
        base.OnClick();
        if (!IsEnableTimer)
        {
            return;
        }

        if (TimeTextBlock != null)
        {
            var originalText = TimeTextBlock.Text;
            var time = WaitSeconds;

            try
            {
                IsBusy = true;
                await Task.Run(async () =>
                {
                    while (time > 0)
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            if (IsAppendTime)
                            {
                                TimeTextBlock.Text = $"{originalText} {time}";
                            }
                            else
                            {
                                TimeTextBlock.Text = time.ToString();
                            }
                        });


                        time--;
                        await Task.Delay(1000);
                    }
                });

                TimeTextBlock.Text = originalText;
                IsBusy = false;
            }
            catch (Exception)
            {
                TimeTextBlock.Text = originalText;
                IsBusy = false;
            }
        }
    }
}
