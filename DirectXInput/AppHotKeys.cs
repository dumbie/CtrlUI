using System.Diagnostics;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVInputOutputHotkeyHook;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private async void EventHotkeyPressed(bool[] keysPressed)
        {
            try
            {
                foreach (ShortcutTriggerKeyboard shortcutTrigger in vShortcutsKeyboard)
                {
                    if (shortcutTrigger.Name == "LaunchCtrlUI")
                    {
                        if (CheckHotkeyPressed(keysPressed, shortcutTrigger.Trigger))
                        {
                            Debug.WriteLine("Button Global - Show or hide CtrlUI");
                            await ToolFunctions.CtrlUI_LaunchShow();
                            return;
                        }
                    }
                }
            }
            catch { }
        }
    }
}