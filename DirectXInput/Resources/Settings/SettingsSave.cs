using System;
using System.Configuration;
using System.Diagnostics;
using static DirectXInput.AppVariables;

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
                    SettingSave("ShortcutDisconnectBluetooth", cb_SettingsShortcutDisconnectBluetooth.IsChecked.ToString());
                };

                cb_SettingsExclusiveGuide.Click += (sender, e) =>
                {
                    SettingSave("ExclusiveGuide", cb_SettingsExclusiveGuide.IsChecked.ToString());
                };

                cb_SettingsWindowsStartup.Click += (sender, e) => { ManageShortcutStartup(); };

                //Battery settings
                cb_SettingsBatteryShowIconLow.Click += (sender, e) =>
                {
                    SettingSave("BatteryShowIconLow", cb_SettingsBatteryShowIconLow.IsChecked.ToString());

                    //Check all controllers for low battery level
                    CheckAllControllersLowBattery();
                };

                cb_SettingsBatteryShowPercentageLow.Click += (sender, e) =>
                {
                    SettingSave("BatteryShowPercentageLow", cb_SettingsBatteryShowPercentageLow.IsChecked.ToString());

                    //Check all controllers for low battery level
                    CheckAllControllersLowBattery();
                };

                cb_SettingsBatteryPlaySoundLow.Click += (sender, e) =>
                {
                    SettingSave("BatteryPlaySoundLow", cb_SettingsBatteryPlaySoundLow.IsChecked.ToString());
                };

                //Shortcut settings
                cb_SettingsShortcutLaunchCtrlUI.Click += (sender, e) =>
                {
                    SettingSave("ShortcutLaunchCtrlUI", cb_SettingsShortcutLaunchCtrlUI.IsChecked.ToString());
                };

                cb_SettingsShortcutLaunchKeyboardController.Click += async (sender, e) =>
                {
                    SettingSave("ShortcutLaunchKeyboardController", cb_SettingsShortcutLaunchKeyboardController.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltEnter.Click += async (sender, e) =>
                {
                    SettingSave("ShortcutAltEnter", cb_SettingsShortcutAltEnter.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltF4.Click += async (sender, e) =>
                {
                    SettingSave("ShortcutAltF4", cb_SettingsShortcutAltF4.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutAltTab.Click += async (sender, e) =>
                {
                    SettingSave("ShortcutAltTab", cb_SettingsShortcutAltTab.IsChecked.ToString());
                    if (cb_SettingsShortcutAltTab.IsChecked == true)
                    {
                        SettingSave("ShortcutWinTab", "False");
                        cb_SettingsShortcutWinTab.IsChecked = false;
                    }
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutWinTab.Click += async (sender, e) =>
                {
                    SettingSave("ShortcutWinTab", cb_SettingsShortcutWinTab.IsChecked.ToString());
                    if (cb_SettingsShortcutWinTab.IsChecked == true)
                    {
                        SettingSave("ShortcutAltTab", "False");
                        cb_SettingsShortcutAltTab.IsChecked = false;
                    }
                    await NotifyCtrlUISettingChanged("Shortcut");
                };

                cb_SettingsShortcutScreenshot.Click += async (sender, e) =>
                {
                    SettingSave("ShortcutScreenshot", cb_SettingsShortcutScreenshot.IsChecked.ToString());
                    await NotifyCtrlUISettingChanged("Shortcut");
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }

        //Save - Application Setting
        void SettingSave(string Name, string Value)
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