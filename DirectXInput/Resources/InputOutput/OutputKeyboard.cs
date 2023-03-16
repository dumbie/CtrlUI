using ArnoldVinkCode;
using System.Windows.Input;
using static DirectXInput.AppVariables;
using static LibraryUsb.FakerInputDevice;

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
                        KeyboardAction keyboardAction = new KeyboardAction()
                        {
                            Key0 = KeyboardKeys.CapsLock
                        };
                        vFakerInputDevice.KeyboardPressRelease(keyboardAction);
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
                        KeyboardAction keyboardAction = new KeyboardAction()
                        {
                            Key0 = KeyboardKeys.NumLock
                        };
                        vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                    }
                });
            }
            catch { }
        }
    }
}