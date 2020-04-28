using System;
using System.Configuration;
using System.Diagnostics;

namespace KeyboardController
{
    public partial class WindowSettings
    {
        //Load - Application Settings
        bool Settings_Load()
        {
            try
            {
                textblock_KeyboardOpacity.Text = textblock_KeyboardOpacity.Tag + ": " + ConfigurationManager.AppSettings["KeyboardOpacity"].ToString() + "%";
                slider_KeyboardOpacity.Value = Convert.ToDouble(ConfigurationManager.AppSettings["KeyboardOpacity"]);

                combobox_KeyboardLayout.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardLayout"]);
                cb_SettingsInterfaceSound.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["InterfaceSound"]);

                //Load the sound volume
                textblock_SettingsSoundVolume.Text = "User interface sound volume: " + Convert.ToInt32(ConfigurationManager.AppSettings["InterfaceSoundVolume"]) + "%";
                slider_SettingsSoundVolume.Value = Convert.ToInt32(ConfigurationManager.AppSettings["InterfaceSoundVolume"]);

                //Load the domain extension
                textbox_SettingsDomainExtension.Text = ConfigurationManager.AppSettings["DomainExtension"].ToString();

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