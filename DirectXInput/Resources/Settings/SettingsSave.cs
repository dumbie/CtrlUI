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

                cb_SettingsShortcutLaunchCtrlUI.Click += (sender, e) =>
                {
                    SettingSave("ShortcutLaunchCtrlUI", cb_SettingsShortcutLaunchCtrlUI.IsChecked.ToString());
                };

                cb_SettingsPlaySoundBatteryLow.Click += (sender, e) =>
                {
                    SettingSave("PlaySoundBatteryLow", cb_SettingsPlaySoundBatteryLow.IsChecked.ToString());
                };

                cb_SettingsShortcutLaunchKeyboardController.Click += (sender, e) =>
                {
                    SettingSave("ShortcutLaunchKeyboardController", cb_SettingsShortcutLaunchKeyboardController.IsChecked.ToString());
                };

                cb_SettingsWindowsStartup.Click += (sender, e) => { ManageShortcutStartup(); };
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