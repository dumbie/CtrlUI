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
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.CapsLock, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
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
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.NumLock, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                });
            }
            catch { }
        }
    }
}