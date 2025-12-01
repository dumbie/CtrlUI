using System;
using System.Diagnostics;
using System.Linq;
using static ArnoldVinkCode.AVArrayFunctions;
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
                if (!vSettings.Check("ServerPort")) { vSettings.Set("ServerPort", "26759"); }

                if (!vSettings.Check("AppFirstLaunch")) { vSettings.Set("AppFirstLaunch", "True"); }
                if (!vSettings.Check("AppFontSize")) { vSettings.Set("AppFontSize", "0"); }
                if (!vSettings.Check("AppImageSize")) { vSettings.Set("AppImageSize", "10"); }
                if (!vSettings.Check("AppWindowSize")) { vSettings.Set("AppWindowSize", "70"); }
                if (!vSettings.Check("ApiIGDBUpdate")) { vSettings.Set("ApiIGDBUpdate", "01/01/1970 00:00:00"); }

                if (!vSettings.Check("DisplayMonitor")) { vSettings.Set("DisplayMonitor", "1"); }
                if (!vSettings.Check("MonitorPreventSleep")) { vSettings.Set("MonitorPreventSleep", "True"); }
                if (!vSettings.Check("AdjustChromiumDpi")) { vSettings.Set("AdjustChromiumDpi", "0,50"); }
                if (!vSettings.Check("ColorAccentLight")) { vSettings.Set("ColorAccentLight", "#1E90FF"); }
                if (!vSettings.Check("LaunchMinimized")) { vSettings.Set("LaunchMinimized", "False"); }

                if (!vSettings.Check("GalleryLoadDays")) { vSettings.Set("GalleryLoadDays", "186"); }

                if (!vSettings.Check("HideBatteryLevel")) { vSettings.Set("HideBatteryLevel", "False"); }
                if (!vSettings.Check("HideControllerHelp")) { vSettings.Set("HideControllerHelp", "False"); }
                if (!vSettings.Check("ShowHiddenFilesFolders")) { vSettings.Set("ShowHiddenFilesFolders", "False"); }
                if (!vSettings.Check("HideNetworkDrives")) { vSettings.Set("HideNetworkDrives", "False"); }

                if (!vSettings.Check("InterfaceSound")) { vSettings.Set("InterfaceSound", "True"); }
                if (!vSettings.Check("InterfaceSoundVolume")) { vSettings.Set("InterfaceSoundVolume", "75"); }
                if (!vSettings.Check("InterfaceSoundPackName")) { vSettings.Set("InterfaceSoundPackName", "ArcticZephyr"); }
                if (!vSettings.Check("InterfaceClockStyleName")) { vSettings.Set("InterfaceClockStyleName", "Cortana"); }
                if (!vSettings.Check("InterfaceFontStyleName")) { vSettings.Set("InterfaceFontStyleName", "Segoe UI"); }

                //Startup settings
                if (!vSettings.Check("LaunchFpsOverlayer")) { vSettings.Set("LaunchFpsOverlayer", "False"); }
                if (!vSettings.Check("LaunchDirectXInput")) { vSettings.Set("LaunchDirectXInput", "True"); }
                if (!vSettings.Check("LaunchScreenCapy")) { vSettings.Set("LaunchScreenCapy", "False"); }

                //Launcher settings
                var appLauncherArray = EnumToEnumArray<AppLauncher>().Where(x => x != AppLauncher.Unknown);
                foreach (AppLauncher appLauncher in appLauncherArray)
                {
                    try
                    {
                        string settingName = "ShowLibrary" + appLauncher.ToString();
                        if (!vSettings.Check(settingName)) { vSettings.Set(settingName, "True"); }
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