using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputKeyboard;

namespace CtrlUI
{
    public partial class WindowMain
    {
        //Application events
        public delegate Task DelegateKeyboardPressSingle(byte virtualKey, IntPtr WindowHandle);
        public DelegateKeyboardPressSingle EventKeyboardPressSingle = null;
        public delegate Task DelegateKeyboardPressCombo(byte Modifier, byte virtualKey, bool ExtendedKey);
        public DelegateKeyboardPressCombo EventKeyboardPressCombo = null;

        //Register application events
        void RegisterApplicationEvents()
        {
            try
            {
                EventKeyboardPressSingle += KeySendSingle;
                EventKeyboardPressCombo += KeyPressCombo;
            }
            catch { }
        }
    }
}