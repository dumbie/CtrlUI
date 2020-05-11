using System;
using System.Configuration;
using System.Diagnostics;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check - Application Settings
        void Settings_Check()
        {
            try
            {
                if (ConfigurationManager.AppSettings["AppFirstLaunch"] == null) { SettingSave("AppFirstLaunch", "True"); }
                if (ConfigurationManager.AppSettings["AppUpdateCheck"] == null) { SettingSave("AppUpdateCheck", DateTime.Now.ToString(vAppCultureInfo)); }
                if (ConfigurationManager.AppSettings["AppFontSize"] == null) { SettingSave("AppFontSize", "0"); }

                if (ConfigurationManager.AppSettings["DisplayMonitor"] == null) { SettingSave("DisplayMonitor", "1"); } //Shared
                if (ConfigurationManager.AppSettings["ColorAccentLight"] == null) { SettingSave("ColorAccentLight", "#00C7FF"); } //Shared
                if (ConfigurationManager.AppSettings["ServerPort"] == null) { SettingSave("ServerPort", "1010"); } //Shared

                if (ConfigurationManager.AppSettings["LaunchFullscreen"] == null) { SettingSave("LaunchFullscreen", "True"); }
                if (ConfigurationManager.AppSettings["LaunchMinimized"] == null) { SettingSave("LaunchMinimized", "False"); }
                if (ConfigurationManager.AppSettings["ShowOtherShortcuts"] == null) { SettingSave("ShowOtherShortcuts", "True"); }
                if (ConfigurationManager.AppSettings["ShowOtherProcesses"] == null) { SettingSave("ShowOtherProcesses", "True"); }
                if (ConfigurationManager.AppSettings["HideAppProcesses"] == null) { SettingSave("HideAppProcesses", "False"); }

                if (ConfigurationManager.AppSettings["HideBatteryLevel"] == null) { SettingSave("HideBatteryLevel", "False"); }
                if (ConfigurationManager.AppSettings["HideMouseCursor"] == null) { SettingSave("HideMouseCursor", "True"); }
                if (ConfigurationManager.AppSettings["HideControllerHelp"] == null) { SettingSave("HideControllerHelp", "False"); }
                if (ConfigurationManager.AppSettings["ShowHiddenFilesFolders"] == null) { SettingSave("ShowHiddenFilesFolders", "False"); }
                if (ConfigurationManager.AppSettings["HideNetworkDrives"] == null) { SettingSave("HideNetworkDrives", "False"); }

                if (ConfigurationManager.AppSettings["InterfaceSound"] == null) { SettingSave("InterfaceSound", "True"); }
                if (ConfigurationManager.AppSettings["InterfaceSoundVolume"] == null) { SettingSave("InterfaceSoundVolume", "70"); }
                if (ConfigurationManager.AppSettings["InterfaceSoundPackName"] == null) { SettingSave("InterfaceSoundPackName", "Default"); } //Shared
                if (ConfigurationManager.AppSettings["InterfaceClockStyleName"] == null) { SettingSave("InterfaceClockStyleName", "Cortana"); }
                if (ConfigurationManager.AppSettings["InterfaceFontStyleName"] == null) { SettingSave("InterfaceFontStyleName", "Segoe UI"); }

                if (ConfigurationManager.AppSettings["CloseMediaScreen"] == null) { SettingSave("CloseMediaScreen", "False"); }
                if (ConfigurationManager.AppSettings["ShowMediaMain"] == null) { SettingSave("ShowMediaMain", "True"); }
                if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == null) { SettingSave("MinimizeAppOnShow", "False"); }
                if (ConfigurationManager.AppSettings["ShortcutVolume"] == null) { SettingSave("ShortcutVolume", "True"); }
                if (ConfigurationManager.AppSettings["LaunchFpsOverlayer"] == null) { SettingSave("LaunchFpsOverlayer", "False"); }

                //Background settings
                if (ConfigurationManager.AppSettings["VideoBackground"] == null) { SettingSave("VideoBackground", "True"); }
                if (ConfigurationManager.AppSettings["DesktopBackground"] == null) { SettingSave("DesktopBackground", "False"); }
                if (ConfigurationManager.AppSettings["BackgroundBrightness"] == null) { SettingSave("BackgroundBrightness", "80"); }
                if (ConfigurationManager.AppSettings["BackgroundPlayVolume"] == null) { SettingSave("BackgroundPlayVolume", "30"); }
                if (ConfigurationManager.AppSettings["BackgroundPlaySpeed"] == null) { SettingSave("BackgroundPlaySpeed", "100"); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}