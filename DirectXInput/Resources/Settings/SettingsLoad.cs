using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Load - Application Settings
        async Task Settings_Load()
        {
            try
            {
                cb_SettingsShortcutDisconnectBluetooth.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutDisconnectBluetooth", typeof(bool));
                cb_SettingsExclusiveGuide.IsChecked = SettingLoad(vConfigurationDirectXInput, "ExclusiveGuide", typeof(bool));

                //Load battery settings
                int batteryLevelLowInt = SettingLoad(vConfigurationDirectXInput, "BatteryLowLevel", typeof(int));
                textblock_BatteryLowLevel.Text = textblock_BatteryLowLevel.Tag + ": " + batteryLevelLowInt + "%";
                slider_BatteryLowLevel.Value = batteryLevelLowInt;

                cb_SettingsBatteryLowBlinkLed.IsChecked = SettingLoad(vConfigurationDirectXInput, "BatteryLowBlinkLed", typeof(bool));
                cb_SettingsBatteryLowShowNotification.IsChecked = SettingLoad(vConfigurationDirectXInput, "BatteryLowShowNotification", typeof(bool));
                cb_SettingsBatteryLowPlaySound.IsChecked = SettingLoad(vConfigurationDirectXInput, "BatteryLowPlaySound", typeof(bool));

                //Load controller settings
                int controllerIdleDisconnectMinInt = SettingLoad(vConfigurationDirectXInput, "ControllerIdleDisconnectMin", typeof(int));
                textblock_ControllerIdleDisconnectMin.Text = textblock_ControllerIdleDisconnectMin.Tag + ": " + controllerIdleDisconnectMinInt + " minutes";
                slider_ControllerIdleDisconnectMin.Value = controllerIdleDisconnectMinInt;

                string ControllerColor0 = SettingLoad(vConfigurationDirectXInput, "ControllerColor0", typeof(string));
                SolidColorBrush ControllerColor0Brush = new BrushConverter().ConvertFrom(ControllerColor0) as SolidColorBrush;
                colorpicker_Controller0.Background = ControllerColor0Brush;
                vController0.Color = ControllerColor0Brush.Color;

                string ControllerColor1 = SettingLoad(vConfigurationDirectXInput, "ControllerColor1", typeof(string));
                SolidColorBrush ControllerColor1Brush = new BrushConverter().ConvertFrom(ControllerColor1) as SolidColorBrush;
                colorpicker_Controller1.Background = ControllerColor1Brush;
                vController1.Color = ControllerColor1Brush.Color;

                string ControllerColor2 = SettingLoad(vConfigurationDirectXInput, "ControllerColor2", typeof(string));
                SolidColorBrush ControllerColor2Brush = new BrushConverter().ConvertFrom(ControllerColor2) as SolidColorBrush;
                colorpicker_Controller2.Background = ControllerColor2Brush;
                vController2.Color = ControllerColor2Brush.Color;

                string ControllerColor3 = SettingLoad(vConfigurationDirectXInput, "ControllerColor3", typeof(string));
                SolidColorBrush ControllerColor3Brush = new BrushConverter().ConvertFrom(ControllerColor3) as SolidColorBrush;
                colorpicker_Controller3.Background = ControllerColor3Brush;
                vController3.Color = ControllerColor3Brush.Color;

                //Load shortcut settings
                cb_SettingsShortcutLaunchCtrlUI.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI", typeof(bool));
                cb_SettingsShortcutLaunchCtrlUIKeyboard.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutLaunchCtrlUIKeyboard", typeof(bool));
                cb_SettingsShortcutKeyboardPopup.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutKeyboardPopup", typeof(bool));
                cb_SettingsShortcutAltEnter.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutAltEnter", typeof(bool));
                cb_SettingsShortcutAltTab.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutAltTab", typeof(bool));
                cb_SettingsShortcutCtrlAltDelete.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutCtrlAltDelete", typeof(bool));
                cb_SettingsShortcutMuteOutput.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutMuteOutput", typeof(bool));
                cb_SettingsShortcutMuteInput.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutMuteInput", typeof(bool));

                //Load capture settings
                cb_SettingsShortcutCaptureImage.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutCaptureImage", typeof(bool));
                cb_SettingsShortcutCaptureVideo.IsChecked = SettingLoad(vConfigurationDirectXInput, "ShortcutCaptureVideo", typeof(bool));

                //Load keyboard settings
                cb_SettingsKeyboardCloseNoController.IsChecked = SettingLoad(vConfigurationDirectXInput, "KeyboardCloseNoController", typeof(bool));
                cb_SettingsKeyboardResetPosition.IsChecked = SettingLoad(vConfigurationDirectXInput, "KeyboardResetPosition", typeof(bool));
                combobox_KeyboardLayout.SelectedIndex = SettingLoad(vConfigurationDirectXInput, "KeyboardLayout", typeof(int));

                //Load mouse sensitivity
                textblock_SettingsKeyboardMouseMoveSensitivity.Text = textblock_SettingsKeyboardMouseMoveSensitivity.Tag.ToString() + SettingLoad(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity", typeof(string));
                slider_SettingsKeyboardMouseMoveSensitivity.Value = SettingLoad(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity", typeof(double));
                textblock_SettingsKeyboardMouseScrollSensitivity2.Text = textblock_SettingsKeyboardMouseScrollSensitivity2.Tag.ToString() + SettingLoad(vConfigurationDirectXInput, "KeyboardMouseScrollSensitivity2", typeof(string));
                slider_SettingsKeyboardMouseScrollSensitivity2.Value = SettingLoad(vConfigurationDirectXInput, "KeyboardMouseScrollSensitivity2", typeof(double));

                //Load media settings
                combobox_ControllerLedCondition.SelectedIndex = SettingLoad(vConfigurationDirectXInput, "ControllerLedCondition", typeof(int));
                textblock_SettingsMediaVolumeStep.Text = textblock_SettingsMediaVolumeStep.Tag.ToString() + SettingLoad(vConfigurationDirectXInput, "MediaVolumeStep", typeof(string));
                slider_SettingsMediaVolumeStep.Value = SettingLoad(vConfigurationDirectXInput, "MediaVolumeStep", typeof(double));

                //Set the application name to string to check shortcuts
                string targetName = AVFunctions.ApplicationName();

                //Check if application is set to launch on Windows startup
                string targetFileStartup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");
                if (File.Exists(targetFileStartup))
                {
                    cb_SettingsWindowsStartup.IsChecked = true;
                }

                //Wait for settings to have loaded
                await Task.Delay(1500);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load application settings: " + ex.Message);
            }
        }
    }
}