using ArnoldVinkCode;
using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
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

                cb_SettingsLaunchScreenCaptureTool.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "LaunchScreenCaptureTool", cb_SettingsLaunchScreenCaptureTool.IsChecked.ToString());
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

                cb_SettingsShowLibraryIndieGala.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryIndieGala", cb_SettingsShowLibraryIndieGala.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.IndieGala;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryItchIO.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryItchIO", cb_SettingsShowLibraryItchIO.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.ItchIO;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                };

                cb_SettingsShowLibraryHumble.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationCtrlUI, "ShowLibraryHumble", cb_SettingsShowLibraryHumble.IsChecked.ToString());
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == AppLauncher.Humble;
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

                cb_SettingsInterfaceSound.Click += (sender, e) => { SettingSave(vConfigurationCtrlUI, "InterfaceSound", cb_SettingsInterfaceSound.IsChecked.ToString()); };

                cb_SettingsWindowsStartup.Click += (sender, e) =>
                {
                    AVSettings.StartupShortcutManage("Launcher.exe", false);
                };

                slider_SettingsAppFontSize.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsAppFontSize.Text = "Adjust the application font size: " + Convert.ToInt32(slider_SettingsAppFontSize.Value);
                    SettingSave(vConfigurationCtrlUI, "AppFontSize", slider_SettingsAppFontSize.Value);
                    AdjustApplicationFontSize();
                };

                slider_SettingsAppImageSize.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsAppImageSize.Text = "Adjust the application image size: " + Convert.ToInt32(slider_SettingsAppImageSize.Value);
                    SettingSave(vConfigurationCtrlUI, "AppImageSize", slider_SettingsAppImageSize.Value);
                    AdjustApplicationImageSize();
                };

                slider_SettingsAppWindowSize.ValueChanged += async (sender, e) =>
                {
                    textblock_SettingsAppWindowSize.Text = textblock_SettingsAppWindowSize.Tag + ": " + slider_SettingsAppWindowSize.Value.ToString() + "%";
                    SettingSave(vConfigurationCtrlUI, "AppWindowSize", slider_SettingsAppWindowSize.Value);
                    WindowUpdateStyle(vInteropWindowHandle, true, false, false, false);
                    await UpdateWindowPosition(false, true);
                };

                slider_SettingsDisplayMonitor.ValueChanged += async (sender, e) =>
                {
                    textblock_SettingsDisplayMonitor.Text = "Monitor to display the applications on: " + Convert.ToInt32(slider_SettingsDisplayMonitor.Value);
                    SettingSave(vConfigurationCtrlUI, "DisplayMonitor", slider_SettingsDisplayMonitor.Value);
                    WindowUpdateStyle(vInteropWindowHandle, true, false, false, false);
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
                    SettingSave(vConfigurationCtrlUI, "AdjustChromiumDpi", slider_SettingsAdjustChromiumDpi.Value);
                };

                slider_SettingsSoundVolume.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsSoundVolume.Text = "User interface sound volume: " + Convert.ToInt32(slider_SettingsSoundVolume.Value) + "%";
                    SettingSave(vConfigurationCtrlUI, "InterfaceSoundVolume", slider_SettingsSoundVolume.Value);
                };

                slider_SettingsGalleryLoadDays.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsGalleryLoadDays.Text = "Limit gallery loading days: " + Convert.ToInt32(slider_SettingsGalleryLoadDays.Value);
                    SettingSave(vConfigurationCtrlUI, "GalleryLoadDays", slider_SettingsGalleryLoadDays.Value);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}