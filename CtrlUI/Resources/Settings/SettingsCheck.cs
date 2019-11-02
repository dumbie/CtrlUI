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
                if (ConfigurationManager.AppSettings["DisplayMonitor"] == null) { SettingSave("DisplayMonitor", "0"); }

                if (ConfigurationManager.AppSettings["WindowSizeWidth"] == null) { SettingSave("WindowSizeWidth", "1280"); }
                if (ConfigurationManager.AppSettings["WindowSizeHeight"] == null) { SettingSave("WindowSizeHeight", "720"); }

                if (ConfigurationManager.AppSettings["LaunchFullscreen"] == null) { SettingSave("LaunchFullscreen", "True"); }
                if (ConfigurationManager.AppSettings["LaunchMinimized"] == null) { SettingSave("LaunchMinimized", "False"); }
                if (ConfigurationManager.AppSettings["ShowOtherShortcuts"] == null) { SettingSave("ShowOtherShortcuts", "True"); }
                if (ConfigurationManager.AppSettings["ShowOtherProcesses"] == null) { SettingSave("ShowOtherProcesses", "True"); }
                if (ConfigurationManager.AppSettings["HideAppProcesses"] == null) { SettingSave("HideAppProcesses", "True"); }

                if (ConfigurationManager.AppSettings["DirectoryShortcuts"] == null) { SettingSave("DirectoryShortcuts", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Shortcuts"); }
                if (ConfigurationManager.AppSettings["HideBatteryLevel"] == null) { SettingSave("HideBatteryLevel", "False"); }
                if (ConfigurationManager.AppSettings["HideMouseCursor"] == null) { SettingSave("HideMouseCursor", "True"); }
                if (ConfigurationManager.AppSettings["HideControllerHelp"] == null) { SettingSave("HideControllerHelp", "False"); }
                if (ConfigurationManager.AppSettings["ShowHiddenFilesFolders"] == null) { SettingSave("ShowHiddenFilesFolders", "False"); }
                if (ConfigurationManager.AppSettings["HideNetworkDrives"] == null) { SettingSave("HideNetworkDrives", "False"); }
                if (ConfigurationManager.AppSettings["InterfaceSound"] == null) { SettingSave("InterfaceSound", "False"); }
                if (ConfigurationManager.AppSettings["DesktopBackground"] == null) { SettingSave("DesktopBackground", "False"); }
                if (ConfigurationManager.AppSettings["CloseMediaScreen"] == null) { SettingSave("CloseMediaScreen", "False"); }
                if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == null) { SettingSave("MinimizeAppOnShow", "False"); }
                if (ConfigurationManager.AppSettings["ShortcutVolume"] == null) { SettingSave("ShortcutVolume", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutAltEnter"] == null) { SettingSave("ShortcutAltEnter", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutAltF4"] == null) { SettingSave("ShortcutAltF4", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutAltTab"] == null) { SettingSave("ShortcutAltTab", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutWinTab"] == null) { SettingSave("ShortcutWinTab", "False"); }
                if (ConfigurationManager.AppSettings["ShortcutScreenshot"] == null) { SettingSave("ShortcutScreenshot", "True"); }

                if (ConfigurationManager.AppSettings["SocketClientIp"] == null) { SettingSave("SocketClientIp", "127.0.0.1"); }
                if (ConfigurationManager.AppSettings["SocketClientPort"] == null) { SettingSave("SocketClientPort", "1010"); }

                if (ConfigurationManager.AppSettings["LaunchDirectXInput"] == null) { SettingSave("LaunchDirectXInput", "False"); }
            }
            catch { }
        }
    }
}