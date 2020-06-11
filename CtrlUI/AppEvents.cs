using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;

namespace CtrlUI
{
    public partial class WindowMain
    {
        //Application events
        public delegate Task DelegateKeyboardPressSingle(KeysVirtual virtualKey, IntPtr windowHandle);
        public DelegateKeyboardPressSingle EventKeyboardPressSingle = null;
        public delegate Task DelegateKeyboardPressCombo(KeysVirtual modifierKey, KeysVirtual virtualKey);
        public DelegateKeyboardPressCombo EventKeyboardPressCombo = null;

        //Register application events
        void RegisterApplicationEvents()
        {
            try
            {
                EventKeyboardPressSingle += KeySendSingle;
                EventKeyboardPressCombo += KeyPressComboAuto;
            }
            catch { }
        }
    }
}