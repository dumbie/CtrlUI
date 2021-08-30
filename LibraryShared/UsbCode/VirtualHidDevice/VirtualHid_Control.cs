using ArnoldVinkCode;
using System.Diagnostics;

namespace LibraryUsb
{
    public partial class VirtualHidDevice
    {
        public bool KeyPressSingle(KeysDDCode ddKey)
        {
            try
            {
                key(ddKey, KeysStatusFlag.Press);
                AVActions.TaskDelayMs(5);
                key(ddKey, KeysStatusFlag.Release);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to press single key.");
                return false;
            }
        }

        public bool KeyPressCombo(KeysDDCode modifierKey, KeysDDCode ddKey)
        {
            try
            {
                key(modifierKey, KeysStatusFlag.Press);
                key(ddKey, KeysStatusFlag.Press);
                AVActions.TaskDelayMs(5);
                key(ddKey, KeysStatusFlag.Release);
                key(modifierKey, KeysStatusFlag.Release);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to press combo keys.");
                return false;
            }
        }
    }
}