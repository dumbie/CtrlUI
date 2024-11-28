using System.Diagnostics;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVInputOutputHotkeyHook;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class AppHotkeys
    {
        public static void EventHotkeyPressed(bool[] keysPressed)
        {
            try
            {
                foreach (ShortcutTriggerKeyboard shortcutTrigger in vShortcutTriggers)
                {
                    if (shortcutTrigger.Name == "ShowHideTools")
                    {
                        if (CheckHotkeyPressed(keysPressed, shortcutTrigger.Trigger))
                        {
                            Debug.WriteLine("Button Global - ShowHideTools");
                            vWindowTools.SwitchToolsVisibility();
                            return;
                        }
                    }
                    else if (shortcutTrigger.Name == "ShowHideCrosshair")
                    {
                        if (CheckHotkeyPressed(keysPressed, shortcutTrigger.Trigger))
                        {
                            Debug.WriteLine("Button Global - ShowHideCrosshair");
                            vWindowCrosshair.SwitchCrosshairVisibility(true);
                            return;
                        }
                    }
                    else if (shortcutTrigger.Name == "ShowHideFpsStats")
                    {
                        if (CheckHotkeyPressed(keysPressed, shortcutTrigger.Trigger))
                        {
                            Debug.WriteLine("Button Global - ShowHideFpsStats");
                            vWindowStats.SwitchFpsOverlayVisibility();
                            return;
                        }
                    }
                    else if (shortcutTrigger.Name == "PositionFpsStats")
                    {
                        if (CheckHotkeyPressed(keysPressed, shortcutTrigger.Trigger))
                        {
                            Debug.WriteLine("Button Global - PositionFpsStats");
                            vWindowStats.ChangeFpsOverlayPosition();
                            return;
                        }
                    }
                }
            }
            catch { }
        }
    }
}