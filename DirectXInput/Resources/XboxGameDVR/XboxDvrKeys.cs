using System;
using System.Diagnostics;
using Windows.Media.Capture;
using static ArnoldVinkCode.AVInputOutputClass;

namespace DirectXInput
{
    partial class XboxGameDVR
    {
        //Get Xbox ToggleGameBar keyboard action
        public static KeysHidAction GetKeysHidAction_ToggleGameBar()
        {
            try
            {
                //Get Xbox Game DVR settings
                AppCaptureSettings appCaptureSettings = AppCaptureManager.GetCurrentSettings();

                //Get alternate shortcut keys
                KeysVirtual keysVirtual = (KeysVirtual)appCaptureSettings.AlternateShortcutKeys.ToggleGameBarKey;
                KeysModifierVirtual keysModifier = (KeysModifierVirtual)appCaptureSettings.AlternateShortcutKeys.ToggleGameBarKeyModifiers;

                //Create keyboard action
                KeysHidAction keyboardAction = new KeysHidAction();
                if (keysVirtual == KeysVirtual.None && keysModifier == KeysModifierVirtual.None)
                {
                    keyboardAction.Modifiers = KeysModifierHid.WindowsLeft;
                    keyboardAction.Key0 = KeysHid.G;
                }
                else
                {
                    keyboardAction.Modifiers = ConvertVirtualToKeysModifierHid(keysModifier);
                    keyboardAction.Key0 = ConvertVirtualToKeysHid(keysVirtual);
                }

                //Return result
                return keyboardAction;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Xbox failed to get keys: " + ex.Message);
                return null;
            }
        }

        //Get Xbox TakeScreenshot keyboard action
        public static KeysHidAction GetKeysHidAction_TakeScreenshot()
        {
            try
            {
                //Get Xbox Game DVR settings
                AppCaptureSettings appCaptureSettings = AppCaptureManager.GetCurrentSettings();

                //Enable Xbox Game DVR capture
                appCaptureSettings.IsAppCaptureEnabled = true;

                //Get alternate shortcut keys
                KeysVirtual keysVirtual = (KeysVirtual)appCaptureSettings.AlternateShortcutKeys.TakeScreenshotKey;
                KeysModifierVirtual keysModifier = (KeysModifierVirtual)appCaptureSettings.AlternateShortcutKeys.TakeScreenshotKeyModifiers;

                //Create keyboard action
                KeysHidAction keyboardAction = new KeysHidAction();
                if (keysVirtual == KeysVirtual.None && keysModifier == KeysModifierVirtual.None)
                {
                    keyboardAction.Modifiers = KeysModifierHid.WindowsLeft | KeysModifierHid.AltLeft;
                    keyboardAction.Key0 = KeysHid.PrintScreen;
                }
                else
                {
                    keyboardAction.Modifiers = ConvertVirtualToKeysModifierHid(keysModifier);
                    keyboardAction.Key0 = ConvertVirtualToKeysHid(keysVirtual);
                }

                //Return result
                return keyboardAction;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Xbox failed to get keys: " + ex.Message);
                return null;
            }
        }

        //Get Xbox ToggleRecording keyboard action
        public static KeysHidAction GetKeysHidAction_ToggleRecording()
        {
            try
            {
                //Get Xbox Game DVR settings
                AppCaptureSettings appCaptureSettings = AppCaptureManager.GetCurrentSettings();

                //Enable Xbox Game DVR capture
                appCaptureSettings.IsAppCaptureEnabled = true;

                //Get alternate shortcut keys
                KeysVirtual keysVirtual = (KeysVirtual)appCaptureSettings.AlternateShortcutKeys.ToggleRecordingKey;
                KeysModifierVirtual keysModifier = (KeysModifierVirtual)appCaptureSettings.AlternateShortcutKeys.ToggleRecordingKeyModifiers;

                //Create keyboard action
                KeysHidAction keyboardAction = new KeysHidAction();
                if (keysVirtual == KeysVirtual.None && keysModifier == KeysModifierVirtual.None)
                {
                    keyboardAction.Modifiers = KeysModifierHid.WindowsLeft | KeysModifierHid.AltLeft;
                    keyboardAction.Key0 = KeysHid.R;
                }
                else
                {
                    keyboardAction.Modifiers = ConvertVirtualToKeysModifierHid(keysModifier);
                    keyboardAction.Key0 = ConvertVirtualToKeysHid(keysVirtual);
                }

                //Return result
                return keyboardAction;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Xbox failed to get keys: " + ex.Message);
                return null;
            }
        }
    }
}