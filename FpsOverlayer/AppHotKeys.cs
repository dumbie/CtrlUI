using System.Collections.Generic;
using System.Diagnostics;
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
                foreach (ShortcutTriggerKeyboard shortcutTrigger in vShortcutTriggers)
                {
                    if (shortcutTrigger.Name == "ShowHideTools")
                    {
                        if (CheckHotkeyPress(keysPressed, shortcutTrigger.Trigger))
                        {
                            Debug.WriteLine("Button Global - ShowHideTools");
                            vWindowTools.SwitchToolsVisibility();
                            return;
                        }
                    }
                    else if (shortcutTrigger.Name == "ShowHideCrosshair")
                    {
                        if (CheckHotkeyPress(keysPressed, shortcutTrigger.Trigger))
                        {
                            Debug.WriteLine("Button Global - ShowHideCrosshair");
                            SwitchCrosshairVisibility(true);
                            return;
                        }
                    }
                    else if (shortcutTrigger.Name == "ShowHideFpsStats")
                    {
                        if (CheckHotkeyPress(keysPressed, shortcutTrigger.Trigger))
                        {
                            Debug.WriteLine("Button Global - ShowHideFpsStats");
                            SwitchFpsOverlayVisibility();
                            return;
                        }
                    }
                    else if (shortcutTrigger.Name == "PositionFpsStats")
                    {
                        if (CheckHotkeyPress(keysPressed, shortcutTrigger.Trigger))
                        {
                            Debug.WriteLine("Button Global - PositionFpsStats");
                            ChangeFpsOverlayPosition();
                            return;
                        }
                    }
                }
            }
            catch { }
        }
    }
}