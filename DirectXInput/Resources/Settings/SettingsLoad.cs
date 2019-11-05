using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Load - Socket server settings
        void Settings_LoadSocket()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                string SocketServerIp = Convert.ToString(config.AppSettings.Settings["SocketClientIp"].Value);
                int SocketServerPort = Convert.ToInt32(config.AppSettings.Settings["SocketClientPort"].Value);

                vSocketServer.vTcpListenerIp = SocketServerIp;
                vSocketServer.vTcpListenerPort = SocketServerPort + 1;
            }
            catch { }
        }

        //Load - Application Settings
        async Task Settings_Load()
        {
            try
            {
                cb_SettingsShortcutDisconnectBluetooth.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutDisconnectBluetooth"]);
                cb_SettingsExclusiveGuide.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ExclusiveGuide"]);
                cb_SettingsShortcutLaunchCtrlUI.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchCtrlUI"]);
                cb_SettingsPlaySoundBatteryLow.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["PlaySoundBatteryLow"]);
                cb_SettingsShortcutLaunchKeyboardController.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchKeyboardController"]);

                //Set the application name to string to check shortcuts
                string TargetName_Admin = Assembly.GetEntryAssembly().GetName().Name;

                //Check if application is set to launch on Windows startup
                string TargetFileStartup_Admin = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + TargetName_Admin + ".url";
                if (File.Exists(TargetFileStartup_Admin)) { cb_SettingsWindowsStartup.IsChecked = true; }
            }
            catch (Exception Ex) { await MessageBoxPopup("Failed to load the application settings.", Ex.Message, "Ok", "", "", ""); }
        }
    }
}