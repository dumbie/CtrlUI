using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Media;
using static KeyboardController.AppVariables;

namespace KeyboardController
{
    public partial class WindowSettings
    {
        //Load - Accent Color settings
        public static void Settings_Load_CtrlUI_AccentColor()
        {
            try
            {
                Debug.WriteLine("Adjusting the application accent color.");

                string colorHexLight = Convert.ToString(vConfigurationCtrlUI.AppSettings.Settings["ColorAccentLight"].Value);
                SolidColorBrush targetSolidColorBrushLight = new BrushConverter().ConvertFrom(colorHexLight) as SolidColorBrush;
                SolidColorBrush targetSolidColorBrushDark = new BrushConverter().ConvertFrom(colorHexLight) as SolidColorBrush;
                targetSolidColorBrushDark.Opacity = 0.50;

                App.Current.Resources["ApplicationAccentLightColor"] = targetSolidColorBrushLight.Color;
                App.Current.Resources["ApplicationAccentDarkColor"] = targetSolidColorBrushDark.Color;
                App.Current.Resources["ApplicationAccentLightBrush"] = targetSolidColorBrushLight;
                App.Current.Resources["ApplicationAccentDarkBrush"] = targetSolidColorBrushDark;
            }
            catch { }
        }

        //Load - CtrlUI Settings
        public static void Settings_Load_CtrlUI()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";
                vConfigurationCtrlUI = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("Failed to load the CtrlUI settings: " + Ex.Message);
            }
        }

        //Load - Application Settings
        void Settings_Load()
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
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("Failed to load the application settings: " + Ex.Message);
            }
        }
    }
}