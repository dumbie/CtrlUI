using Microsoft.Win32;
using System;
using System.Diagnostics;
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
                //Get alternate shortcut keys
                KeysVirtual keysVirtual = KeysVirtual.None;
                KeysModifierVirtual keysModifier = KeysModifierVirtual.None;
                using (RegistryKey registryKeyBase = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey registryKeySub = registryKeyBase.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR"))
                    {
                        keysVirtual = (KeysVirtual)Convert.ToByte(registryKeySub.GetValue("VKToggleGameBar"));
                        keysModifier = (KeysModifierVirtual)Convert.ToByte(registryKeySub.GetValue("VKMToggleGameBar"));
                    }
                }

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
                //Get alternate shortcut keys
                KeysVirtual keysVirtual = KeysVirtual.None;
                KeysModifierVirtual keysModifier = KeysModifierVirtual.None;
                using (RegistryKey registryKeyBase = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey registryKeySub = registryKeyBase.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR"))
                    {
                        keysVirtual = (KeysVirtual)Convert.ToByte(registryKeySub.GetValue("VKTakeScreenshot"));
                        keysModifier = (KeysModifierVirtual)Convert.ToByte(registryKeySub.GetValue("VKMTakeScreenshot"));
                    }
                }

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
                //Get alternate shortcut keys
                KeysVirtual keysVirtual = KeysVirtual.None;
                KeysModifierVirtual keysModifier = KeysModifierVirtual.None;
                using (RegistryKey registryKeyBase = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey registryKeySub = registryKeyBase.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR"))
                    {
                        keysVirtual = (KeysVirtual)Convert.ToByte(registryKeySub.GetValue("VKToggleRecording"));
                        keysModifier = (KeysModifierVirtual)Convert.ToByte(registryKeySub.GetValue("VKMToggleRecording"));
                    }
                }

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