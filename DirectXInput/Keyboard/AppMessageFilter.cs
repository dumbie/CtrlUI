using System.Windows.Interop;
using static ArnoldVinkCode.AVInteropDll;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Handle received filter messages
        void ReceivedFilterMessage(ref MSG windowMessage, ref bool messageHandled)
        {
            try
            {
                if (messageHandled) { return; }
                if (windowMessage.message == (int)WindowMessages.WM_KEYUP || windowMessage.message == (int)WindowMessages.WM_SYSKEYUP)
                {
                    HandleKeyboardUp(windowMessage, ref messageHandled);
                }
                else if (windowMessage.message == (int)WindowMessages.WM_KEYDOWN || windowMessage.message == (int)WindowMessages.WM_SYSKEYDOWN)
                {
                    HandleKeyboardDown(windowMessage, ref messageHandled);
                }
            }
            catch { }
        }
    }
}