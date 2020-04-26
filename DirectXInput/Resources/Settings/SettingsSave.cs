using System.Configuration;
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

                cb_SettingsBatteryPlaySoundLow.Click += (sender, e) =>
                {
                    SettingSave("BatteryPlaySoundLow", cb_SettingsBatteryPlaySoundLow.IsChecked.ToString());
                };

                //Shortcut settings
                cb_SettingsShortcutLaunchCtrlUI.Click += (sender, e) =>
                {
                    SettingSave("ShortcutLaunchCtrlUI", cb_SettingsShortcutLaunchCtrlUI.IsChecked.ToString());
                };

                cb_SettingsShortcutLaunchKeyboardController.Click += (sender, e) =>
                {
                    SettingSave("ShortcutLaunchKeyboardController", cb_SettingsShortcutLaunchKeyboardController.IsChecked.ToString());
                };

                cb_SettingsShortcutAltEnter.Click += (sender, e) =>
                {
                    SettingSave("ShortcutAltEnter", cb_SettingsShortcutAltEnter.IsChecked.ToString());
                };

                cb_SettingsShortcutAltF4.Click += (sender, e) =>
                {
                    SettingSave("ShortcutAltF4", cb_SettingsShortcutAltF4.IsChecked.ToString());
                };

                cb_SettingsShortcutAltTab.Click += (sender, e) =>
                {
                    SettingSave("ShortcutAltTab", cb_SettingsShortcutAltTab.IsChecked.ToString());
                    if (cb_SettingsShortcutAltTab.IsChecked == true)
                    {
                        SettingSave("ShortcutWinTab", "False");
                        cb_SettingsShortcutWinTab.IsChecked = false;
                    }
                };

                cb_SettingsShortcutWinTab.Click += (sender, e) =>
                {
                    SettingSave("ShortcutWinTab", cb_SettingsShortcutWinTab.IsChecked.ToString());
                    if (cb_SettingsShortcutWinTab.IsChecked == true)
                    {
                        SettingSave("ShortcutAltTab", "False");
                        cb_SettingsShortcutAltTab.IsChecked = false;
                    }
                };

                cb_SettingsShortcutScreenshot.Click += (sender, e) =>
                {
                    SettingSave("ShortcutScreenshot", cb_SettingsShortcutScreenshot.IsChecked.ToString());
                };
            }
            catch { }
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