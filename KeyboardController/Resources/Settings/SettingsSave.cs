using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Controls;
using static KeyboardController.AppVariables;

namespace KeyboardController
{
    public partial class WindowSettings
    {
        //Save - Monitor Application Settings
        void Settings_Save()
        {
            try
            {
                slider_KeyboardOpacity.ValueChanged += (sender, e) =>
                {
                    textblock_KeyboardOpacity.Text = textblock_KeyboardOpacity.Tag + ": " + slider_KeyboardOpacity.Value.ToString("0.00") + "%";
                    SettingSave("KeyboardOpacity", slider_KeyboardOpacity.Value.ToString("0.00"));
                    App.vWindowMain.UpdateKeyboardOpacity();
                };

                combobox_KeyboardLayout.SelectionChanged += (sender, e) =>
                {
                    SettingSave("KeyboardLayout", combobox_KeyboardLayout.SelectedIndex.ToString());
                    App.vWindowMain.UpdateKeyboardLayout();
                };

                cb_SettingsInterfaceSound.Click += (sender, e) =>
                {
                    SettingSave("InterfaceSound", cb_SettingsInterfaceSound.IsChecked.ToString());
                };

                slider_SettingsSoundVolume.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsSoundVolume.Text = "User interface sound volume: " + Convert.ToInt32(slider_SettingsSoundVolume.Value) + "%";
                    SettingSave("InterfaceSoundVolume", Convert.ToInt32(slider_SettingsSoundVolume.Value).ToString());
                };

                textbox_SettingsDomainExtension.TextChanged += (sender, e) =>
                {
                    TextBox senderTextBox = (TextBox)sender;
                    if (!string.IsNullOrWhiteSpace(senderTextBox.Text))
                    {
                        SettingSave("DomainExtension", senderTextBox.Text);
                        App.vWindowMain.UpdateDomainExtension();
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }

        //Save - Application Setting
        public static void SettingSave(string Name, string Value)
        {
            try
            {
                vConfigurationApplication.AppSettings.Settings.Remove(Name);
                vConfigurationApplication.AppSettings.Settings.Add(Name, Value);
                vConfigurationApplication.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }
    }
}