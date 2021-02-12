using System.Windows.Forms;
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
                //Get the pressed keys
                KeysVirtual usedVirtualKey = (KeysVirtual)windowMessage.wParam;

                //Check pressed key modifier
                KeysVirtual? usedModifierKey = null;
                Keys keysData = (Keys)(int)usedVirtualKey | Control.ModifierKeys;
                if (keysData.HasFlag(Keys.Control)) { usedModifierKey = KeysVirtual.Control; }
                else if (keysData.HasFlag(Keys.Alt)) { usedModifierKey = KeysVirtual.Alt; }
                else if (keysData.HasFlag(Keys.Shift)) { usedModifierKey = KeysVirtual.Shift; }

                //Save keypad button mapping
                messageHandled = KeypadSaveMapping(usedVirtualKey, usedModifierKey);
            }
            catch { }
        }
    }
}