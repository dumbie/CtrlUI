using System.Windows.Interop;
using static ArnoldVinkCode.AVInteropDll;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle received filter messages
        void ReceivedFilterMessage(ref MSG WindowMessage, ref bool Handled)
        {
            try
            {
                if (Handled) { return; }
                if (WindowMessage.message == (int)WindowMessages.WM_KEYUP)
                {
                    HandleKeyboardUp(WindowMessage, ref Handled);
                }
                else if (WindowMessage.message == (int)WindowMessages.WM_KEYDOWN)
                {
                    HandleKeyboardDown(WindowMessage, ref Handled);
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