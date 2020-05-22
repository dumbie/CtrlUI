using System;
using System.Configuration;
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
                if (ConfigurationManager.AppSettings["AppFirstLaunch"] == null) { SettingSave(vConfigurationApplication, "AppFirstLaunch", "True"); }
                if (ConfigurationManager.AppSettings["AppUpdateCheck"] == null) { SettingSave(vConfigurationApplication, "AppUpdateCheck", DateTime.Now.ToString(vAppCultureInfo)); }
                if (ConfigurationManager.AppSettings["AppFontSize"] == null) { SettingSave(vConfigurationApplication, "AppFontSize", "0"); }

                if (ConfigurationManager.AppSettings["DisplayMonitor"] == null) { SettingSave(vConfigurationApplication, "DisplayMonitor", "1"); } //Shared
                if (ConfigurationManager.AppSettings["ColorAccentLight"] == null) { SettingSave(vConfigurationApplication, "ColorAccentLight", "#00C7FF"); } //Shared
                if (ConfigurationManager.AppSettings["ServerPort"] == null) { SettingSave(vConfigurationApplication, "ServerPort", "1010"); } //Shared

                if (ConfigurationManager.AppSettings["LaunchFullscreen"] == null) { SettingSave(vConfigurationApplication, "LaunchFullscreen", "True"); }
                if (ConfigurationManager.AppSettings["LaunchMinimized"] == null) { SettingSave(vConfigurationApplication, "LaunchMinimized", "False"); }
                if (ConfigurationManager.AppSettings["ShowOtherShortcuts"] == null) { SettingSave(vConfigurationApplication, "ShowOtherShortcuts", "True"); }
                if (ConfigurationManager.AppSettings["ShowOtherProcesses"] == null) { SettingSave(vConfigurationApplication, "ShowOtherProcesses", "True"); }
                if (ConfigurationManager.AppSettings["HideAppProcesses"] == null) { SettingSave(vConfigurationApplication, "HideAppProcesses", "False"); }

                if (ConfigurationManager.AppSettings["HideBatteryLevel"] == null) { SettingSave(vConfigurationApplication, "HideBatteryLevel", "False"); }
                if (ConfigurationManager.AppSettings["HideMouseCursor"] == null) { SettingSave(vConfigurationApplication, "HideMouseCursor", "True"); }
                if (ConfigurationManager.AppSettings["HideControllerHelp"] == null) { SettingSave(vConfigurationApplication, "HideControllerHelp", "False"); }
                if (ConfigurationManager.AppSettings["ShowHiddenFilesFolders"] == null) { SettingSave(vConfigurationApplication, "ShowHiddenFilesFolders", "False"); }
                if (ConfigurationManager.AppSettings["HideNetworkDrives"] == null) { SettingSave(vConfigurationApplication, "HideNetworkDrives", "False"); }

                if (ConfigurationManager.AppSettings["InterfaceSound"] == null) { SettingSave(vConfigurationApplication, "InterfaceSound", "True"); }
                if (ConfigurationManager.AppSettings["InterfaceSoundVolume"] == null) { SettingSave(vConfigurationApplication, "InterfaceSoundVolume", "70"); }
                if (ConfigurationManager.AppSettings["InterfaceSoundPackName"] == null) { SettingSave(vConfigurationApplication, "InterfaceSoundPackName", "Default"); } //Shared
                if (ConfigurationManager.AppSettings["InterfaceClockStyleName"] == null) { SettingSave(vConfigurationApplication, "InterfaceClockStyleName", "Cortana"); }
                if (ConfigurationManager.AppSettings["InterfaceFontStyleName"] == null) { SettingSave(vConfigurationApplication, "InterfaceFontStyleName", "Segoe UI"); }

                if (ConfigurationManager.AppSettings["CloseMediaScreen"] == null) { SettingSave(vConfigurationApplication, "CloseMediaScreen", "False"); }
                if (ConfigurationManager.AppSettings["ShowMediaMain"] == null) { SettingSave(vConfigurationApplication, "ShowMediaMain", "True"); }
                if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == null) { SettingSave(vConfigurationApplication, "MinimizeAppOnShow", "False"); }
                if (ConfigurationManager.AppSettings["ShortcutVolume"] == null) { SettingSave(vConfigurationApplication, "ShortcutVolume", "True"); }
                if (ConfigurationManager.AppSettings["LaunchFpsOverlayer"] == null) { SettingSave(vConfigurationApplication, "LaunchFpsOverlayer", "False"); }

                //Background settings
                if (ConfigurationManager.AppSettings["VideoBackground"] == null) { SettingSave(vConfigurationApplication, "VideoBackground", "True"); }
                if (ConfigurationManager.AppSettings["DesktopBackground"] == null) { SettingSave(vConfigurationApplication, "DesktopBackground", "False"); }
                if (ConfigurationManager.AppSettings["BackgroundBrightness"] == null) { SettingSave(vConfigurationApplication, "BackgroundBrightness", "80"); }
                if (ConfigurationManager.AppSettings["BackgroundPlayVolume"] == null) { SettingSave(vConfigurationApplication, "BackgroundPlayVolume", "30"); }
                if (ConfigurationManager.AppSettings["BackgroundPlaySpeed"] == null) { SettingSave(vConfigurationApplication, "BackgroundPlaySpeed", "100"); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}