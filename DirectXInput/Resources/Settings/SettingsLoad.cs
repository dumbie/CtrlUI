﻿using System;
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

                cb_SettingsBatteryPlaySoundLow.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryPlaySoundLow"));
                cb_SettingsBatteryShowIconLow.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryShowIconLow"));
                cb_SettingsBatteryShowPercentageLow.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryShowPercentageLow"));

                //Load controller settings
                int controllerIdleDisconnectMinInt = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "ControllerIdleDisconnectMin"));
                textblock_ControllerIdleDisconnectMin.Text = textblock_ControllerIdleDisconnectMin.Tag + ": " + controllerIdleDisconnectMinInt + " minutes";
                slider_ControllerIdleDisconnectMin.Value = controllerIdleDisconnectMinInt;

                string ControllerColor0 = Setting_Load(vConfigurationDirectXInput, "ControllerColor0").ToString();
                SolidColorBrush ControllerColor0Brush = new BrushConverter().ConvertFrom(ControllerColor0) as SolidColorBrush;
                colorpicker_Controller0.Background = ControllerColor0Brush;
                App.vWindowOverlay.stackpanel_Battery_Warning_Controller1_Color.Background = ControllerColor0Brush;

                string ControllerColor1 = Setting_Load(vConfigurationDirectXInput, "ControllerColor1").ToString();
                SolidColorBrush ControllerColor1Brush = new BrushConverter().ConvertFrom(ControllerColor1) as SolidColorBrush;
                colorpicker_Controller1.Background = ControllerColor1Brush;
                App.vWindowOverlay.stackpanel_Battery_Warning_Controller2_Color.Background = ControllerColor1Brush;

                string ControllerColor2 = Setting_Load(vConfigurationDirectXInput, "ControllerColor2").ToString();
                SolidColorBrush ControllerColor2Brush = new BrushConverter().ConvertFrom(ControllerColor2) as SolidColorBrush;
                colorpicker_Controller2.Background = ControllerColor2Brush;
                App.vWindowOverlay.stackpanel_Battery_Warning_Controller3_Color.Background = ControllerColor2Brush;

                string ControllerColor3 = Setting_Load(vConfigurationDirectXInput, "ControllerColor3").ToString();
                SolidColorBrush ControllerColor3Brush = new BrushConverter().ConvertFrom(ControllerColor3) as SolidColorBrush;
                colorpicker_Controller3.Background = ControllerColor3Brush;
                App.vWindowOverlay.stackpanel_Battery_Warning_Controller4_Color.Background = ControllerColor3Brush;

                //Load shortcut settings
                cb_SettingsShortcutLaunchCtrlUI.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI"));
                cb_SettingsShortcutLaunchKeyboardController.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutLaunchKeyboardController"));
                cb_SettingsShortcutAltEnter.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltEnter"));
                cb_SettingsShortcutAltF4.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltF4"));
                cb_SettingsShortcutAltTab.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltTab"));
                cb_SettingsShortcutWinTab.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutWinTab"));
                cb_SettingsShortcutScreenshot.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutScreenshot"));
                cb_SettingsShortcutMediaPopup.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutMediaPopup"));

                //Load keyboard settings
                textblock_KeyboardOpacity.Text = textblock_KeyboardOpacity.Tag + ": " + Setting_Load(vConfigurationDirectXInput, "KeyboardOpacity").ToString() + "%";
                slider_KeyboardOpacity.Value = Convert.ToDouble(Setting_Load(vConfigurationDirectXInput, "KeyboardOpacity"));
                cb_SettingsKeyboardCloseNoController.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "KeyboardCloseNoController"));
                cb_SettingsKeyboardResetPosition.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "KeyboardResetPosition"));
                combobox_KeyboardLayout.SelectedIndex = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardLayout"));

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