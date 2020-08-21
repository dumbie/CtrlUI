using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Load - Application Settings
        bool Settings_Load()
        {
            try
            {
                cb_SettingsShortcutDisconnectBluetooth.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutDisconnectBluetooth"));
                cb_SettingsExclusiveGuide.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ExclusiveGuide"));

                cb_SettingsBatteryPlaySoundLow.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryPlaySoundLow"));
                cb_SettingsBatteryShowIconLow.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryShowIconLow"));
                cb_SettingsBatteryShowPercentageLow.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryShowPercentageLow"));

                //Load controller idle
                int controllerIdleDisconnectMinInt = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "ControllerIdleDisconnectMin"));
                textblock_ControllerIdleDisconnectMin.Text = textblock_ControllerIdleDisconnectMin.Tag + ": " + controllerIdleDisconnectMinInt + " minutes";
                slider_ControllerIdleDisconnectMin.Value = controllerIdleDisconnectMinInt;

                cb_SettingsShortcutLaunchCtrlUI.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI"));
                cb_SettingsShortcutLaunchKeyboardController.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutLaunchKeyboardController"));
                cb_SettingsShortcutAltEnter.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltEnter"));
                cb_SettingsShortcutAltF4.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltF4"));
                cb_SettingsShortcutAltTab.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltTab"));
                cb_SettingsShortcutWinTab.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutWinTab"));
                cb_SettingsShortcutScreenshot.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutScreenshot"));

                //Load keyboard opacity
                textblock_KeyboardOpacity.Text = textblock_KeyboardOpacity.Tag + ": " + Setting_Load(vConfigurationDirectXInput, "KeyboardOpacity").ToString() + "%";
                slider_KeyboardOpacity.Value = Convert.ToDouble(Setting_Load(vConfigurationDirectXInput, "KeyboardOpacity"));

                //Load keyboard layout
                combobox_KeyboardLayout.SelectedIndex = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardLayout"));

                //Load keyboard domain extensions
                textbox_SettingsDomainExtensionDefault.Text = Setting_Load(vConfigurationDirectXInput, "KeyboardDomainExtensionDefault").ToString();
                textbox_SettingsDomainExtension.Text = Setting_Load(vConfigurationDirectXInput, "KeyboardDomainExtension").ToString();

                //Load mouse sensitivity
                textblock_SettingsMouseMoveSensitivity.Text = textblock_SettingsMouseMoveSensitivity.Tag.ToString() + Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "MouseMoveSensitivity"));
                slider_SettingsMouseMoveSensitivity.Value = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "MouseMoveSensitivity"));
                textblock_SettingsMouseScrollSensitivity.Text = textblock_SettingsMouseScrollSensitivity.Tag.ToString() + Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "MouseScrollSensitivity"));
                slider_SettingsMouseScrollSensitivity.Value = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "MouseScrollSensitivity"));

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