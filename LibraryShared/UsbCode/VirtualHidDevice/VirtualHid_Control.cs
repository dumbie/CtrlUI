using ArnoldVinkCode;
using System.Diagnostics;

namespace LibraryUsb
{
    public partial class VirtualHidDevice
    {
        //Single key press and release
        public bool KeyPressReleaseSingle(KeysDDCode ddKey)
        {
            try
            {
                key(ddKey, KeysStatusFlag.Press);
                AVActions.TaskDelayMs(40);
                key(ddKey, KeysStatusFlag.Release);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to press and release single key.");
                return false;
            }
        }

        //Single key press or release
        public bool KeyToggleSingle(KeysDDCode ddKey, bool pressKey)
        {
            try
            {
                if (pressKey)
                {
                    key(ddKey, KeysStatusFlag.Press);
                }
                else
                {
                    key(ddKey, KeysStatusFlag.Release);
                }
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to toggle single key.");
                return false;
            }
        }

        //Combo key press and release
        public bool KeyPressReleaseCombo(KeysDDCode modifierKey, KeysDDCode ddKey)
        {
            try
            {
                key(modifierKey, KeysStatusFlag.Press);
                key(ddKey, KeysStatusFlag.Press);
                AVActions.TaskDelayMs(40);
                key(ddKey, KeysStatusFlag.Release);
                key(modifierKey, KeysStatusFlag.Release);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to press and release combo keys.");
                return false;
            }
        }

        //Combo key press or release
        public bool KeyToggleCombo(KeysDDCode modifierKey, KeysDDCode ddKey, bool pressKey)
        {
            try
            {
                if (pressKey)
                {
                    key(modifierKey, KeysStatusFlag.Press);
                    key(ddKey, KeysStatusFlag.Press);
                }
                else
                {
                    key(ddKey, KeysStatusFlag.Release);
                    key(modifierKey, KeysStatusFlag.Release);
                }
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to toggle combo keys.");
                return false;
            }
        }
    }
}