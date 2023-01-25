using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Save - Monitor Application Settings
        void Settings_Save()
        {
            try
            {
                cb_SettingsLaunchMinimized.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "LaunchMinimized", cb_SettingsLaunchMinimized.IsChecked.ToString());
                };

                cb_SettingsLaunchFpsOverlayer.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "LaunchFpsOverlayer", cb_SettingsLaunchFpsOverlayer.IsChecked.ToString());
                };

                cb_SettingsLaunchDirectXInput.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "LaunchDirectXInput", cb_SettingsLaunchDirectXInput.IsChecked.ToString());
                };

                cb_SettingsShowLibrarySteam.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibrarySteam", cb_SettingsShowLibrarySteam.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Steam;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryEADesktop.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryEADesktop", cb_SettingsShowLibraryEADesktop.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.EADesktop;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryEpic.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryEpic", cb_SettingsShowLibraryEpic.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Epic;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryUbisoft.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryUbisoft", cb_SettingsShowLibraryUbisoft.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Ubisoft;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryGoG.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryGoG", cb_SettingsShowLibraryGoG.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.GoG;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryBattleNet.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryBattleNet", cb_SettingsShowLibraryBattleNet.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.BattleNet;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryRockstar.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryRockstar", cb_SettingsShowLibraryRockstar.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Rockstar;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryAmazon.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryAmazon", cb_SettingsShowLibraryAmazon.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Amazon;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryUwp.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryUwp", cb_SettingsShowLibraryUwp.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.UWP;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsHideBatteryLevel.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "HideBatteryLevel", cb_SettingsHideBatteryLevel.IsChecked.ToString());
                    if ((bool)cb_SettingsHideBatteryLevel.IsChecked)
                    {
                        HideBatteryStatus(true);
                    }
                };

                cb_SettingsHideControllerHelp.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "HideControllerHelp", cb_SettingsHideControllerHelp.IsChecked.ToString());
                    UpdateControllerHelp();
                };

                cb_SettingsShowHiddenFilesFolders.Click += (sender, e) => { SettingSave(vConfigurationCtrlUI, "ShowHiddenFilesFolders", cb_SettingsShowHiddenFilesFolders.IsChecked.ToString()); };
                cb_SettingsHideNetworkDrives.Click += (sender, e) => { SettingSave(vConfigurationCtrlUI, "HideNetworkDrives", cb_SettingsHideNetworkDrives.IsChecked.ToString()); };
                cb_SettingsNotReadyNetworkDrives.Click += (sender, e) => { SettingSave(vConfigurationCtrlUI, "NotReadyNetworkDrives", cb_SettingsNotReadyNetworkDrives.IsChecked.ToString()); };

                cb_SettingsInterfaceSound.Click += (sender, e) => { SettingSave(vConfigurationCtrlUI, "InterfaceSound", cb_SettingsInterfaceSound.IsChecked.ToString()); };

                cb_SettingsWindowsStartup.Click += (sender, e) =>
                {
                    AVSettings.ManageStartupShortcut("CtrlUI-Launcher.exe");
                };

                slider_SettingsFontSize.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsFontSize.Text = "Adjust the application font size: " + Convert.ToInt32(slider_SettingsFontSize.Value);
                    SettingSave(vConfigurationCtrlUI, "AppFontSize", Convert.ToInt32(slider_SettingsFontSize.Value).ToString());
                    AdjustApplicationFontSize();
                };

                slider_SettingsAppWindowSize.ValueChanged += async (sender, e) =>
                {
                    textblock_SettingsAppWindowSize.Text = textblock_SettingsAppWindowSize.Tag + ": " + slider_SettingsAppWindowSize.Value.ToString() + "%";
                    SettingSave(vConfigurationCtrlUI, "AppWindowSize", slider_SettingsAppWindowSize.Value.ToString());
                    await UpdateWindowPosition(false, true);
                };

                slider_SettingsDisplayMonitor.ValueChanged += async (sender, e) =>
                {
                    textblock_SettingsDisplayMonitor.Text = "Monitor to display the applications on: " + Convert.ToInt32(slider_SettingsDisplayMonitor.Value);
                    SettingSave(vConfigurationCtrlUI, "DisplayMonitor", Convert.ToInt32(slider_SettingsDisplayMonitor.Value).ToString());
                    await UpdateWindowPosition(true, false);
                };

                cb_SettingsMonitorPreventSleep.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "MonitorPreventSleep", cb_SettingsMonitorPreventSleep.IsChecked.ToString());
                    //Prevent or allow monitor sleep
                    UpdateMonitorSleepAuto();
                };

                slider_SettingsAdjustChromiumDpi.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsAdjustChromiumDpi.Text = textblock_SettingsAdjustChromiumDpi.Tag + ": +" + slider_SettingsAdjustChromiumDpi.Value.ToString("0.00") + "%";
                    SettingSave(vConfigurationCtrlUI, "AdjustChromiumDpi", slider_SettingsAdjustChromiumDpi.Value.ToString("0.00"));
                };

                slider_SettingsSoundVolume.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsSoundVolume.Text = "User interface sound volume: " + Convert.ToInt32(slider_SettingsSoundVolume.Value) + "%";
                    SettingSave(vConfigurationCtrlUI, "InterfaceSoundVolume", Convert.ToInt32(slider_SettingsSoundVolume.Value).ToString());
                };

                //Save - Socket Client Port
                txt_SettingsSocketClientPortStart.TextChanged += (sender, e) =>
                {
                    //Color brushes
                    BrushConverter BrushConvert = new BrushConverter();
                    Brush BrushInvalid = BrushConvert.ConvertFromString("#CD1A2B") as Brush;
                    Brush BrushValid = BrushConvert.ConvertFromString("#1DB954") as Brush;

                    if (string.IsNullOrWhiteSpace(txt_SettingsSocketClientPortStart.Text))
                    {
                        txt_SettingsSocketClientPortStart.BorderBrush = BrushInvalid;
                        txt_SettingsSocketClientPortRange.BorderBrush = BrushInvalid;
                        return;
                    }

                    if (Regex.IsMatch(txt_SettingsSocketClientPortStart.Text, "(\\D+)"))
                    {
                        txt_SettingsSocketClientPortStart.BorderBrush = BrushInvalid;
                        txt_SettingsSocketClientPortRange.BorderBrush = BrushInvalid;
                        return;
                    }

                    int NewServerPort = Convert.ToInt32(txt_SettingsSocketClientPortStart.Text);
                    if (NewServerPort < 100 || NewServerPort > 65500)
                    {
                        txt_SettingsSocketClientPortStart.BorderBrush = BrushInvalid;
                        txt_SettingsSocketClientPortRange.BorderBrush = BrushInvalid;
                        return;
                    }

                    txt_SettingsSocketClientPortStart.BorderBrush = BrushValid;
                    txt_SettingsSocketClientPortRange.BorderBrush = BrushValid;
                    txt_SettingsSocketClientPortRange.Text = Convert.ToString(NewServerPort + 2);
                    SettingSave(vConfigurationCtrlUI, "ServerPort", txt_SettingsSocketClientPortStart.Text);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}