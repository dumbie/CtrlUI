using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media;
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

                //Load battery settings
                int batteryLevelLowInt = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "BatteryLowLevel"));
                textblock_BatteryLowLevel.Text = textblock_BatteryLowLevel.Tag + ": " + batteryLevelLowInt + "%";
                slider_BatteryLowLevel.Value = batteryLevelLowInt;

                cb_SettingsBatteryLowBlinkLed.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryLowBlinkLed"));
                cb_SettingsBatteryLowShowNotification.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryLowShowNotification"));
                cb_SettingsBatteryLowPlaySound.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryLowPlaySound"));

                //Load controller settings
                int controllerIdleDisconnectMinInt = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "ControllerIdleDisconnectMin"));
                textblock_ControllerIdleDisconnectMin.Text = textblock_ControllerIdleDisconnectMin.Tag + ": " + batteryLevelLowInt + " minutes";
                slider_ControllerIdleDisconnectMin.Value = batteryLevelLowInt;

                string ControllerColor0 = Setting_Load(vConfigurationDirectXInput, "ControllerColor0").ToString();
                SolidColorBrush ControllerColor0Brush = new BrushConverter().ConvertFrom(ControllerColor0) as SolidColorBrush;
                colorpicker_Controller0.Background = ControllerColor0Brush;
                vController0.Color = ControllerColor0Brush.Color;

                string ControllerColor1 = Setting_Load(vConfigurationDirectXInput, "ControllerColor1").ToString();
                SolidColorBrush ControllerColor1Brush = new BrushConverter().ConvertFrom(ControllerColor1) as SolidColorBrush;
                colorpicker_Controller1.Background = ControllerColor1Brush;
                vController1.Color = ControllerColor1Brush.Color;

                string ControllerColor2 = Setting_Load(vConfigurationDirectXInput, "ControllerColor2").ToString();
                SolidColorBrush ControllerColor2Brush = new BrushConverter().ConvertFrom(ControllerColor2) as SolidColorBrush;
                colorpicker_Controller2.Background = ControllerColor2Brush;
                vController2.Color = ControllerColor2Brush.Color;

                string ControllerColor3 = Setting_Load(vConfigurationDirectXInput, "ControllerColor3").ToString();
                SolidColorBrush ControllerColor3Brush = new BrushConverter().ConvertFrom(ControllerColor3) as SolidColorBrush;
                colorpicker_Controller3.Background = ControllerColor3Brush;
                vController3.Color = ControllerColor3Brush.Color;

                //Load shortcut settings
                cb_SettingsShortcutLaunchCtrlUI.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI"));
                cb_SettingsShortcutLaunchKeyboardController.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutLaunchKeyboardController"));
                cb_SettingsShortcutAltEnter.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltEnter"));
                cb_SettingsShortcutAltF4.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltF4"));
                cb_SettingsShortcutAltTab.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltTab"));
                cb_SettingsShortcutWinTab.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutWinTab"));
                cb_SettingsShortcutScreenshot.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutScreenshot"));
                cb_SettingsShortcutMediaPopup.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutMediaPopup"));
                combobox_ShortcutMuteFunction.SelectedIndex = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "ShortcutMuteFunction"));

                //Load keyboard settings
                textblock_KeyboardOpacity.Text = textblock_KeyboardOpacity.Tag + ": " + Setting_Load(vConfigurationDirectXInput, "KeyboardOpacity").ToString() + "%";
                slider_KeyboardOpacity.Value = Convert.ToDouble(Setting_Load(vConfigurationDirectXInput, "KeyboardOpacity"));
                cb_SettingsKeyboardCloseNoController.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "KeyboardCloseNoController"));
                cb_SettingsKeyboardResetPosition.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "KeyboardResetPosition"));
                combobox_KeyboardLayout.SelectedIndex = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardLayout"));

                //Load mouse sensitivity
                textblock_SettingsKeyboardMouseMoveSensitivity.Text = textblock_SettingsKeyboardMouseMoveSensitivity.Tag.ToString() + Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity"));
                slider_SettingsKeyboardMouseMoveSensitivity.Value = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity"));
                textblock_SettingsKeyboardMouseScrollSensitivity.Text = textblock_SettingsKeyboardMouseScrollSensitivity.Tag.ToString() + Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMouseScrollSensitivity"));
                slider_SettingsKeyboardMouseScrollSensitivity.Value = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMouseScrollSensitivity"));

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