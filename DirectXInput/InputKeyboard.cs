using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private async void EventHotKeyPressed(KeysModifier keysModifier, KeysVirtual keysVirtual)
        {
            try
            {
                //Make screenshot hotkey
                if (keysModifier == KeysModifier.Alt && keysVirtual == KeysVirtual.F12)
                {
                    if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutScreenshotKeyboard")))
                    {
                        Debug.WriteLine("Button Global - Screenshot");
                        await CaptureScreen.CaptureScreenToFile();
                    }
                }
            }
            catch { }
        }
    }
}