﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Automation.Provider;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpalOPC.WPF.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = $"OpalOPC {Util.VersionUtil.AppAssemblyVersion}";

    [ObservableProperty] private ObservableCollection<Models.View> viewCollection;
    [ObservableProperty] private Models.View selectedView;

    public MainWindowViewModel()
    {
        LoadMenuItems();
    }

    private void LoadMenuItems()
    {
        ViewCollection = new ObservableCollection<Models.View>
        {
            new()
            {
                ViewType = Models.ViewType.ScanView,
                Title = "Scan",
                IconPath =
                    "M2 0C.9 0 0 .9 0 2V6H2V2H6V0H2M18 0V2H22V6H24V2C24 .9 23.1 0 22 0H18M9.5 13C8.7 13 8 12.3 8 11.5S8.7 10 9.5 10 11 10.7 11 11.5 10.3 13 9.5 13M11 15L12 13L13 15H11M14.5 13C13.7 13 13 12.3 13 11.5S13.7 10 14.5 10 16 10.7 16 11.5 15.3 13 14.5 13M0 18V22C0 23.1 .9 24 2 24H6V22H2V18H0M22 18V22H18V24H22C23.1 24 24 23.1 24 22V18H22M12 3C7.6 3 4 6.6 4 11C4 13 4.8 14.9 6 16.3V21H18V16.3C19.2 14.9 20 13.1 20 11C20 6.6 16.4 3 12 3M16 15.4V19H14V17H13V19H11V17H10V19H8V15.4C6.8 14.3 6 12.7 6 11C6 7.7 8.7 5 12 5S18 7.7 18 11C18 12.8 17.2 14.3 16 15.4Z",
            },
            new()
            {
                ViewType = Models.ViewType.ConfigurationView,
                Title = "Configuration",
                IconPath =
                    "M150 210.8625A60.8625 60.8625 0 1 1 150 89.1375A60.8625 60.8625 0 0 1 150 210.8625zM107.8875 150A42.1125 42.1125 0 1 0 192.1125 150A42.1125 42.1125 0 0 0 107.8875 150zM183.675 274.81875C173.79375 308.38125 126.20625 308.38125 116.325 274.81875L114.5625 268.8375A16.36875 16.36875 0 0 0 91.03125 259.0875L85.55625 262.0875C54.80625 278.8125 21.1875 245.175 37.93125 214.44375L40.9125 208.96875A16.36875 16.36875 0 0 0 31.1625 185.4375L25.18125 183.675C-8.38125 173.79375 -8.38125 126.20625 25.18125 116.325L31.1625 114.5625A16.36875 16.36875 0 0 0 40.9125 91.03125L37.9125 85.55625C21.1875 54.80625 54.80625 21.16875 85.55625 37.93125L91.03125 40.9125A16.36875 16.36875 0 0 0 114.5625 31.1625L116.325 25.18125C126.20625 -8.38125 173.79375 -8.38125 183.675 25.18125L185.4375 31.1625A16.36875 16.36875 0 0 0 208.96875 40.9125L214.44375 37.9125C245.19375 21.16875 278.83125 54.825 262.06875 85.55625L259.0875 91.03125A16.36875 16.36875 0 0 0 268.8375 114.5625L274.81875 116.325C308.38125 126.20625 308.38125 173.79375 274.81875 183.675L268.8375 185.4375A16.36875 16.36875 0 0 0 259.0875 208.96875L262.0875 214.44375C278.83125 245.19375 245.175 278.8125 214.44375 262.06875L208.96875 259.0875A16.36875 16.36875 0 0 0 185.4375 268.8375L183.675 274.81875zM134.30625 269.5125C138.91875 285.16875 161.08125 285.16875 165.69375 269.5125L167.45625 263.53125A35.11875 35.11875 0 0 1 217.95 242.625L223.40625 245.625C237.73125 253.40625 253.40625 237.75 245.60625 223.40625L242.625 217.93125A35.11875 35.11875 0 0 1 263.55 167.45625L269.5125 165.69375C285.16875 161.08125 285.16875 138.91875 269.5125 134.30625L263.53125 132.54375A35.11875 35.11875 0 0 1 242.6249999999999 82.05L245.6249999999999 76.59375C253.4062499999999 62.26875 237.7499999999999 46.59375 223.4062499999999 54.39375L217.9499999999999 57.3750000000001A35.11875 35.11875 0 0 1 167.4562499999999 36.45L165.6937499999999 30.4875000000001C161.0812499999999 14.8312500000001 138.9187499999999 14.8312500000001 134.30625 30.4875000000001L132.5437499999999 36.4687500000001A35.11875 35.11875 0 0 1 82.0687499999999 57.3750000000001L76.5937499999999 54.3750000000001C62.2687499999999 46.5937500000001 46.5937499999999 62.2500000000001 54.3937499999999 76.5937500000001L57.3749999999999 82.0500000000001A35.11875 35.11875 0 0 1 36.46875 132.5625L30.4875 134.325C14.83125 138.9375 14.83125 161.1 30.4875 165.7125L36.46875 167.475A35.11875 35.11875 0 0 1 57.375 217.93125L54.375 223.40625C46.59375 237.73125 62.25 253.40625 76.59375 245.60625L82.06875 242.625A35.11875 35.11875 0 0 1 132.54375 263.53125L134.30625 269.5125z",
            }
        };

        SelectedView = ViewCollection[0];
    }
}
