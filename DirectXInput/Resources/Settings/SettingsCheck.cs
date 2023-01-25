using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Check - Application Settings
        void Settings_Check()
        {
            try
            {
                if (!SettingCheck(vConfigurationDirectXInput, "AppFirstLaunch")) { SettingSave(vConfigurationDirectXInput, "AppFirstLaunch", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutDisconnectBluetooth")) { SettingSave(vConfigurationDirectXInput, "ShortcutDisconnectBluetooth", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ExclusiveGuide")) { SettingSave(vConfigurationDirectXInput, "ExclusiveGuide", "True"); }

                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI")) { SettingSave(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutLaunchCtrlUIKeyboard")) { SettingSave(vConfigurationDirectXInput, "ShortcutLaunchCtrlUIKeyboard", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutKeyboardPopup")) { SettingSave(vConfigurationDirectXInput, "ShortcutKeyboardPopup", "True"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutAltEnter")) { SettingSave(vConfigurationDirectXInput, "ShortcutAltEnter", "True"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutAltTab")) { SettingSave(vConfigurationDirectXInput, "ShortcutAltTab", "True"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutCtrlAltDelete")) { SettingSave(vConfigurationDirectXInput, "ShortcutCtrlAltDelete", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutScreenshotController")) { SettingSave(vConfigurationDirectXInput, "ShortcutScreenshotController", "True"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutScreenshotKeyboard")) { SettingSave(vConfigurationDirectXInput, "ShortcutScreenshotKeyboard", "True"); }

                //Battery settings
                if (!SettingCheck(vConfigurationDirectXInput, "BatteryLowLevel")) { SettingSave(vConfigurationDirectXInput, "BatteryLowLevel", "20"); }
                if (!SettingCheck(vConfigurationDirectXInput, "BatteryLowBlinkLed")) { SettingSave(vConfigurationDirectXInput, "BatteryLowBlinkLed", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "BatteryLowShowNotification")) { SettingSave(vConfigurationDirectXInput, "BatteryLowShowNotification", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "BatteryLowPlaySound")) { SettingSave(vConfigurationDirectXInput, "BatteryLowPlaySound", "True"); }

                //Controller settings
                if (!SettingCheck(vConfigurationDirectXInput, "ControllerIdleDisconnectMin")) { SettingSave(vConfigurationDirectXInput, "ControllerIdleDisconnectMin", "10"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ControllerColor0")) { SettingSave(vConfigurationDirectXInput, "ControllerColor0", "#00C7FF"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ControllerColor1")) { SettingSave(vConfigurationDirectXInput, "ControllerColor1", "#F0140A"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ControllerColor2")) { SettingSave(vConfigurationDirectXInput, "ControllerColor2", "#14F00A"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ControllerColor3")) { SettingSave(vConfigurationDirectXInput, "ControllerColor3", "#F0DC0A"); } //Shared

                //Keyboard settings
                if (!SettingCheck(vConfigurationDirectXInput, "KeyboardLayout")) { SettingSave(vConfigurationDirectXInput, "KeyboardLayout", "0"); }
                if (!SettingCheck(vConfigurationDirectXInput, "KeyboardMode")) { SettingSave(vConfigurationDirectXInput, "KeyboardMode", "1"); }
                if (!SettingCheck(vConfigurationDirectXInput, "KeyboardResetPosition")) { SettingSave(vConfigurationDirectXInput, "KeyboardResetPosition", "False"); }
                if (!SettingCheck(vConfigurationDirectXInput, "KeyboardCloseNoController")) { SettingSave(vConfigurationDirectXInput, "KeyboardCloseNoController", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity")) { SettingSave(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity", "7,50"); }
                if (!SettingCheck(vConfigurationDirectXInput, "KeyboardMouseScrollSensitivity2")) { SettingSave(vConfigurationDirectXInput, "KeyboardMouseScrollSensitivity2", "2"); }

                //Media settings
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutMuteFunction")) { SettingSave(vConfigurationDirectXInput, "ShortcutMuteFunction", "0"); }
                if (!SettingCheck(vConfigurationDirectXInput, "MediaVolumeStep")) { SettingSave(vConfigurationDirectXInput, "MediaVolumeStep", "2"); }

                //Screenshot settings
                if (!SettingCheck(vConfigurationDirectXInput, "ScreenshotLocation")) { SettingSave(vConfigurationDirectXInput, "ScreenshotLocation", "Screenshots"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ScreenshotHDRtoSDR")) { SettingSave(vConfigurationDirectXInput, "ScreenshotHDRtoSDR", "False"); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}