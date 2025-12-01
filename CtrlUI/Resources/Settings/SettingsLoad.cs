using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Load - Application Settings
        async Task Settings_Load()
        {
            try
            {
                cb_SettingsLaunchMinimized.IsChecked = vSettings.Load("LaunchMinimized", typeof(bool));
                cb_SettingsHideBatteryLevel.IsChecked = vSettings.Load("HideBatteryLevel", typeof(bool));
                cb_SettingsHideControllerHelp.IsChecked = vSettings.Load("HideControllerHelp", typeof(bool));

                cb_SettingsShowHiddenFilesFolders.IsChecked = vSettings.Load("ShowHiddenFilesFolders", typeof(bool));
                cb_SettingsHideNetworkDrives.IsChecked = vSettings.Load("HideNetworkDrives", typeof(bool));

                //Load launch settings
                cb_SettingsLaunchFpsOverlayer.IsChecked = vSettings.Load("LaunchFpsOverlayer", typeof(bool));
                cb_SettingsLaunchDirectXInput.IsChecked = vSettings.Load("LaunchDirectXInput", typeof(bool));
                cb_SettingsLaunchScreenCapy.IsChecked = vSettings.Load("LaunchScreenCapy", typeof(bool));

                //Load application font size
                textblock_SettingsAppFontSize.Text = "Adjust the application font size: " + vSettings.Load("AppFontSize", typeof(string));
                slider_SettingsAppFontSize.Value = vSettings.Load("AppFontSize", typeof(double));

                //Load application image size
                textblock_SettingsAppImageSize.Text = "Adjust the application image size: " + vSettings.Load("AppImageSize", typeof(string));
                slider_SettingsAppImageSize.Value = vSettings.Load("AppImageSize", typeof(double));

                //Load application window size
                textblock_SettingsAppWindowSize.Text = textblock_SettingsAppWindowSize.Tag + ": " + vSettings.Load("AppWindowSize", typeof(string)) + "%";
                slider_SettingsAppWindowSize.Value = vSettings.Load("AppWindowSize", typeof(double));

                //Load display monitor
                int monitorNumber = vSettings.Load("DisplayMonitor", typeof(int));
                textblock_SettingsDisplayMonitor.Text = textblock_SettingsDisplayMonitor.Tag + ": " + monitorNumber;
                slider_SettingsDisplayMonitor.Value = monitorNumber;

                //Load display settings
                cb_SettingsMonitorPreventSleep.IsChecked = vSettings.Load("MonitorPreventSleep", typeof(bool));

                textblock_SettingsAdjustChromiumDpi.Text = textblock_SettingsAdjustChromiumDpi.Tag + ": +" + vSettings.Load("AdjustChromiumDpi", typeof(string)) + "%";
                slider_SettingsAdjustChromiumDpi.Value = vSettings.Load("AdjustChromiumDpi", typeof(double));

                //Load sound volume
                cb_SettingsInterfaceSound.IsChecked = vSettings.Load("InterfaceSound", typeof(bool));
                textblock_SettingsSoundVolume.Text = "User interface sound volume: " + vSettings.Load("InterfaceSoundVolume", typeof(string)) + "%";
                slider_SettingsSoundVolume.Value = vSettings.Load("InterfaceSoundVolume", typeof(double));

                //Load gallery days
                textblock_SettingsGalleryLoadDays.Text = "Limit gallery loading days: " + vSettings.Load("GalleryLoadDays", typeof(string));
                slider_SettingsGalleryLoadDays.Value = vSettings.Load("GalleryLoadDays", typeof(double));

                //Startup settings
                cb_SettingsWindowsStartup.IsChecked = AVSettings.StartupShortcutCheck();

                //Wait for settings to have loaded
                await Task.Delay(1500);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load application settings: " + ex.Message);
            }
        }
    }
}