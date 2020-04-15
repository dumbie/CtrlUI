using System.Configuration;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Check - Application Settings
        void Settings_Check()
        {
            try
            {
                if (ConfigurationManager.AppSettings["AppFirstLaunch"] == null) { SettingSave("AppFirstLaunch", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutDisconnectBluetooth"] == null) { SettingSave("ShortcutDisconnectBluetooth", "True"); }
                if (ConfigurationManager.AppSettings["ExclusiveGuide"] == null) { SettingSave("ExclusiveGuide", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutLaunchCtrlUI"] == null) { SettingSave("ShortcutLaunchCtrlUI", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutLaunchKeyboardController"] == null) { SettingSave("ShortcutLaunchKeyboardController", "True"); }

                if (ConfigurationManager.AppSettings["InterfaceSound"] == null) { SettingSave("InterfaceSound", "True"); }
                if (ConfigurationManager.AppSettings["InterfaceSoundVolume"] == null) { SettingSave("InterfaceSoundVolume", "80"); }
                if (ConfigurationManager.AppSettings["InterfaceSoundPackName"] == null) { SettingSave("InterfaceSoundPackName", "Default"); }
                if (ConfigurationManager.AppSettings["PlaySoundBatteryLow"] == null) { SettingSave("PlaySoundBatteryLow", "True"); }
            }
            catch { }
        }
    }
}