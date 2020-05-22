using System;
using System.Configuration;
using System.Diagnostics;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Check - Application Settings
        void Settings_Check()
        {
            try
            {
                if (ConfigurationManager.AppSettings["AppFirstLaunch"] == null) { SettingSave(vConfigurationApplication, "AppFirstLaunch", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutDisconnectBluetooth"] == null) { SettingSave(vConfigurationApplication, "ShortcutDisconnectBluetooth", "True"); }
                if (ConfigurationManager.AppSettings["ExclusiveGuide"] == null) { SettingSave(vConfigurationApplication, "ExclusiveGuide", "True"); }

                if (ConfigurationManager.AppSettings["ShortcutLaunchCtrlUI"] == null) { SettingSave(vConfigurationApplication, "ShortcutLaunchCtrlUI", "True"); }
                if (ConfigurationManager.AppSettings["ShortcutLaunchKeyboardController"] == null) { SettingSave(vConfigurationApplication, "ShortcutLaunchKeyboardController", "True"); } //Shared
                if (ConfigurationManager.AppSettings["ShortcutAltEnter"] == null) { SettingSave(vConfigurationApplication, "ShortcutAltEnter", "True"); } //Shared
                if (ConfigurationManager.AppSettings["ShortcutAltF4"] == null) { SettingSave(vConfigurationApplication, "ShortcutAltF4", "True"); } //Shared
                if (ConfigurationManager.AppSettings["ShortcutAltTab"] == null) { SettingSave(vConfigurationApplication, "ShortcutAltTab", "True"); } //Shared
                if (ConfigurationManager.AppSettings["ShortcutWinTab"] == null) { SettingSave(vConfigurationApplication, "ShortcutWinTab", "False"); } //Shared
                if (ConfigurationManager.AppSettings["ShortcutScreenshot"] == null) { SettingSave(vConfigurationApplication, "ShortcutScreenshot", "True"); } //Shared

                if (ConfigurationManager.AppSettings["InterfaceSound"] == null) { SettingSave(vConfigurationApplication, "InterfaceSound", "True"); }
                if (ConfigurationManager.AppSettings["InterfaceSoundVolume"] == null) { SettingSave(vConfigurationApplication, "InterfaceSoundVolume", "80"); }
                if (ConfigurationManager.AppSettings["InterfaceSoundPackName"] == null) { SettingSave(vConfigurationApplication, "InterfaceSoundPackName", "Default"); }

                if (ConfigurationManager.AppSettings["BatteryShowIconLow"] == null) { SettingSave(vConfigurationApplication, "BatteryShowIconLow", "True"); }
                if (ConfigurationManager.AppSettings["BatteryShowPercentageLow"] == null) { SettingSave(vConfigurationApplication, "BatteryShowPercentageLow", "False"); }
                if (ConfigurationManager.AppSettings["BatteryPlaySoundLow"] == null) { SettingSave(vConfigurationApplication, "BatteryPlaySoundLow", "True"); }

                //Keyboard settings
                if (ConfigurationManager.AppSettings["KeyboardLayout"] == null) { SettingSave(vConfigurationApplication, "KeyboardLayout", "0"); }
                if (ConfigurationManager.AppSettings["KeyboardMode"] == null) { SettingSave(vConfigurationApplication, "KeyboardMode", "0"); }
                if (ConfigurationManager.AppSettings["KeyboardOpacity"] == null) { SettingSave(vConfigurationApplication, "KeyboardOpacity", "0,95"); }
                if (ConfigurationManager.AppSettings["KeyboardDomainExtension"] == null) { SettingSave(vConfigurationApplication, "KeyboardDomainExtension", ".nl"); }
                if (ConfigurationManager.AppSettings["MouseMoveSensitivity"] == null) { SettingSave(vConfigurationApplication, "MouseMoveSensitivity", "10"); }
                if (ConfigurationManager.AppSettings["MouseScrollSensitivity"] == null) { SettingSave(vConfigurationApplication, "MouseScrollSensitivity", "12"); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}