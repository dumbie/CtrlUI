using System.Collections.Generic;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputHotkey;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private async void EventHotkeyPressed(List<KeysVirtual> keysPressed)
        {
            try
            {
                //Check hotkeys
                List<KeysVirtual> usedKeysCaptureImage = new List<KeysVirtual>
                {
                    (KeysVirtual)SettingLoad(vConfigurationDirectXInput, "Hotkey0LaunchCtrlUI", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfigurationDirectXInput, "Hotkey1LaunchCtrlUI", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfigurationDirectXInput, "Hotkey2LaunchCtrlUI", typeof(byte))
                };

                //Check presses
                if (CheckHotkeyPress(keysPressed, usedKeysCaptureImage))
                {
                    Debug.WriteLine("Button Global - Show or hide CtrlUI");
                    await ToolFunctions.CtrlUI_LaunchShow();
                }
            }
            catch { }
        }
    }
}