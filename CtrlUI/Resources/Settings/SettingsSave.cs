using ArnoldVinkCode;
using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVWindowFunctions;
using static CtrlUI.AppVariables;

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
                    vSettings.Set("LaunchMinimized", cb_SettingsLaunchMinimized.IsChecked.ToString());
                };

                cb_SettingsLaunchFpsOverlayer.Click += (sender, e) =>
                {
                    vSettings.Set("LaunchFpsOverlayer", cb_SettingsLaunchFpsOverlayer.IsChecked.ToString());
                };

                cb_SettingsLaunchDirectXInput.Click += (sender, e) =>
                {
                    vSettings.Set("LaunchDirectXInput", cb_SettingsLaunchDirectXInput.IsChecked.ToString());
                };

                cb_SettingsLaunchScreenCapy.Click += (sender, e) =>
                {
                    vSettings.Set("LaunchScreenCapy", cb_SettingsLaunchScreenCapy.IsChecked.ToString());
                };

                cb_SettingsHideBatteryLevel.Click += (sender, e) =>
                {
                    vSettings.Set("HideBatteryLevel", cb_SettingsHideBatteryLevel.IsChecked.ToString());
                    if ((bool)cb_SettingsHideBatteryLevel.IsChecked)
                    {
                        HideBatteryStatus(true);
                    }
                };

                cb_SettingsHideControllerHelp.Click += (sender, e) =>
                {
                    vSettings.Set("HideControllerHelp", cb_SettingsHideControllerHelp.IsChecked.ToString());
                    UpdateControllerHelp();
                };

                cb_SettingsShowHiddenFilesFolders.Click += (sender, e) => { vSettings.Set("ShowHiddenFilesFolders", cb_SettingsShowHiddenFilesFolders.IsChecked.ToString()); };
                cb_SettingsHideNetworkDrives.Click += (sender, e) => { vSettings.Set("HideNetworkDrives", cb_SettingsHideNetworkDrives.IsChecked.ToString()); };

                cb_SettingsInterfaceSound.Click += (sender, e) => { vSettings.Set("InterfaceSound", cb_SettingsInterfaceSound.IsChecked.ToString()); };

                cb_SettingsWindowsStartup.Click += (sender, e) =>
                {
                    AVSettings.StartupShortcutManage("Launcher.exe", false);
                };

                slider_SettingsAppFontSize.ValueChangedDelay += (sender, e) =>
                {
                    textblock_SettingsAppFontSize.Text = "Adjust the application font size: " + Convert.ToInt32(slider_SettingsAppFontSize.Value);
                    vSettings.Set("AppFontSize", slider_SettingsAppFontSize.Value);
                    AdjustApplicationFontSize();
                };

                slider_SettingsAppImageSize.ValueChangedDelay += (sender, e) =>
                {
                    textblock_SettingsAppImageSize.Text = "Adjust the application image size: " + Convert.ToInt32(slider_SettingsAppImageSize.Value);
                    vSettings.Set("AppImageSize", slider_SettingsAppImageSize.Value);
                    AdjustApplicationImageSize();
                };

                slider_SettingsAppWindowSize.ValueChangedDelay += async (sender, e) =>
                {
                    textblock_SettingsAppWindowSize.Text = textblock_SettingsAppWindowSize.Tag + ": " + slider_SettingsAppWindowSize.Value.ToString() + "%";
                    vSettings.Set("AppWindowSize", slider_SettingsAppWindowSize.Value);
                    WindowUpdateStyle(vWindowMain.GetHandle(), true, false, false, false);
                    await UpdateWindowPosition(true);
                };

                slider_SettingsDisplayMonitor.ValueChangedDelay += async (sender, e) =>
                {
                    textblock_SettingsDisplayMonitor.Text = textblock_SettingsDisplayMonitor.Tag + ": " + Convert.ToInt32(slider_SettingsDisplayMonitor.Value);
                    vSettings.Set("DisplayMonitor", slider_SettingsDisplayMonitor.Value);
                    WindowUpdateStyle(vWindowMain.GetHandle(), true, false, false, false);
                    await UpdateWindowPosition(false);
                };

                cb_SettingsMonitorPreventSleep.Click += (sender, e) =>
                {
                    vSettings.Set("MonitorPreventSleep", cb_SettingsMonitorPreventSleep.IsChecked.ToString());
                    //Prevent or allow monitor sleep
                    UpdateMonitorSleepAuto();
                };

                slider_SettingsAdjustChromiumDpi.ValueChangedDelay += (sender, e) =>
                {
                    textblock_SettingsAdjustChromiumDpi.Text = textblock_SettingsAdjustChromiumDpi.Tag + ": +" + slider_SettingsAdjustChromiumDpi.Value.ToString("0.00") + "%";
                    vSettings.Set("AdjustChromiumDpi", slider_SettingsAdjustChromiumDpi.Value);
                };

                slider_SettingsSoundVolume.ValueChangedDelay += (sender, e) =>
                {
                    textblock_SettingsSoundVolume.Text = "User interface sound volume: " + Convert.ToInt32(slider_SettingsSoundVolume.Value) + "%";
                    vSettings.Set("InterfaceSoundVolume", slider_SettingsSoundVolume.Value);
                };

                slider_SettingsGalleryLoadDays.ValueChangedDelay += (sender, e) =>
                {
                    textblock_SettingsGalleryLoadDays.Text = "Limit gallery loading days: " + Convert.ToInt32(slider_SettingsGalleryLoadDays.Value);
                    vSettings.Set("GalleryLoadDays", slider_SettingsGalleryLoadDays.Value);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}