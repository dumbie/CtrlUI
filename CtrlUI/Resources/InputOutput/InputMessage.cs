using System.Windows.Interop;
using static ArnoldVinkCode.AVInteropDll;

namespace CtrlUI
{
    partial class WindowMain
    {
        void ReceiveInput(ref MSG WindowMessage, ref bool Handled)
        {
            try
            {
                if (Handled) { return; }
                if (WindowMessage.message == (int)WindowMessages.WM_KEYUP)
                {
                    HandleKeyboardUp(ref WindowMessage, ref Handled);
                }
                else if (WindowMessage.message == (int)WindowMessages.WM_KEYDOWN)
                {
                    HandleKeyboardDown(ref WindowMessage, ref Handled);
                }
                else if (WindowMessage.message == (int)WindowMessages.WM_HOTKEY)
                {
                    HandleHotkey(WindowMessage);
                    Handled = true;
                }
            }
            catch { }
        }
    }
}