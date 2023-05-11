using ArnoldVinkCode;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Disable hardware capslock
        public static void DisableHardwareCapsLock()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    if (Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
                    {
                        KeysHidAction KeysHidAction = new KeysHidAction()
                        {
                            Key0 = KeysHid.CapsLock
                        };
                        vFakerInputDevice.KeyboardPressRelease(KeysHidAction);
                    }
                });
            }
            catch { }
        }

        //Enable hardware numlock
        public static void EnableHardwareNumLock()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    if (Keyboard.GetKeyStates(Key.NumLock) != KeyStates.Toggled)
                    {
                        KeysHidAction KeysHidAction = new KeysHidAction()
                        {
                            Key0 = KeysHid.NumpadLock
                        };
                        vFakerInputDevice.KeyboardPressRelease(KeysHidAction);
                    }
                });
            }
            catch { }
        }
    }
}