using System;
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
                if (Setting_Load(vConfigurationDirectXInput, "AppFirstLaunch") == null) { Setting_Save(vConfigurationDirectXInput, "AppFirstLaunch", "True"); }
                if (Setting_Load(vConfigurationDirectXInput, "ShortcutDisconnectBluetooth") == null) { Setting_Save(vConfigurationDirectXInput, "ShortcutDisconnectBluetooth", "True"); }
                if (Setting_Load(vConfigurationDirectXInput, "ExclusiveGuide") == null) { Setting_Save(vConfigurationDirectXInput, "ExclusiveGuide", "True"); }

                if (Setting_Load(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI") == null) { Setting_Save(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI", "True"); }
                if (Setting_Load(vConfigurationDirectXInput, "ShortcutLaunchKeyboardController") == null) { Setting_Save(vConfigurationDirectXInput, "ShortcutLaunchKeyboardController", "True"); } //Shared
                if (Setting_Load(vConfigurationDirectXInput, "ShortcutAltEnter") == null) { Setting_Save(vConfigurationDirectXInput, "ShortcutAltEnter", "True"); } //Shared
                if (Setting_Load(vConfigurationDirectXInput, "ShortcutAltF4") == null) { Setting_Save(vConfigurationDirectXInput, "ShortcutAltF4", "True"); } //Shared
                if (Setting_Load(vConfigurationDirectXInput, "ShortcutAltTab") == null) { Setting_Save(vConfigurationDirectXInput, "ShortcutAltTab", "True"); } //Shared
                if (Setting_Load(vConfigurationDirectXInput, "ShortcutWinTab") == null) { Setting_Save(vConfigurationDirectXInput, "ShortcutWinTab", "False"); } //Shared
                if (Setting_Load(vConfigurationDirectXInput, "ShortcutScreenshot") == null) { Setting_Save(vConfigurationDirectXInput, "ShortcutScreenshot", "True"); } //Shared

                if (Setting_Load(vConfigurationDirectXInput, "BatteryShowIconLow") == null) { Setting_Save(vConfigurationDirectXInput, "BatteryShowIconLow", "True"); }
                if (Setting_Load(vConfigurationDirectXInput, "BatteryShowPercentageLow") == null) { Setting_Save(vConfigurationDirectXInput, "BatteryShowPercentageLow", "False"); }
                if (Setting_Load(vConfigurationDirectXInput, "BatteryPlaySoundLow") == null) { Setting_Save(vConfigurationDirectXInput, "BatteryPlaySoundLow", "True"); }
                if (Setting_Load(vConfigurationDirectXInput, "ControllerIdleDisconnectMin") == null) { Setting_Save(vConfigurationDirectXInput, "ControllerIdleDisconnectMin", "10"); }

                //Keyboard settings
                if (Setting_Load(vConfigurationDirectXInput, "KeyboardLayout") == null) { Setting_Save(vConfigurationDirectXInput, "KeyboardLayout", "0"); }
                if (Setting_Load(vConfigurationDirectXInput, "KeyboardMode") == null) { Setting_Save(vConfigurationDirectXInput, "KeyboardMode", "0"); }
                if (Setting_Load(vConfigurationDirectXInput, "KeyboardOpacity") == null) { Setting_Save(vConfigurationDirectXInput, "KeyboardOpacity", "0,90"); }
                if (Setting_Load(vConfigurationDirectXInput, "KeyboardResetPosition") == null) { Setting_Save(vConfigurationDirectXInput, "KeyboardResetPosition", "False"); }
                if (Setting_Load(vConfigurationDirectXInput, "KeyboardCloseNoController") == null) { Setting_Save(vConfigurationDirectXInput, "KeyboardCloseNoController", "True"); }
                if (Setting_Load(vConfigurationDirectXInput, "KeyboardDomainExtensionDefault") == null) { Setting_Save(vConfigurationDirectXInput, "KeyboardDomainExtensionDefault", ".com"); }
                if (Setting_Load(vConfigurationDirectXInput, "KeyboardDomainExtension") == null) { Setting_Save(vConfigurationDirectXInput, "KeyboardDomainExtension", ".nl"); }
                if (Setting_Load(vConfigurationDirectXInput, "MouseMoveSensitivity") == null) { Setting_Save(vConfigurationDirectXInput, "MouseMoveSensitivity", "10"); }
                if (Setting_Load(vConfigurationDirectXInput, "MouseScrollSensitivity") == null) { Setting_Save(vConfigurationDirectXInput, "MouseScrollSensitivity", "10"); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}