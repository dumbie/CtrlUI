using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Windows.Media;
using static DirectXInput.AppVariables;
using static DirectXInput.ProfileFunctions;
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
                slider_BatteryLowLevel.ValueChanged += async (sender, e) =>
                {
                    string batteryLevelLowString = slider_BatteryLowLevel.Value.ToString();
                    Setting_Save(vConfigurationDirectXInput, "BatteryLowLevel", batteryLevelLowString);
                    textblock_BatteryLowLevel.Text = textblock_BatteryLowLevel.Tag + ": " + batteryLevelLowString + "%";

                    //Check all controllers for low battery level
                    await CheckAllControllersLowBattery(true);
                };

                cb_SettingsBatteryLowBlinkLed.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "BatteryLowBlinkLed", cb_SettingsBatteryLowBlinkLed.IsChecked.ToString());

                    //Check all controllers for low battery level
                    await CheckAllControllersLowBattery(true);
                };

                cb_SettingsBatteryLowShowNotification.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "BatteryLowShowNotification", cb_SettingsBatteryLowShowNotification.IsChecked.ToString());

                    //Check all controllers for low battery level
                    await CheckAllControllersLowBattery(true);
                };

                cb_SettingsBatteryLowPlaySound.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "BatteryLowPlaySound", cb_SettingsBatteryLowPlaySound.IsChecked.ToString());

                    //Check all controllers for low battery level
                    await CheckAllControllersLowBattery(true);
                };

                //Controller settings
                slider_ControllerIdleDisconnectMin.ValueChanged += (sender, e) =>
                {
                    string controllerIdleDisconnectMinString = slider_ControllerIdleDisconnectMin.Value.ToString();
                    Setting_Save(vConfigurationDirectXInput, "ControllerIdleDisconnectMin", controllerIdleDisconnectMinString);
                    textblock_ControllerIdleDisconnectMin.Text = textblock_ControllerIdleDisconnectMin.Tag + ": " + controllerIdleDisconnectMinString + " minutes";
                };

                colorpicker_Controller0.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        Setting_Save(vConfigurationDirectXInput, "ControllerColor0", newBrush.ToString());
                        colorpicker_Controller0.Background = newBrush;
                        vController0.Color = newBrush.Color;
                        if (vController0 == vActiveController())
                        {
                            txt_ActiveControllerName.Foreground = newBrush;
                        }

                        //Controller update led color
                        ControllerLedColor(vController0);
                        await NotifyCtrlUISettingChanged("ControllerColor");
                    }
                };

                colorpicker_Controller1.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        Setting_Save(vConfigurationDirectXInput, "ControllerColor1", newBrush.ToString());
                        colorpicker_Controller1.Background = newBrush;
                        vController1.Color = newBrush.Color;
                        if (vController1 == vActiveController())
                        {
                            txt_ActiveControllerName.Foreground = newBrush;
                        }

                        //Controller update led color
                        ControllerLedColor(vController1);
                        await NotifyCtrlUISettingChanged("ControllerColor");
                    }
                };

                colorpicker_Controller2.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        Setting_Save(vConfigurationDirectXInput, "ControllerColor2", newBrush.ToString());
                        colorpicker_Controller2.Background = newBrush;
                        vController2.Color = newBrush.Color;
                        if (vController2 == vActiveController())
                        {
                            txt_ActiveControllerName.Foreground = newBrush;
                        }

                        //Controller update led color
                        ControllerLedColor(vController2);
                        await NotifyCtrlUISettingChanged("ControllerColor");
                    }
                };

                colorpicker_Controller3.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        Setting_Save(vConfigurationDirectXInput, "ControllerColor3", newBrush.ToString());
                        colorpicker_Controller3.Background = newBrush;
                        vController3.Color = newBrush.Color;
                        if (vController3 == vActiveController())
                        {
                            txt_ActiveControllerName.Foreground = newBrush;
                        }

                        //Controller update led color
                        ControllerLedColor(vController3);
                        await NotifyCtrlUISettingChanged("ControllerColor");
                    }
                };

                //Shortcut settings
                cb_SettingsShortcutLaunchCtrlUI.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI", cb_SettingsShortcutLaunchCtrlUI.IsChecked.ToString());
                };

                cb_SettingsShortcutKeyboardPopup.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutKeyboardPopup", cb_SettingsShortcutKeyboardPopup.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutMediaPopup.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutMediaPopup", cb_SettingsShortcutMediaPopup.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltEnter.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutAltEnter", cb_SettingsShortcutAltEnter.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltTab.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutAltTab", cb_SettingsShortcutAltTab.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutScreenshot.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutScreenshot", cb_SettingsShortcutScreenshot.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                //Keyboard settings
                slider_KeyboardOpacity.ValueChanged += (sender, e) =>
                {
                    textblock_KeyboardOpacity.Text = textblock_KeyboardOpacity.Tag + ": " + slider_KeyboardOpacity.Value.ToString("0.00") + "%";
                    Setting_Save(vConfigurationDirectXInput, "KeyboardOpacity", slider_KeyboardOpacity.Value.ToString("0.00"));
                    App.vWindowKeyboard.UpdatePopupOpacity();
                };

                cb_SettingsKeyboardCloseNoController.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardCloseNoController", cb_SettingsKeyboardCloseNoController.IsChecked.ToString());
                };

                cb_SettingsKeyboardResetPosition.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardResetPosition", cb_SettingsKeyboardResetPosition.IsChecked.ToString());
                };

                combobox_KeyboardLayout.SelectionChanged += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardLayout", combobox_KeyboardLayout.SelectedIndex.ToString());
                    App.vWindowKeyboard.UpdateKeyboardLayout();
                };

                slider_SettingsKeyboardMouseMoveSensitivity.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsKeyboardMouseMoveSensitivity.Text = textblock_SettingsKeyboardMouseMoveSensitivity.Tag.ToString() + slider_SettingsKeyboardMouseMoveSensitivity.Value.ToString("0.00");
                    Setting_Save(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity", slider_SettingsKeyboardMouseMoveSensitivity.Value.ToString("0.00"));
                };

                slider_SettingsKeyboardMouseScrollSensitivity2.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsKeyboardMouseScrollSensitivity2.Text = textblock_SettingsKeyboardMouseScrollSensitivity2.Tag.ToString() + slider_SettingsKeyboardMouseScrollSensitivity2.Value.ToString();
                    Setting_Save(vConfigurationDirectXInput, "KeyboardMouseScrollSensitivity2", slider_SettingsKeyboardMouseScrollSensitivity2.Value.ToString());
                };

                //Keypad settings
                slider_KeypadOpacity.ValueChanged += (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.KeypadOpacity = slider_KeypadOpacity.Value;

                    //Save changes to Json file
                    JsonSaveObject(selectedProfile, GenerateJsonNameKeypadMapping(selectedProfile));

                    textblock_KeypadOpacity.Text = textblock_KeypadOpacity.Tag + ": " + slider_KeypadOpacity.Value.ToString("0.00") + "%";
                    App.vWindowKeypad.UpdatePopupOpacity();
                };

                combobox_KeypadDisplayStyle.SelectionChanged += (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.KeypadDisplayStyle = combobox_KeypadDisplayStyle.SelectedIndex;

                    //Save changes to Json file
                    JsonSaveObject(selectedProfile, GenerateJsonNameKeypadMapping(selectedProfile));

                    App.vWindowKeypad.UpdateKeypadStyle();
                };

                slider_KeypadDisplaySize.ValueChanged += async (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.KeypadDisplaySize = Convert.ToInt32(slider_KeypadDisplaySize.Value);

                    //Save changes to Json file
                    JsonSaveObject(selectedProfile, GenerateJsonNameKeypadMapping(selectedProfile));

                    textblock_KeypadDisplaySize.Text = textblock_KeypadDisplaySize.Tag + ": " + selectedProfile.KeypadDisplaySize + "%";

                    //Update the keypad size
                    double keypadHeight = App.vWindowKeypad.UpdateKeypadSize();

                    //Notify - Fps Overlayer keypad size changed
                    await NotifyFpsOverlayerKeypadSizeChanged(Convert.ToInt32(keypadHeight));
                };

                slider_KeypadRepeatIntervalMs.ValueChanged += (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.ButtonDelayRepeatMs = Convert.ToInt32(slider_KeypadRepeatIntervalMs.Value);

                    //Save changes to Json file
                    JsonSaveObject(selectedProfile, GenerateJsonNameKeypadMapping(selectedProfile));

                    textblock_KeypadRepeatIntervalMs.Text = textblock_KeypadRepeatIntervalMs.Tag + ": " + selectedProfile.ButtonDelayRepeatMs + "ms";
                };

                cb_SettingsKeypadMouseMoveEnabled.Click += (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.KeypadMouseMoveEnabled = (bool)cb_SettingsKeypadMouseMoveEnabled.IsChecked;

                    //Save changes to Json file
                    JsonSaveObject(selectedProfile, GenerateJsonNameKeypadMapping(selectedProfile));

                    //Update all keypad key names
                    App.vWindowKeypad.UpdateKeypadNames();
                };

                slider_SettingsKeypadMouseMoveSensitivity.ValueChanged += (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.KeypadMouseMoveSensitivity = slider_SettingsKeypadMouseMoveSensitivity.Value;

                    //Save changes to Json file
                    JsonSaveObject(selectedProfile, GenerateJsonNameKeypadMapping(selectedProfile));

                    textblock_SettingsKeypadMouseMoveSensitivity.Text = textblock_SettingsKeypadMouseMoveSensitivity.Tag + ": " + selectedProfile.KeypadMouseMoveSensitivity.ToString("0.00");
                };

                //Media settings
                combobox_ShortcutMuteFunction.SelectionChanged += (sender, e) =>
                {
                    Setting_Save(vConfigurationDirectXInput, "ShortcutMuteFunction", combobox_ShortcutMuteFunction.SelectedIndex.ToString());
                };

                slider_SettingsMediaVolumeStep.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsMediaVolumeStep.Text = textblock_SettingsMediaVolumeStep.Tag.ToString() + slider_SettingsMediaVolumeStep.Value.ToString();
                    Setting_Save(vConfigurationDirectXInput, "MediaVolumeStep", slider_SettingsMediaVolumeStep.Value.ToString());
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}