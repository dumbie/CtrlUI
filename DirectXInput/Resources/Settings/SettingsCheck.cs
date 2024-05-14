using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
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
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutKeyboardPopup")) { SettingSave(vConfigurationDirectXInput, "ShortcutKeyboardPopup", "True"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutAltEnter")) { SettingSave(vConfigurationDirectXInput, "ShortcutAltEnter", "True"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutAltTab")) { SettingSave(vConfigurationDirectXInput, "ShortcutAltTab", "True"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutCtrlAltDelete")) { SettingSave(vConfigurationDirectXInput, "ShortcutCtrlAltDelete", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutMuteOutput")) { SettingSave(vConfigurationDirectXInput, "ShortcutMuteOutput", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutMuteInput")) { SettingSave(vConfigurationDirectXInput, "ShortcutMuteInput", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutCaptureImage")) { SettingSave(vConfigurationDirectXInput, "ShortcutCaptureImage", "True"); } //Shared
                if (!SettingCheck(vConfigurationDirectXInput, "ShortcutCaptureVideo")) { SettingSave(vConfigurationDirectXInput, "ShortcutCaptureVideo", "True"); } //Shared

                //Battery settings
                if (!SettingCheck(vConfigurationDirectXInput, "BatteryLowLevel")) { SettingSave(vConfigurationDirectXInput, "BatteryLowLevel", "20"); }
                if (!SettingCheck(vConfigurationDirectXInput, "BatteryLowBlinkLed")) { SettingSave(vConfigurationDirectXInput, "BatteryLowBlinkLed", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "BatteryLowShowNotification")) { SettingSave(vConfigurationDirectXInput, "BatteryLowShowNotification", "True"); }
                if (!SettingCheck(vConfigurationDirectXInput, "BatteryLowPlaySound")) { SettingSave(vConfigurationDirectXInput, "BatteryLowPlaySound", "True"); }

                //Controller settings
                if (!SettingCheck(vConfigurationDirectXInput, "ControllerIdleDisconnectMin")) { SettingSave(vConfigurationDirectXInput, "ControllerIdleDisconnectMin", "10"); }
                if (!SettingCheck(vConfigurationDirectXInput, "ControllerLedCondition")) { SettingSave(vConfigurationDirectXInput, "ControllerLedCondition", "0"); }
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
                if (!SettingCheck(vConfigurationDirectXInput, "MediaVolumeStep")) { SettingSave(vConfigurationDirectXInput, "MediaVolumeStep", "2"); }

                //Socket settings
                if (!SettingCheck(vConfigurationDirectXInput, "ServerPortScreenCaptureTool")) { SettingSave(vConfigurationDirectXInput, "ServerPortScreenCaptureTool", "1040"); }

                //Check hotkey settings
                if (!SettingCheck(vConfigurationDirectXInput, "Hotkey0LaunchCtrlUI")) { SettingSave(vConfigurationDirectXInput, "Hotkey0LaunchCtrlUI", (byte)KeysVirtual.WindowsLeft); }
                if (!SettingCheck(vConfigurationDirectXInput, "Hotkey1LaunchCtrlUI")) { SettingSave(vConfigurationDirectXInput, "Hotkey1LaunchCtrlUI", (byte)KeysVirtual.None); }
                if (!SettingCheck(vConfigurationDirectXInput, "Hotkey2LaunchCtrlUI")) { SettingSave(vConfigurationDirectXInput, "Hotkey2LaunchCtrlUI", (byte)KeysVirtual.Tilde); }

                //Set hotkey settings
                hotkey_LaunchCtrlUI.Configuration = vConfigurationDirectXInput;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}