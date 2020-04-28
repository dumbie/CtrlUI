using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace LibraryShared
{
    public partial class Settings
    {
        //Load - Accent Color settings
        public static void Settings_Load_AccentColor(Configuration sourceConfig)
        {
            try
            {
                Debug.WriteLine("Adjusting the application accent color.");

                string colorHexLight = string.Empty;
                if (sourceConfig != null)
                {
                    colorHexLight = Convert.ToString(sourceConfig.AppSettings.Settings["ColorAccentLight"].Value);
                }
                else
                {
                    colorHexLight = ConfigurationManager.AppSettings["ColorAccentLight"].ToString();
                }

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
        public static bool Settings_Load_CtrlUI(ref Configuration targetConfig)
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";
                targetConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the CtrlUI settings: " + ex.Message);
                return false;
            }
        }

        //Load - DirectXInput Settings
        public static bool Settings_Load_DirectXInput(ref Configuration targetConfig)
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "DirectXInput.exe.Config";
                targetConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the DirectXInput settings: " + ex.Message);
                return false;
            }
        }

        //Load - Fps Overlayer Settings
        public static bool Settings_Load_FpsOverlayer(ref Configuration targetConfig)
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "FpsOverlayer.exe.Config";
                targetConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the Fps Overlayer settings: " + ex.Message);
                return false;
            }
        }
    }
}