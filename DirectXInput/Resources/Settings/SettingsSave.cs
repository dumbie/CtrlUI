using System;
using System.Diagnostics;
using System.Windows.Media;
using static DirectXInput.AppVariables;
using static DirectXInput.SettingsNotify;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;
using static LibraryShared.Settings;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Save - Monitor Application Settings
        void Settings_Save()
        {
            try
            {
                cb_SettingsShortcutDisconnectBluetooth.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutDisconnectBluetooth", cb_SettingsShortcutDisconnectBluetooth.IsChecked.ToString());
                };

                cb_SettingsExclusiveGuide.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ExclusiveGuide", cb_SettingsExclusiveGuide.IsChecked.ToString());
                };

                cb_SettingsWindowsStartup.Click += (sender, e) => { ManageShortcutStartup(); };

                //Battery settings
                cb_SettingsBatteryShowIconLow.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "BatteryShowIconLow", cb_SettingsBatteryShowIconLow.IsChecked.ToString());

                    //Check all controllers for low battery level
                    CheckAllControllersLowBattery();
                };

                cb_SettingsBatteryShowPercentageLow.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "BatteryShowPercentageLow", cb_SettingsBatteryShowPercentageLow.IsChecked.ToString());

                    //Check all controllers for low battery level
                    CheckAllControllersLowBattery();
                };

                cb_SettingsBatteryPlaySoundLow.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "BatteryPlaySoundLow", cb_SettingsBatteryPlaySoundLow.IsChecked.ToString());
                };

                //Controller settings
                slider_ControllerIdleDisconnectMin.ValueChanged += (sender, e) =>
                {
                    string controllerIdleDisconnectMinString = slider_ControllerIdleDisconnectMin.Value.ToString();
                    Setting_Save(vConfigurationDirectXInput, "ControllerIdleDisconnectMin", controllerIdleDisconnectMinString);
                    textblock_ControllerIdleDisconnectMin.Text = textblock_ControllerIdleDisconnectMin.Tag + ": " + controllerIdleDisconnectMinString + " minutes";
                };

                colorpicker_Controller0.SelectedColorChanged += async (Color color) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ControllerColor0", color.ToString());
                    ControllerOutput(vController0, false, false);
                    App.vWindowOverlay.stackpanel_Battery_Warning_Controller1_Color.Background = new BrushConverter().ConvertFrom(color.ToString()) as SolidColorBrush;
                    await NotifyCtrlUISettingChanged("ControllerColor");
                };

                colorpicker_Controller1.SelectedColorChanged += async (Color color) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ControllerColor1", color.ToString());
                    ControllerOutput(vController1, false, false);
                    App.vWindowOverlay.stackpanel_Battery_Warning_Controller2_Color.Background = new BrushConverter().ConvertFrom(color.ToString()) as SolidColorBrush;
                    await NotifyCtrlUISettingChanged("ControllerColor");
                };

                colorpicker_Controller2.SelectedColorChanged += async (Color color) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ControllerColor2", color.ToString());
                    ControllerOutput(vController2, false, false);
                    App.vWindowOverlay.stackpanel_Battery_Warning_Controller3_Color.Background = new BrushConverter().ConvertFrom(color.ToString()) as SolidColorBrush;
                    await NotifyCtrlUISettingChanged("ControllerColor");
                };

                colorpicker_Controller3.SelectedColorChanged += async (Color color) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ControllerColor3", color.ToString());
                    ControllerOutput(vController3, false, false);
                    App.vWindowOverlay.stackpanel_Battery_Warning_Controller4_Color.Background = new BrushConverter().ConvertFrom(color.ToString()) as SolidColorBrush;
                    await NotifyCtrlUISettingChanged("ControllerColor");
                };

                cb_ControllerShowDebugInformation.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShowDebugInformation", cb_ControllerShowDebugInformation.IsChecked.ToString());
                };

                //Shortcut settings
                cb_SettingsShortcutLaunchCtrlUI.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI", cb_SettingsShortcutLaunchCtrlUI.IsChecked.ToString());
                };

                cb_SettingsShortcutLaunchKeyboardController.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutLaunchKeyboardController", cb_SettingsShortcutLaunchKeyboardController.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltEnter.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutAltEnter", cb_SettingsShortcutAltEnter.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltF4.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutAltF4", cb_SettingsShortcutAltF4.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltTab.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutAltTab", cb_SettingsShortcutAltTab.IsChecked.ToString());
                    if (cb_SettingsShortcutAltTab.IsChecked == true)
                    {
                        Setting_Save(vConfigurationDirectXInput, "ShortcutWinTab", "False");
                        cb_SettingsShortcutWinTab.IsChecked = false;
                    }
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutWinTab.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutWinTab", cb_SettingsShortcutWinTab.IsChecked.ToString());
                    if (cb_SettingsShortcutWinTab.IsChecked == true)
                    {
                        Setting_Save(vConfigurationDirectXInput, "ShortcutAltTab", "False");
                        cb_SettingsShortcutAltTab.IsChecked = false;
                    }
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutScreenshot.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutScreenshot", cb_SettingsShortcutScreenshot.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutMediaPopup.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutMediaPopup", cb_SettingsShortcutMediaPopup.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                //Keyboard settings
                slider_KeyboardOpacity.ValueChanged += (sender, e) =>
                {
                    textblock_KeyboardOpacity.Text = textblock_KeyboardOpacity.Tag + ": " + slider_KeyboardOpacity.Value.ToString("0.00") + "%";
                    Setting_Save(vConfigurationDirectXInput, "KeyboardOpacity", slider_KeyboardOpacity.Value.ToString("0.00"));
                    App.vWindowKeyboard.UpdatePopupOpacity(false);
                };

                cb_SettingsKeyboardCloseNoController.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardCloseNoController", cb_SettingsKeyboardCloseNoController.IsChecked.ToString());
                };

                cb_SettingsKeyboardResetPosition.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardResetPosition", cb_SettingsKeyboardResetPosition.IsChecked.ToString());
                };

                combobox_KeyboardLayout.SelectionChanged += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardLayout", combobox_KeyboardLayout.SelectedIndex.ToString());
                    await App.vWindowKeyboard.UpdateKeyboardLayout();
                };

                slider_SettingsMouseMoveSensitivity.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsMouseMoveSensitivity.Text = textblock_SettingsMouseMoveSensitivity.Tag.ToString() + Convert.ToInt32(slider_SettingsMouseMoveSensitivity.Value);
                    Setting_Save(vConfigurationDirectXInput, "MouseMoveSensitivity", Convert.ToInt32(slider_SettingsMouseMoveSensitivity.Value).ToString());
                };

                slider_SettingsMouseScrollSensitivity.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsMouseScrollSensitivity.Text = textblock_SettingsMouseScrollSensitivity.Tag.ToString() + Convert.ToInt32(slider_SettingsMouseScrollSensitivity.Value);
                    Setting_Save(vConfigurationDirectXInput, "MouseScrollSensitivity", Convert.ToInt32(slider_SettingsMouseScrollSensitivity.Value).ToString());
                };

                //Keypad settings
                slider_KeypadOpacity.ValueChanged += (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.KeypadOpacity = slider_KeypadOpacity.Value;

                    //Save changes to Json file
                    JsonSaveObject(vDirectKeypadMapping, "DirectKeypadMapping");

                    textblock_KeypadOpacity.Text = textblock_KeypadOpacity.Tag + ": " + slider_KeypadOpacity.Value.ToString("0.00") + "%";
                    App.vWindowKeypad.UpdateKeypadOpacity();
                };

                combobox_KeypadDisplayStyle.SelectionChanged += (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.KeypadDisplayStyle = combobox_KeypadDisplayStyle.SelectedIndex;

                    //Save changes to Json file
                    JsonSaveObject(vDirectKeypadMapping, "DirectKeypadMapping");

                    App.vWindowKeypad.UpdateKeypadStyle();
                };

                slider_KeypadDisplaySize.ValueChanged += async (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.KeypadDisplaySize = Convert.ToInt32(slider_KeypadDisplaySize.Value);

                    //Save changes to Json file
                    JsonSaveObject(vDirectKeypadMapping, "DirectKeypadMapping");

                    textblock_KeypadDisplaySize.Text = textblock_KeypadDisplaySize.Tag + ": " + selectedProfile.KeypadDisplaySize + "%";

                    //Update the keypad size
                    double keypadHeight = App.vWindowKeypad.UpdateKeypadSize();

                    //Notify - Fps Overlayer keypad size changed
                    await NotifyFpsOverlayerKeypadSizeChanged(Convert.ToInt32(keypadHeight));
                };

                slider_KeypadRepeatIntervalMs.ValueChanged += (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.ButtonRepeatIntervalMs = Convert.ToInt32(slider_KeypadRepeatIntervalMs.Value);

                    //Save changes to Json file
                    JsonSaveObject(vDirectKeypadMapping, "DirectKeypadMapping");

                    textblock_KeypadRepeatIntervalMs.Text = textblock_KeypadRepeatIntervalMs.Tag + ": " + selectedProfile.ButtonRepeatIntervalMs + "ms";
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}