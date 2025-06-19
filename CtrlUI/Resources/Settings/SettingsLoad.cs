using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVSettings;
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
                cb_SettingsLaunchMinimized.IsChecked = SettingLoad(vConfigurationCtrlUI, "LaunchMinimized", typeof(bool));

                cb_SettingsShowLibrarySteam.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibrarySteam", typeof(bool));
                cb_SettingsShowLibraryEADesktop.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryEADesktop", typeof(bool));
                cb_SettingsShowLibraryEpic.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryEpic", typeof(bool));
                cb_SettingsShowLibraryUbisoft.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryUbisoft", typeof(bool));
                cb_SettingsShowLibraryGoG.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryGoG", typeof(bool));
                cb_SettingsShowLibraryBattleNet.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryBattleNet", typeof(bool));
                cb_SettingsShowLibraryRockstar.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryRockstar", typeof(bool));
                cb_SettingsShowLibraryAmazon.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryAmazon", typeof(bool));
                cb_SettingsShowLibraryUwp.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryUwp", typeof(bool));
                cb_SettingsShowLibraryIndieGala.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryIndieGala", typeof(bool));
                cb_SettingsShowLibraryItchIO.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryItchIO", typeof(bool));
                cb_SettingsShowLibraryHumble.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowLibraryHumble", typeof(bool));

                cb_SettingsHideBatteryLevel.IsChecked = SettingLoad(vConfigurationCtrlUI, "HideBatteryLevel", typeof(bool));
                cb_SettingsHideControllerHelp.IsChecked = SettingLoad(vConfigurationCtrlUI, "HideControllerHelp", typeof(bool));

                cb_SettingsShowHiddenFilesFolders.IsChecked = SettingLoad(vConfigurationCtrlUI, "ShowHiddenFilesFolders", typeof(bool));
                cb_SettingsHideNetworkDrives.IsChecked = SettingLoad(vConfigurationCtrlUI, "HideNetworkDrives", typeof(bool));

                //Load launch settings
                cb_SettingsLaunchFpsOverlayer.IsChecked = SettingLoad(vConfigurationCtrlUI, "LaunchFpsOverlayer", typeof(bool));
                cb_SettingsLaunchDirectXInput.IsChecked = SettingLoad(vConfigurationCtrlUI, "LaunchDirectXInput", typeof(bool));
                cb_SettingsLaunchScreenCaptureTool.IsChecked = SettingLoad(vConfigurationCtrlUI, "LaunchScreenCaptureTool", typeof(bool));

                //Load the application font size
                textblock_SettingsAppFontSize.Text = "Adjust the application font size: " + SettingLoad(vConfigurationCtrlUI, "AppFontSize", typeof(string));
                slider_SettingsAppFontSize.Value = SettingLoad(vConfigurationCtrlUI, "AppFontSize", typeof(double));

                //Load the application image size
                textblock_SettingsAppImageSize.Text = "Adjust the application image size: " + SettingLoad(vConfigurationCtrlUI, "AppImageSize", typeof(string));
                slider_SettingsAppImageSize.Value = SettingLoad(vConfigurationCtrlUI, "AppImageSize", typeof(double));

                //Load the application window size
                textblock_SettingsAppWindowSize.Text = textblock_SettingsAppWindowSize.Tag + ": " + SettingLoad(vConfigurationCtrlUI, "AppWindowSize", typeof(string)) + "%";
                slider_SettingsAppWindowSize.Value = SettingLoad(vConfigurationCtrlUI, "AppWindowSize", typeof(double));

                //Load the display monitor
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));
                textblock_SettingsDisplayMonitor.Text = textblock_SettingsDisplayMonitor.Tag + ": " + monitorNumber;
                slider_SettingsDisplayMonitor.Value = monitorNumber;

                //Load display settings
                cb_SettingsMonitorPreventSleep.IsChecked = SettingLoad(vConfigurationCtrlUI, "MonitorPreventSleep", typeof(bool));

                textblock_SettingsAdjustChromiumDpi.Text = textblock_SettingsAdjustChromiumDpi.Tag + ": +" + SettingLoad(vConfigurationCtrlUI, "AdjustChromiumDpi", typeof(string)) + "%";
                slider_SettingsAdjustChromiumDpi.Value = SettingLoad(vConfigurationCtrlUI, "AdjustChromiumDpi", typeof(double));

                //Load sound volume
                cb_SettingsInterfaceSound.IsChecked = SettingLoad(vConfigurationCtrlUI, "InterfaceSound", typeof(bool));
                textblock_SettingsSoundVolume.Text = "User interface sound volume: " + SettingLoad(vConfigurationCtrlUI, "InterfaceSoundVolume", typeof(string)) + "%";
                slider_SettingsSoundVolume.Value = SettingLoad(vConfigurationCtrlUI, "InterfaceSoundVolume", typeof(double));

                //Load gallery days
                textblock_SettingsGalleryLoadDays.Text = "Limit gallery loading days: " + SettingLoad(vConfigurationCtrlUI, "GalleryLoadDays", typeof(string));
                slider_SettingsGalleryLoadDays.Value = SettingLoad(vConfigurationCtrlUI, "GalleryLoadDays", typeof(double));

                //Launch settings
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