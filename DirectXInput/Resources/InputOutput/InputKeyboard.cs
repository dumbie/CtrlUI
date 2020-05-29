using System.Windows.Interop;
using static ArnoldVinkCode.AVInputOutputClass;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Handle keyboard up
        void HandleKeyboardUp(MSG windowMessage, ref bool messageHandled)
        {
            try
            {
                //Check the pressed keys
                KeysVirtual usedVirtualKey = (KeysVirtual)windowMessage.wParam;

                //Save keypad button mapping
                messageHandled = KeypadSaveMapping(usedVirtualKey);
            }
            catch { }
        }
    }
}