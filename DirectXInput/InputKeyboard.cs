using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;

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
                    if (SettingLoad(vConfigurationDirectXInput, "ShortcutScreenshotKeyboard", typeof(bool)))
                    {
                        Debug.WriteLine("Button Global - Screenshot");
                        await CaptureScreen.CaptureScreenToFile();
                    }
                }
                else if (keysModifier == KeysModifier.Win && keysVirtual == KeysVirtual.CapsLock)
                {
                    if (SettingLoad(vConfigurationDirectXInput, "ShortcutLaunchCtrlUIKeyboard", typeof(bool)))
                    {
                        Debug.WriteLine("Button Global - Show or hide CtrlUI");
                        await ProcessFunctions.LaunchShowCtrlUI();
                    }
                }
            }
            catch { }
        }
    }
}