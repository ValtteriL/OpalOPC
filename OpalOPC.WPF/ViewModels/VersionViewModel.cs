﻿using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpalOPCWPF.ViewModels;

public partial class VersionViewModel : ObservableObject
{
    [ObservableProperty] private string _version;
    public VersionViewModel()
    {
        Version = $"{Util.VersionUtil.AppVersion}";
    }

    [RelayCommand]
    public static void Navigate(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
