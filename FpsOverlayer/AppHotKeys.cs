using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputHotkey;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        private void EventHotkeyPressed(List<KeysVirtual> keysPressed)
        {
            try
            {
                ShortcutTriggerKeyboard shortcutTrigger = vShortcutTriggers.FirstOrDefault(x => x.Name == "ShowHideBrowser");
                if (shortcutTrigger != null)
                {
                    if (CheckHotkeyPress(keysPressed, shortcutTrigger.Trigger))
                    {
                        Debug.WriteLine("Button Global - ShowHideBrowser");
                        vWindowBrowser.Browser_Switch_Visibility();
                        return;
                    }
                }

                shortcutTrigger = vShortcutTriggers.FirstOrDefault(x => x.Name == "ShowHideCrosshair");
                if (shortcutTrigger != null)
                {
                    if (CheckHotkeyPress(keysPressed, shortcutTrigger.Trigger))
                    {
                        Debug.WriteLine("Button Global - ShowHideCrosshair");
                        SwitchCrosshairVisibility(true);
                        return;
                    }
                }

                shortcutTrigger = vShortcutTriggers.FirstOrDefault(x => x.Name == "ShowHideFpsStats");
                if (shortcutTrigger != null)
                {
                    if (CheckHotkeyPress(keysPressed, shortcutTrigger.Trigger))
                    {
                        Debug.WriteLine("Button Global - ShowHideFpsStats");
                        SwitchFpsOverlayVisibility();
                        return;
                    }
                }

                shortcutTrigger = vShortcutTriggers.FirstOrDefault(x => x.Name == "PositionFpsStats");
                if (shortcutTrigger != null)
                {
                    if (CheckHotkeyPress(keysPressed, shortcutTrigger.Trigger))
                    {
                        Debug.WriteLine("Button Global - PositionFpsStats");
                        ChangeFpsOverlayPosition();
                        return;
                    }
                }
            }
            catch { }
        }
    }
}