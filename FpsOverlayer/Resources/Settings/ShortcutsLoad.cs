using System;
using System.Diagnostics;
using System.Linq;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        void Shortcuts_Load()
        {
            try
            {
                Debug.WriteLine("Loading application shortcuts...");

                keyboard_ShowHideTools.Set(vShortcutTriggers.FirstOrDefault(x => x.Name == keyboard_ShowHideTools.TriggerName));
                keyboard_ShowHideCrosshair.Set(vShortcutTriggers.FirstOrDefault(x => x.Name == keyboard_ShowHideCrosshair.TriggerName));
                keyboard_ShowHideFpsStats.Set(vShortcutTriggers.FirstOrDefault(x => x.Name == keyboard_ShowHideFpsStats.TriggerName));
                keyboard_PositionFpsStats.Set(vShortcutTriggers.FirstOrDefault(x => x.Name == keyboard_PositionFpsStats.TriggerName));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load application shortcuts: " + ex.Message);
            }
        }
    }
}