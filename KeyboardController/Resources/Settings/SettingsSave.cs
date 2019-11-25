using System;
using System.Configuration;
using System.Diagnostics;
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
                    SettingSave("SoundVolume", Convert.ToInt32(slider_SettingsSoundVolume.Value).ToString());
                    vInterfaceSoundVolume = (double)Convert.ToInt32(ConfigurationManager.AppSettings["SoundVolume"]) / 100;
                };
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + Ex.Message);
            }
        }

        //Save - Application Setting
        public static void SettingSave(string Name, string Value)
        {
            try
            {
                vConfiguration.AppSettings.Settings.Remove(Name);
                vConfiguration.AppSettings.Settings.Add(Name, Value);
                vConfiguration.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }
    }
}