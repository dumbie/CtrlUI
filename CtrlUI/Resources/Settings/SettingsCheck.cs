using System;
using System.Diagnostics;
using System.Linq;
using static ArnoldVinkCode.AVArrayFunctions;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check - Application Settings
        void Settings_Check()
        {
            try
            {
                //Server settings
                if (!SettingCheck(vConfigurationCtrlUI, "ServerPort")) { SettingSave(vConfigurationCtrlUI, "ServerPort", "26759"); }

                if (!SettingCheck(vConfigurationCtrlUI, "AppFirstLaunch")) { SettingSave(vConfigurationCtrlUI, "AppFirstLaunch", "True"); }
                if (!SettingCheck(vConfigurationCtrlUI, "AppFontSize")) { SettingSave(vConfigurationCtrlUI, "AppFontSize", "0"); }
                if (!SettingCheck(vConfigurationCtrlUI, "AppImageSize")) { SettingSave(vConfigurationCtrlUI, "AppImageSize", "10"); }
                if (!SettingCheck(vConfigurationCtrlUI, "AppWindowSize")) { SettingSave(vConfigurationCtrlUI, "AppWindowSize", "70"); }
                if (!SettingCheck(vConfigurationCtrlUI, "ApiIGDBUpdate")) { SettingSave(vConfigurationCtrlUI, "ApiIGDBUpdate", "01/01/1970 00:00:00"); }

                if (!SettingCheck(vConfigurationCtrlUI, "DisplayMonitor")) { SettingSave(vConfigurationCtrlUI, "DisplayMonitor", "1"); }
                if (!SettingCheck(vConfigurationCtrlUI, "MonitorPreventSleep")) { SettingSave(vConfigurationCtrlUI, "MonitorPreventSleep", "True"); }
                if (!SettingCheck(vConfigurationCtrlUI, "AdjustChromiumDpi")) { SettingSave(vConfigurationCtrlUI, "AdjustChromiumDpi", "0,50"); }
                if (!SettingCheck(vConfigurationCtrlUI, "ColorAccentLight")) { SettingSave(vConfigurationCtrlUI, "ColorAccentLight", "#1E90FF"); }
                if (!SettingCheck(vConfigurationCtrlUI, "LaunchMinimized")) { SettingSave(vConfigurationCtrlUI, "LaunchMinimized", "False"); }

                if (!SettingCheck(vConfigurationCtrlUI, "GalleryLoadDays")) { SettingSave(vConfigurationCtrlUI, "GalleryLoadDays", "186"); }

                if (!SettingCheck(vConfigurationCtrlUI, "HideBatteryLevel")) { SettingSave(vConfigurationCtrlUI, "HideBatteryLevel", "False"); }
                if (!SettingCheck(vConfigurationCtrlUI, "HideControllerHelp")) { SettingSave(vConfigurationCtrlUI, "HideControllerHelp", "False"); }
                if (!SettingCheck(vConfigurationCtrlUI, "ShowHiddenFilesFolders")) { SettingSave(vConfigurationCtrlUI, "ShowHiddenFilesFolders", "False"); }
                if (!SettingCheck(vConfigurationCtrlUI, "HideNetworkDrives")) { SettingSave(vConfigurationCtrlUI, "HideNetworkDrives", "False"); }

                if (!SettingCheck(vConfigurationCtrlUI, "InterfaceSound")) { SettingSave(vConfigurationCtrlUI, "InterfaceSound", "True"); }
                if (!SettingCheck(vConfigurationCtrlUI, "InterfaceSoundVolume")) { SettingSave(vConfigurationCtrlUI, "InterfaceSoundVolume", "75"); }
                if (!SettingCheck(vConfigurationCtrlUI, "InterfaceSoundPackName")) { SettingSave(vConfigurationCtrlUI, "InterfaceSoundPackName", "ArcticZephyr"); }
                if (!SettingCheck(vConfigurationCtrlUI, "InterfaceClockStyleName")) { SettingSave(vConfigurationCtrlUI, "InterfaceClockStyleName", "Cortana"); }
                if (!SettingCheck(vConfigurationCtrlUI, "InterfaceFontStyleName")) { SettingSave(vConfigurationCtrlUI, "InterfaceFontStyleName", "Segoe UI"); }

                //Startup settings
                if (!SettingCheck(vConfigurationCtrlUI, "LaunchFpsOverlayer")) { SettingSave(vConfigurationCtrlUI, "LaunchFpsOverlayer", "False"); }
                if (!SettingCheck(vConfigurationCtrlUI, "LaunchDirectXInput")) { SettingSave(vConfigurationCtrlUI, "LaunchDirectXInput", "True"); }
                if (!SettingCheck(vConfigurationCtrlUI, "LaunchScreenCaptureTool")) { SettingSave(vConfigurationCtrlUI, "LaunchScreenCaptureTool", "False"); }

                //Launcher settings
                var appLauncherArray = EnumToEnumArray<AppLauncher>().Where(x => x != AppLauncher.Unknown);
                foreach (AppLauncher appLauncher in appLauncherArray)
                {
                    try
                    {
                        string settingName = "ShowLibrary" + appLauncher.ToString();
                        if (!SettingCheck(vConfigurationCtrlUI, settingName)) { SettingSave(vConfigurationCtrlUI, settingName, "True"); }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}