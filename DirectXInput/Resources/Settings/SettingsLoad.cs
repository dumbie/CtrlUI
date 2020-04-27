using AVForms;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Load - Accent Color settings
        void Settings_Load_CtrlUI_AccentColor()
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
        async Task Settings_Load_CtrlUI()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";
                vConfigurationCtrlUI = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            }
            catch (Exception Ex)
            {
                await AVMessageBox.MessageBoxPopup(this, "Failed to load the CtrlUI settings.", Ex.Message, "Ok", "", "", "");
            }
        }

        //Load - Application Settings
        async Task Settings_Load()
        {
            try
            {
                cb_SettingsShortcutDisconnectBluetooth.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutDisconnectBluetooth"]);
                cb_SettingsExclusiveGuide.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ExclusiveGuide"]);

                cb_SettingsBatteryPlaySoundLow.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["BatteryPlaySoundLow"]);
                cb_SettingsBatteryShowIconLow.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["BatteryShowIconLow"]);

                cb_SettingsShortcutLaunchCtrlUI.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchCtrlUI"]);
                cb_SettingsShortcutLaunchKeyboardController.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchKeyboardController"]);
                cb_SettingsShortcutAltEnter.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltEnter"]);
                cb_SettingsShortcutAltF4.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltF4"]);
                cb_SettingsShortcutAltTab.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltTab"]);
                cb_SettingsShortcutWinTab.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutWinTab"]);
                cb_SettingsShortcutScreenshot.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutScreenshot"]);

                //Set the application name to string to check shortcuts
                string targetName = Assembly.GetEntryAssembly().GetName().Name;

                //Check if application is set to launch on Windows startup
                string targetFileStartup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");
                if (File.Exists(targetFileStartup))
                {
                    cb_SettingsWindowsStartup.IsChecked = true;
                }
            }
            catch (Exception Ex)
            {
                await AVMessageBox.MessageBoxPopup(this, "Failed to load the application settings.", Ex.Message, "Ok", "", "", "");
            }
        }
    }
}