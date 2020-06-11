using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Load - Application Settings
        bool Settings_Load()
        {
            try
            {
                cb_SettingsShortcutDisconnectBluetooth.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutDisconnectBluetooth"]);
                cb_SettingsExclusiveGuide.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ExclusiveGuide"]);

                cb_SettingsBatteryPlaySoundLow.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["BatteryPlaySoundLow"]);
                cb_SettingsBatteryShowIconLow.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["BatteryShowIconLow"]);
                cb_SettingsBatteryShowPercentageLow.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["BatteryShowPercentageLow"]);

                cb_SettingsShortcutLaunchCtrlUI.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchCtrlUI"]);
                cb_SettingsShortcutLaunchKeyboardController.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchKeyboardController"]);
                cb_SettingsShortcutAltEnter.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltEnter"]);
                cb_SettingsShortcutAltF4.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltF4"]);
                cb_SettingsShortcutAltTab.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltTab"]);
                cb_SettingsShortcutWinTab.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutWinTab"]);
                cb_SettingsShortcutScreenshot.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutScreenshot"]);

                //Load keyboard opacity
                textblock_KeyboardOpacity.Text = textblock_KeyboardOpacity.Tag + ": " + ConfigurationManager.AppSettings["KeyboardOpacity"].ToString() + "%";
                slider_KeyboardOpacity.Value = Convert.ToDouble(ConfigurationManager.AppSettings["KeyboardOpacity"]);

                //Load keyboard layout
                combobox_KeyboardLayout.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardLayout"]);

                //Load keyboard domain extensions
                textbox_SettingsDomainExtensionDefault.Text = ConfigurationManager.AppSettings["KeyboardDomainExtensionDefault"].ToString();
                textbox_SettingsDomainExtension.Text = ConfigurationManager.AppSettings["KeyboardDomainExtension"].ToString();

                //Load mouse sensitivity
                textblock_SettingsMouseMoveSensitivity.Text = textblock_SettingsMouseMoveSensitivity.Tag.ToString() + Convert.ToInt32(ConfigurationManager.AppSettings["MouseMoveSensitivity"]);
                slider_SettingsMouseMoveSensitivity.Value = Convert.ToInt32(ConfigurationManager.AppSettings["MouseMoveSensitivity"]);
                textblock_SettingsMouseScrollSensitivity.Text = textblock_SettingsMouseScrollSensitivity.Tag.ToString() + Convert.ToInt32(ConfigurationManager.AppSettings["MouseScrollSensitivity"]);
                slider_SettingsMouseScrollSensitivity.Value = Convert.ToInt32(ConfigurationManager.AppSettings["MouseScrollSensitivity"]);

                //Set the application name to string to check shortcuts
                string targetName = Assembly.GetEntryAssembly().GetName().Name;

                //Check if application is set to launch on Windows startup
                string targetFileStartup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");
                if (File.Exists(targetFileStartup))
                {
                    cb_SettingsWindowsStartup.IsChecked = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the application settings: " + ex.Message);
                return false;
            }
        }
    }
}