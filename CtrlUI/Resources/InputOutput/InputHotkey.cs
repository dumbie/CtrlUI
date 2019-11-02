using System.Diagnostics;
using System.Windows.Interop;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle hotkey
        async void HandleHotkey(MSG WindowMessage)
        {
            try
            {
                Debug.WriteLine("Hotkey pressed.");

                //Check the pressed keys
                int UsedVirtualKey = ((int)WindowMessage.lParam >> 16) & 0xFFFF;
                if (UsedVirtualKey == (byte)KeysVirtual.CapsLock) { await AppWindow_HideShow(); }
            }
            catch { }
        }
    }
}