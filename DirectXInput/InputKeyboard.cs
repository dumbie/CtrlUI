using System.Collections.Generic;
using System.Diagnostics;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.ControllerTimings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private async void EventHotKeyPressed(List<KeysVirtual> keysPressed)
        {
            try
            {
                if (GetSystemTicksMs() >= vDelayKeyboardShortcut)
                {
                    bool shortcutDelay = false;
                    bool altPressed = keysPressed.Contains(KeysVirtual.AltLeft);
                    bool ctrlPressed = keysPressed.Contains(KeysVirtual.ControlLeft);
                    bool shiftPressed = keysPressed.Contains(KeysVirtual.ShiftLeft);
                    bool windowsPressed = keysPressed.Contains(KeysVirtual.WindowsLeft);
                    bool modifierKeyPressed = altPressed || ctrlPressed || shiftPressed || windowsPressed;

                    if (windowsPressed && keysPressed.Contains(KeysVirtual.OEMTilde))
                    {
                        //Launch or show CtrlUI
                        if (SettingLoad(vConfigurationDirectXInput, "ShortcutLaunchCtrlUIKeyboard", typeof(bool)))
                        {
                            Debug.WriteLine("Button Global - Show or hide CtrlUI");
                            await ToolFunctions.CtrlUI_LaunchShow();
                            shortcutDelay = true;
                        }
                    }

                    //Update shortcut delay
                    if (shortcutDelay)
                    {
                        vDelayKeyboardShortcut = GetSystemTicksMs() + vControllerDelayTicks500;
                    }
                }
            }
            catch { }
        }
    }
}