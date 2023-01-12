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
                Application.Current.Resources["ApplicationAccentLightColor"] = targetSolidColorBrushLight.Color;
                Application.Current.Resources["ApplicationAccentLightBrush"] = targetSolidColorBrushLight;
                //Debug.WriteLine("Light color: " + targetSolidColorBrushLight.Color);

                SolidColorBrush targetSolidColorBrushDim = AdjustColorBrightness(targetSolidColorBrushLight, 0.80);
                Application.Current.Resources["ApplicationAccentDimColor"] = targetSolidColorBrushDim.Color;
                Application.Current.Resources["ApplicationAccentDimBrush"] = targetSolidColorBrushDim;
                //Debug.WriteLine("Dim color: " + targetSolidColorBrushDim.Color);

                SolidColorBrush targetSolidColorBrushDark = AdjustColorBrightness(targetSolidColorBrushLight, 0.50);
                Application.Current.Resources["ApplicationAccentDarkColor"] = targetSolidColorBrushDark.Color;
                Application.Current.Resources["ApplicationAccentDarkBrush"] = targetSolidColorBrushDark;
                //Debug.WriteLine("Dark color: " + targetSolidColorBrushDark.Color);
            }
            catch { }
        }

        //Adjust the color brightness
        public static SolidColorBrush AdjustColorBrightness(SolidColorBrush solidColorBrush, double brightness)
        {
            try
            {
                Color adjustedColor = Color.FromRgb((byte)(solidColorBrush.Color.R * brightness), (byte)(solidColorBrush.Color.G * brightness), (byte)(solidColorBrush.Color.B * brightness));
                return new SolidColorBrush(adjustedColor);
            }
            catch { }
            return solidColorBrush;
        }

        //Adjust the color opacity
        public static SolidColorBrush AdjustColorOpacity(SolidColorBrush solidColorBrush, double opacity)
        {
            try
            {
                Color adjustedColor = Color.FromArgb((byte)(solidColorBrush.Color.A * opacity), solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B);
                return new SolidColorBrush(adjustedColor);
            }
            catch { }
            return solidColorBrush;
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