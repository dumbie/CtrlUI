using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVInputOutputClass;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        public void Shortcuts_Check()
        {
            try
            {
                Debug.WriteLine("Checking application shortcuts...");

                if (!vShortcutTriggers.Any(x => x.Name == "ShowHideBrowser"))
                {
                    ShortcutTriggerKeyboard shortcutTrigger = new ShortcutTriggerKeyboard();
                    shortcutTrigger.Name = "ShowHideBrowser";
                    shortcutTrigger.Trigger = [KeysVirtual.CtrlLeft, KeysVirtual.None, KeysVirtual.F9];
                    vShortcutTriggers.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutTriggers, @"Profiles\User\FpsShortcutsKeyboard.json");
                }
                if (!vShortcutTriggers.Any(x => x.Name == "ShowHideCrosshair"))
                {
                    ShortcutTriggerKeyboard shortcutTrigger = new ShortcutTriggerKeyboard();
                    shortcutTrigger.Name = "ShowHideCrosshair";
                    shortcutTrigger.Trigger = [KeysVirtual.CtrlLeft, KeysVirtual.None, KeysVirtual.F10];
                    vShortcutTriggers.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutTriggers, @"Profiles\User\FpsShortcutsKeyboard.json");
                }
                if (!vShortcutTriggers.Any(x => x.Name == "ShowHideFpsStats"))
                {
                    ShortcutTriggerKeyboard shortcutTrigger = new ShortcutTriggerKeyboard();
                    shortcutTrigger.Name = "ShowHideFpsStats";
                    shortcutTrigger.Trigger = [KeysVirtual.CtrlLeft, KeysVirtual.None, KeysVirtual.F11];
                    vShortcutTriggers.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutTriggers, @"Profiles\User\FpsShortcutsKeyboard.json");
                }
                if (!vShortcutTriggers.Any(x => x.Name == "PositionFpsStats"))
                {
                    ShortcutTriggerKeyboard shortcutTrigger = new ShortcutTriggerKeyboard();
                    shortcutTrigger.Name = "PositionFpsStats";
                    shortcutTrigger.Trigger = [KeysVirtual.CtrlLeft, KeysVirtual.None, KeysVirtual.F12];
                    vShortcutTriggers.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutTriggers, @"Profiles\User\FpsShortcutsKeyboard.json");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check application shortcuts: " + ex.Message);
            }
        }
    }
}