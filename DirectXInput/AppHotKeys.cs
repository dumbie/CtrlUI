using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputHotkey;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private async void EventHotkeyPressed(List<KeysVirtual> keysPressed)
        {
            try
            {
                ShortcutTriggerKeyboard shortcutTrigger = vShortcutsKeyboard.FirstOrDefault(x => x.Name == "LaunchCtrlUI");
                if (shortcutTrigger != null)
                {
                    if (CheckHotkeyPress(keysPressed, shortcutTrigger.Trigger))
                    {
                        Debug.WriteLine("Button Global - Show or hide CtrlUI");
                        await ToolFunctions.CtrlUI_LaunchShow();
                        return;
                    }
                }
            }
            catch { }
        }
    }
}