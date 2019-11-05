﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Media;
using static KeyboardController.AppVariables;

namespace KeyboardController
{
    public partial class WindowSettings
    {
        //Load - Socket server settings
        public static void Settings_LoadSocket()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                string SocketServerIp = Convert.ToString(config.AppSettings.Settings["SocketClientIp"].Value);
                int SocketServerPort = Convert.ToInt32(config.AppSettings.Settings["SocketClientPort"].Value);

                vSocketServer.vTcpListenerIp = SocketServerIp;
                vSocketServer.vTcpListenerPort = SocketServerPort + 2;
            }
            catch { }
        }

        //Load - Accent Color settings
        public static void Settings_LoadAccentColor()
        {
            try
            {
                Debug.WriteLine("Adjusting the application accent color.");

                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                string colorHexLight = Convert.ToString(config.AppSettings.Settings["ColorAccentLight"].Value);
                SolidColorBrush targetSolidColorBrushLight = new BrushConverter().ConvertFrom(colorHexLight) as SolidColorBrush;

                string colorHexDark = Convert.ToString(config.AppSettings.Settings["ColorAccentDark"].Value);
                SolidColorBrush targetSolidColorBrushDark = new BrushConverter().ConvertFrom(colorHexDark) as SolidColorBrush;

                Color targetColorLight = Color.FromArgb(targetSolidColorBrushLight.Color.A, targetSolidColorBrushLight.Color.R, targetSolidColorBrushLight.Color.G, targetSolidColorBrushLight.Color.B);
                Color targetColorDark = Color.FromArgb(targetSolidColorBrushDark.Color.A, targetSolidColorBrushDark.Color.R, targetSolidColorBrushDark.Color.G, targetSolidColorBrushDark.Color.B);

                App.Current.Resources["ApplicationAccentLightColor"] = targetColorLight;
                App.Current.Resources["ApplicationAccentDarkColor"] = targetColorDark;
                App.Current.Resources["ApplicationAccentLightBrush"] = targetSolidColorBrushLight;
                App.Current.Resources["ApplicationAccentDarkBrush"] = targetSolidColorBrushDark;
            }
            catch { }
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
            }
            catch (Exception Ex) { Debug.WriteLine("Failed to load the application settings: " + Ex.Message); }
        }
    }
}