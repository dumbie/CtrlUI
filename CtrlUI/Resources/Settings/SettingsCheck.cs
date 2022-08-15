using System;
using System.Diagnostics;
using static CtrlUI.AppVariables;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check - Application Settings
        void Settings_Check()
        {
            try
            {
                if (Setting_Load(vConfigurationCtrlUI, "AppFirstLaunch") == null) { Setting_Save(vConfigurationCtrlUI, "AppFirstLaunch", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "AppFontSize") == null) { Setting_Save(vConfigurationCtrlUI, "AppFontSize", "0"); }
                if (Setting_Load(vConfigurationCtrlUI, "ApiIGDBUpdate") == null) { Setting_Save(vConfigurationCtrlUI, "ApiIGDBUpdate", "01/01/1970 00:00:00"); }                    

                if (Setting_Load(vConfigurationCtrlUI, "DisplayMonitor") == null) { Setting_Save(vConfigurationCtrlUI, "DisplayMonitor", "1"); } //Shared
                if (Setting_Load(vConfigurationCtrlUI, "MonitorPreventSleep") == null) { Setting_Save(vConfigurationCtrlUI, "MonitorPreventSleep", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "AdjustChromiumDpi") == null) { Setting_Save(vConfigurationCtrlUI, "AdjustChromiumDpi", "0,50"); }
                if (Setting_Load(vConfigurationCtrlUI, "ColorAccentLight") == null) { Setting_Save(vConfigurationCtrlUI, "ColorAccentLight", "#1E90FF"); } //Shared
                if (Setting_Load(vConfigurationCtrlUI, "ServerPort") == null) { Setting_Save(vConfigurationCtrlUI, "ServerPort", "26759"); } //Shared

                if (Setting_Load(vConfigurationCtrlUI, "LaunchFullscreen") == null) { Setting_Save(vConfigurationCtrlUI, "LaunchFullscreen", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "LaunchMinimized") == null) { Setting_Save(vConfigurationCtrlUI, "LaunchMinimized", "False"); }

                if (Setting_Load(vConfigurationCtrlUI, "ShowOtherShortcuts") == null) { Setting_Save(vConfigurationCtrlUI, "ShowOtherShortcuts", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowOtherProcesses") == null) { Setting_Save(vConfigurationCtrlUI, "ShowOtherProcesses", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "HideAppProcesses") == null) { Setting_Save(vConfigurationCtrlUI, "HideAppProcesses", "False"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowLibrarySteam") == null) { Setting_Save(vConfigurationCtrlUI, "ShowLibrarySteam", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowLibraryEADesktop") == null) { Setting_Save(vConfigurationCtrlUI, "ShowLibraryEADesktop", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowLibraryEpic") == null) { Setting_Save(vConfigurationCtrlUI, "ShowLibraryEpic", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowLibraryUbisoft") == null) { Setting_Save(vConfigurationCtrlUI, "ShowLibraryUbisoft", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowLibraryGoG") == null) { Setting_Save(vConfigurationCtrlUI, "ShowLibraryGoG", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowLibraryBattleNet") == null) { Setting_Save(vConfigurationCtrlUI, "ShowLibraryBattleNet", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowLibraryRockstar") == null) { Setting_Save(vConfigurationCtrlUI, "ShowLibraryRockstar", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowLibraryAmazon") == null) { Setting_Save(vConfigurationCtrlUI, "ShowLibraryAmazon", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowLibraryUwp") == null) { Setting_Save(vConfigurationCtrlUI, "ShowLibraryUwp", "True"); }

                if (Setting_Load(vConfigurationCtrlUI, "HideBatteryLevel") == null) { Setting_Save(vConfigurationCtrlUI, "HideBatteryLevel", "False"); }
                if (Setting_Load(vConfigurationCtrlUI, "HideMouseCursor") == null) { Setting_Save(vConfigurationCtrlUI, "HideMouseCursor", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "HideControllerHelp") == null) { Setting_Save(vConfigurationCtrlUI, "HideControllerHelp", "False"); }
                if (Setting_Load(vConfigurationCtrlUI, "ShowHiddenFilesFolders") == null) { Setting_Save(vConfigurationCtrlUI, "ShowHiddenFilesFolders", "False"); }
                if (Setting_Load(vConfigurationCtrlUI, "HideNetworkDrives") == null) { Setting_Save(vConfigurationCtrlUI, "HideNetworkDrives", "False"); }
                if (Setting_Load(vConfigurationCtrlUI, "NotReadyNetworkDrives") == null) { Setting_Save(vConfigurationCtrlUI, "NotReadyNetworkDrives", "True"); }

                if (Setting_Load(vConfigurationCtrlUI, "InterfaceSound") == null) { Setting_Save(vConfigurationCtrlUI, "InterfaceSound", "True"); } //Shared
                if (Setting_Load(vConfigurationCtrlUI, "InterfaceSoundVolume") == null) { Setting_Save(vConfigurationCtrlUI, "InterfaceSoundVolume", "75"); } //Shared
                if (Setting_Load(vConfigurationCtrlUI, "InterfaceSoundPackName") == null) { Setting_Save(vConfigurationCtrlUI, "InterfaceSoundPackName", "ArcticZephyr"); } //Shared
                if (Setting_Load(vConfigurationCtrlUI, "InterfaceClockStyleName") == null) { Setting_Save(vConfigurationCtrlUI, "InterfaceClockStyleName", "Cortana"); } //Shared
                if (Setting_Load(vConfigurationCtrlUI, "InterfaceFontStyleName") == null) { Setting_Save(vConfigurationCtrlUI, "InterfaceFontStyleName", "Segoe UI"); }

                if (Setting_Load(vConfigurationCtrlUI, "ShowMediaMain") == null) { Setting_Save(vConfigurationCtrlUI, "ShowMediaMain", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "MinimizeAppOnShow") == null) { Setting_Save(vConfigurationCtrlUI, "MinimizeAppOnShow", "False"); }
                if (Setting_Load(vConfigurationCtrlUI, "LaunchFpsOverlayer") == null) { Setting_Save(vConfigurationCtrlUI, "LaunchFpsOverlayer", "False"); }
                if (Setting_Load(vConfigurationCtrlUI, "LaunchDirectXInput") == null) { Setting_Save(vConfigurationCtrlUI, "LaunchDirectXInput", "True"); }

                //Background settings
                if (Setting_Load(vConfigurationCtrlUI, "VideoBackground") == null) { Setting_Save(vConfigurationCtrlUI, "VideoBackground", "True"); }
                if (Setting_Load(vConfigurationCtrlUI, "DesktopBackground") == null) { Setting_Save(vConfigurationCtrlUI, "DesktopBackground", "False"); }
                if (Setting_Load(vConfigurationCtrlUI, "BackgroundBrightness") == null) { Setting_Save(vConfigurationCtrlUI, "BackgroundBrightness", "80"); }
                if (Setting_Load(vConfigurationCtrlUI, "BackgroundPlayVolume") == null) { Setting_Save(vConfigurationCtrlUI, "BackgroundPlayVolume", "30"); }
                if (Setting_Load(vConfigurationCtrlUI, "BackgroundPlaySpeed") == null) { Setting_Save(vConfigurationCtrlUI, "BackgroundPlaySpeed", "100"); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}