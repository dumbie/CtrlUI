using ArnoldVinkCode;
using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVJsonFunctions;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        void Shortcuts_Save()
        {
            try
            {
                Debug.WriteLine("Saving application shortcuts...");

                keyboard_ShowHideTools.TriggerChanged += Shortcut_Keyboard_TriggerChanged;
                keyboard_ShowHideCrosshair.TriggerChanged += Shortcut_Keyboard_TriggerChanged;
                keyboard_ShowHideFpsStats.TriggerChanged += Shortcut_Keyboard_TriggerChanged;
                keyboard_PositionFpsStats.TriggerChanged += Shortcut_Keyboard_TriggerChanged;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save application shortcuts: " + ex.Message);
            }
        }

        void Shortcut_Keyboard_TriggerChanged(ShortcutTriggerKeyboard triggers)
        {
            try
            {
                if (vShortcutTriggers.ListReplaceFirstItem(x => x.Name == triggers.Name, triggers))
                {
                    JsonSaveObject(vShortcutTriggers, @"Profiles\User\FpsShortcutsKeyboard.json");
                }
            }
            catch { }
        }
    }
}