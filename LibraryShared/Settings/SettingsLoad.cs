using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace LibraryShared
{
    public partial class Settings
    {
        //Load - Application Setting Value
        public static object Setting_Load(Configuration sourceConfig, string settingName)
        {
            try
            {
                return sourceConfig.AppSettings.Settings[settingName].Value;
            }
            catch
            {
                return null;
            }
        }

        //Load - Accent Color settings
        public static void Settings_Load_AccentColor(Configuration sourceConfig)
        {
            try
            {
                Debug.WriteLine("Adjusting the application accent color.");

                string colorHexLight = Convert.ToString(Setting_Load(sourceConfig, "ColorAccentLight"));

                SolidColorBrush targetSolidColorBrushLight = new BrushConverter().ConvertFrom(colorHexLight) as SolidColorBrush;
                SolidColorBrush targetSolidColorBrushDark = new BrushConverter().ConvertFrom(colorHexLight) as SolidColorBrush;
                targetSolidColorBrushDark.Opacity = 0.50;

                Application.Current.Resources["ApplicationAccentLightColor"] = targetSolidColorBrushLight.Color;
                Application.Current.Resources["ApplicationAccentDarkColor"] = targetSolidColorBrushDark.Color;
                Application.Current.Resources["ApplicationAccentLightBrush"] = targetSolidColorBrushLight;
                Application.Current.Resources["ApplicationAccentDarkBrush"] = targetSolidColorBrushDark;
            }
            catch { }
        }

        //Load - CtrlUI Settings
        public static Configuration Settings_Load_CtrlUI()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.csettings";
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                Debug.WriteLine("Loaded the CtrlUI settings.");
                return configuration;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the CtrlUI settings: " + ex.Message);
                return null;
            }
        }

        //Load - DirectXInput Settings
        public static Configuration Settings_Load_DirectXInput()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "DirectXInput.exe.csettings";
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                Debug.WriteLine("Loaded the DirectXInput settings.");
                return configuration;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the DirectXInput settings: " + ex.Message);
                return null;
            }
        }

        //Load - Fps Overlayer Settings
        public static Configuration Settings_Load_FpsOverlayer()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "FpsOverlayer.exe.csettings";
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                Debug.WriteLine("Loaded the Fps Overlayer settings.");
                return configuration;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the Fps Overlayer settings: " + ex.Message);
                return null;
            }
        }
    }
}