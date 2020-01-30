using System;
using System.Configuration;
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
                if (ConfigurationManager.AppSettings["SoundVolume"] == null) { SettingSave("SoundVolume", "70"); }

                if (ConfigurationManager.AppSettings["DisplayMonitor"] == null) { SettingSave("DisplayMonitor", "0"); } //Shared
                if (ConfigurationManager.AppSettings["ColorAccentLight"] == null) { SettingSave("ColorAccentLight", "#00C7FF"); } //Shared
                if (ConfigurationManager.AppSettings["ServerPort"] == null) { SettingSave("ServerPort", "1010"); } //Shared

                if (ConfigurationManager.AppSettings["WindowSizeWidth"] == null) { SettingSave("WindowSizeWidth", "1280"); }
                if (ConfigurationManager.AppSettings["WindowSizeHeight"] == null) { SettingSave("WindowSizeHeight", "720"); }

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
                if (ConfigurationManager.AppSettings["InterfaceSound"] == null) { SettingSave("InterfaceSound", "False"); }
                if (ConfigurationManager.AppSettings["CloseMediaScreen"] == null) { SettingSave("CloseMediaScreen", "False"); }
                if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == null) { SettingSave("MinimizeAppOnShow", "False"); }
                if (ConfigurationManager.AppSettings["ShortcutVolume"] == null) { SettingSave("ShortcutVolume", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutAltEnter"] == null) { SettingSave("ShortcutAltEnter", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutAltF4"] == null) { SettingSave("ShortcutAltF4", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutAltTab"] == null) { SettingSave("ShortcutAltTab", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutWinTab"] == null) { SettingSave("ShortcutWinTab", "False"); }
                if (ConfigurationManager.AppSettings["ShortcutScreenshot"] == null) { SettingSave("ShortcutScreenshot", "True"); }

                if (ConfigurationManager.AppSettings["LaunchDirectXInput"] == null) { SettingSave("LaunchDirectXInput", "False"); }

                //Background settings
                if (ConfigurationManager.AppSettings["VideoBackground"] == null) { SettingSave("VideoBackground", "True"); }
                if (ConfigurationManager.AppSettings["DesktopBackground"] == null) { SettingSave("DesktopBackground", "False"); }
                if (ConfigurationManager.AppSettings["BackgroundBrightness"] == null) { SettingSave("BackgroundBrightness", "90"); }
                if (ConfigurationManager.AppSettings["BackgroundPlayVolume"] == null) { SettingSave("BackgroundPlayVolume", "30"); }
                if (ConfigurationManager.AppSettings["BackgroundPlaySpeed"] == null) { SettingSave("BackgroundPlaySpeed", "100"); }
            }
            catch { }
        }
    }
}