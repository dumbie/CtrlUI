using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Media;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

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
                    Setting_Save(vConfigurationCtrlUI, "LaunchMinimized", cb_SettingsLaunchMinimized.IsChecked.ToString());
                };

                cb_SettingsLaunchFpsOverlayer.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "LaunchFpsOverlayer", cb_SettingsLaunchFpsOverlayer.IsChecked.ToString());
                };

                cb_SettingsLaunchDirectXInput.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "LaunchDirectXInput", cb_SettingsLaunchDirectXInput.IsChecked.ToString());
                };

                cb_SettingsHideAppProcesses.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "HideAppProcesses", cb_SettingsHideAppProcesses.IsChecked.ToString());
                };

                cb_SettingsShowLibrarySteam.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "ShowLibrarySteam", cb_SettingsShowLibrarySteam.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Steam;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryEADesktop.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "ShowLibraryEADesktop", cb_SettingsShowLibraryEADesktop.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.EADesktop;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryEpic.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "ShowLibraryEpic", cb_SettingsShowLibraryEpic.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Epic;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryUbisoft.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "ShowLibraryUbisoft", cb_SettingsShowLibraryUbisoft.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Ubisoft;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryGoG.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "ShowLibraryGoG", cb_SettingsShowLibraryGoG.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.GoG;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryBattleNet.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "ShowLibraryBattleNet", cb_SettingsShowLibraryBattleNet.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.BattleNet;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryRockstar.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "ShowLibraryRockstar", cb_SettingsShowLibraryRockstar.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Rockstar;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryAmazon.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "ShowLibraryAmazon", cb_SettingsShowLibraryAmazon.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Amazon;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryUwp.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "ShowLibraryUwp", cb_SettingsShowLibraryUwp.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.UWP;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsHideBatteryLevel.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "HideBatteryLevel", cb_SettingsHideBatteryLevel.IsChecked.ToString());
                    if ((bool)cb_SettingsHideBatteryLevel.IsChecked)
                    {
                        HideBatteryStatus(true);
                    }
                };

                cb_SettingsHideControllerHelp.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "HideControllerHelp", cb_SettingsHideControllerHelp.IsChecked.ToString());
                    UpdateControllerHelp();
                };

                cb_SettingsShowHiddenFilesFolders.Click += (sender, e) => { Setting_Save(vConfigurationCtrlUI, "ShowHiddenFilesFolders", cb_SettingsShowHiddenFilesFolders.IsChecked.ToString()); };
                cb_SettingsHideNetworkDrives.Click += (sender, e) => { Setting_Save(vConfigurationCtrlUI, "HideNetworkDrives", cb_SettingsHideNetworkDrives.IsChecked.ToString()); };
                cb_SettingsNotReadyNetworkDrives.Click += (sender, e) => { Setting_Save(vConfigurationCtrlUI, "NotReadyNetworkDrives", cb_SettingsNotReadyNetworkDrives.IsChecked.ToString()); };

                cb_SettingsInterfaceSound.Click += (sender, e) => { Setting_Save(vConfigurationCtrlUI, "InterfaceSound", cb_SettingsInterfaceSound.IsChecked.ToString()); };

                cb_SettingsWindowsStartup.Click += (sender, e) =>
                {
                    AVSettings.ManageStartupShortcut("CtrlUI-Launcher.exe");
                };

                slider_SettingsFontSize.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsFontSize.Text = "Adjust the application font size: " + Convert.ToInt32(slider_SettingsFontSize.Value);
                    Setting_Save(vConfigurationCtrlUI, "AppFontSize", Convert.ToInt32(slider_SettingsFontSize.Value).ToString());
                    AdjustApplicationFontSize();
                };

                slider_SettingsAppWindowSize.ValueChanged += async (sender, e) =>
                {
                    textblock_SettingsAppWindowSize.Text = textblock_SettingsAppWindowSize.Tag + ": " + slider_SettingsAppWindowSize.Value.ToString() + "%";
                    Setting_Save(vConfigurationCtrlUI, "AppWindowSize", slider_SettingsAppWindowSize.Value.ToString());
                    await UpdateWindowPosition(false, true);
                };

                slider_SettingsDisplayMonitor.ValueChanged += async (sender, e) =>
                {
                    textblock_SettingsDisplayMonitor.Text = "Monitor to display the applications on: " + Convert.ToInt32(slider_SettingsDisplayMonitor.Value);
                    Setting_Save(vConfigurationCtrlUI, "DisplayMonitor", Convert.ToInt32(slider_SettingsDisplayMonitor.Value).ToString());
                    await UpdateWindowPosition(true, false);
                };

                cb_SettingsMonitorPreventSleep.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationCtrlUI, "MonitorPreventSleep", cb_SettingsMonitorPreventSleep.IsChecked.ToString());
                    //Prevent or allow monitor sleep
                    UpdateMonitorSleepAuto();
                };

                slider_SettingsAdjustChromiumDpi.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsAdjustChromiumDpi.Text = textblock_SettingsAdjustChromiumDpi.Tag + ": +" + slider_SettingsAdjustChromiumDpi.Value.ToString("0.00") + "%";
                    Setting_Save(vConfigurationCtrlUI, "AdjustChromiumDpi", slider_SettingsAdjustChromiumDpi.Value.ToString("0.00"));
                };

                slider_SettingsSoundVolume.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsSoundVolume.Text = "User interface sound volume: " + Convert.ToInt32(slider_SettingsSoundVolume.Value) + "%";
                    Setting_Save(vConfigurationCtrlUI, "InterfaceSoundVolume", Convert.ToInt32(slider_SettingsSoundVolume.Value).ToString());
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
                    Setting_Save(vConfigurationCtrlUI, "ServerPort", txt_SettingsSocketClientPortStart.Text);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}