using System;
using System.Diagnostics;
using System.Windows.Controls;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
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
                    SettingSave(vConfigurationApplication, "ShortcutDisconnectBluetooth", cb_SettingsShortcutDisconnectBluetooth.IsChecked.ToString());
                };

                cb_SettingsExclusiveGuide.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "ExclusiveGuide", cb_SettingsExclusiveGuide.IsChecked.ToString());
                };

                cb_SettingsWindowsStartup.Click += (sender, e) => { ManageShortcutStartup(); };

                //Battery settings
                cb_SettingsBatteryShowIconLow.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "BatteryShowIconLow", cb_SettingsBatteryShowIconLow.IsChecked.ToString());

                    //Check all controllers for low battery level
                    CheckAllControllersLowBattery();
                };

                cb_SettingsBatteryShowPercentageLow.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "BatteryShowPercentageLow", cb_SettingsBatteryShowPercentageLow.IsChecked.ToString());

                    //Check all controllers for low battery level
                    CheckAllControllersLowBattery();
                };

                cb_SettingsBatteryPlaySoundLow.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "BatteryPlaySoundLow", cb_SettingsBatteryPlaySoundLow.IsChecked.ToString());
                };

                //Shortcut settings
                cb_SettingsShortcutLaunchCtrlUI.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "ShortcutLaunchCtrlUI", cb_SettingsShortcutLaunchCtrlUI.IsChecked.ToString());
                };

                cb_SettingsShortcutLaunchKeyboardController.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "ShortcutLaunchKeyboardController", cb_SettingsShortcutLaunchKeyboardController.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltEnter.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "ShortcutAltEnter", cb_SettingsShortcutAltEnter.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltF4.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "ShortcutAltF4", cb_SettingsShortcutAltF4.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltTab.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "ShortcutAltTab", cb_SettingsShortcutAltTab.IsChecked.ToString());
                    if (cb_SettingsShortcutAltTab.IsChecked == true)
                    {
                        SettingSave(vConfigurationApplication, "ShortcutWinTab", "False");
                        cb_SettingsShortcutWinTab.IsChecked = false;
                    }
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutWinTab.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "ShortcutWinTab", cb_SettingsShortcutWinTab.IsChecked.ToString());
                    if (cb_SettingsShortcutWinTab.IsChecked == true)
                    {
                        SettingSave(vConfigurationApplication, "ShortcutAltTab", "False");
                        cb_SettingsShortcutAltTab.IsChecked = false;
                    }
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutScreenshot.Click += async (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "ShortcutScreenshot", cb_SettingsShortcutScreenshot.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                //Keyboard settings
                slider_KeyboardOpacity.ValueChanged += (sender, e) =>
                {
                    textblock_KeyboardOpacity.Text = textblock_KeyboardOpacity.Tag + ": " + slider_KeyboardOpacity.Value.ToString("0.00") + "%";
                    SettingSave(vConfigurationApplication, "KeyboardOpacity", slider_KeyboardOpacity.Value.ToString("0.00"));
                    App.vWindowKeyboard.UpdateKeyboardOpacity(false);
                };

                combobox_KeyboardLayout.SelectionChanged += async (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "KeyboardLayout", combobox_KeyboardLayout.SelectedIndex.ToString());
                    await App.vWindowKeyboard.UpdateKeyboardLayout();
                };

                slider_SettingsMouseMoveSensitivity.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsMouseMoveSensitivity.Text = textblock_SettingsMouseMoveSensitivity.Tag.ToString() + Convert.ToInt32(slider_SettingsMouseMoveSensitivity.Value);
                    SettingSave(vConfigurationApplication, "MouseMoveSensitivity", Convert.ToInt32(slider_SettingsMouseMoveSensitivity.Value).ToString());
                };

                slider_SettingsMouseScrollSensitivity.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsMouseScrollSensitivity.Text = textblock_SettingsMouseScrollSensitivity.Tag.ToString() + Convert.ToInt32(slider_SettingsMouseScrollSensitivity.Value);
                    SettingSave(vConfigurationApplication, "MouseScrollSensitivity", Convert.ToInt32(slider_SettingsMouseScrollSensitivity.Value).ToString());
                };

                textbox_SettingsDomainExtension.TextChanged += (sender, e) =>
                {
                    TextBox senderTextBox = (TextBox)sender;
                    if (!string.IsNullOrWhiteSpace(senderTextBox.Text))
                    {
                        SettingSave(vConfigurationApplication, "KeyboardDomainExtension", senderTextBox.Text);
                        App.vWindowKeyboard.UpdateDomainExtension();
                    }
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

                slider_KeypadRepeatIntervalMs.ValueChanged += (sender, e) =>
                {
                    KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                    selectedProfile.ButtonRepeatIntervalMs = Convert.ToInt32(slider_KeypadRepeatIntervalMs.Value);

                    //Save changes to Json file
                    JsonSaveObject(vDirectKeypadMapping, "DirectKeypadMapping");

                    textblock_KeypadRepeatIntervalMs.Text = textblock_KeypadRepeatIntervalMs.Tag + ": " + Convert.ToInt32(slider_KeypadRepeatIntervalMs.Value) + "ms";
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}