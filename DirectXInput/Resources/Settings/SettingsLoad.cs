using AVForms;
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
        //Load - CtrlUI Settings
        async Task Settings_Load_CtrlUI()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";
                vConfigurationCtrlUI = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            }
            catch (Exception Ex)
            {
                await AVMessageBox.MessageBoxPopup(this, "Failed to load the CtrlUI settings.", Ex.Message, "Ok", "", "", "");
            }
        }

        //Load - Application Settings
        async Task Settings_Load()
        {
            try
            {
                cb_SettingsShortcutDisconnectBluetooth.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutDisconnectBluetooth"]);
                cb_SettingsExclusiveGuide.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ExclusiveGuide"]);

                cb_SettingsBatteryPlaySoundLow.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["BatteryPlaySoundLow"]);
                cb_SettingsBatteryShowIconLow.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["BatteryShowIconLow"]);

                cb_SettingsShortcutLaunchCtrlUI.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchCtrlUI"]);
                cb_SettingsShortcutLaunchKeyboardController.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchKeyboardController"]);
                cb_SettingsShortcutAltEnter.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltEnter"]);
                cb_SettingsShortcutAltF4.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltF4"]);
                cb_SettingsShortcutAltTab.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltTab"]);
                cb_SettingsShortcutWinTab.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutWinTab"]);
                cb_SettingsShortcutScreenshot.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutScreenshot"]);

                //Set the application name to string to check shortcuts
                string targetName = Assembly.GetEntryAssembly().GetName().Name;

                //Check if application is set to launch on Windows startup
                string targetFileStartup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");
                if (File.Exists(targetFileStartup))
                {
                    cb_SettingsWindowsStartup.IsChecked = true;
                }
            }
            catch (Exception Ex)
            {
                await AVMessageBox.MessageBoxPopup(this, "Failed to load the application settings.", Ex.Message, "Ok", "", "", "");
            }
        }
    }
}