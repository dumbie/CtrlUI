using System.Collections.Generic;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        private void EventHotKeyPressed(List<KeysVirtual> keysPressed)
        {
            try
            {
                bool altPressed = keysPressed.Contains(KeysVirtual.AltLeft);

                if (altPressed && keysPressed.Contains(KeysVirtual.F8))
                {
                    if (SettingLoad(vConfigurationFpsOverlayer, "ShortcutShowHideBrowser", typeof(bool)))
                    {
                        Debug.WriteLine("Button Global - Alt + F8");
                        vWindowBrowser.Browser_Switch_Visibility();
                    }
                }
                else if (altPressed && keysPressed.Contains(KeysVirtual.F9))
                {
                    if (SettingLoad(vConfigurationFpsOverlayer, "ShortcutShowHideCrosshair", typeof(bool)))
                    {
                        Debug.WriteLine("Button Global - Alt + F9");
                        SwitchCrosshairVisibility(true);
                    }
                }
                else if (altPressed && keysPressed.Contains(KeysVirtual.F10))
                {
                    if (SettingLoad(vConfigurationFpsOverlayer, "ShortcutShowHideFpsStats", typeof(bool)))
                    {
                        Debug.WriteLine("Button Global - Alt + F10");
                        SwitchFpsOverlayVisibility();
                    }
                }
                else if (altPressed && keysPressed.Contains(KeysVirtual.F11))
                {
                    if (SettingLoad(vConfigurationFpsOverlayer, "ShortcutPositionFpsStats", typeof(bool)))
                    {
                        Debug.WriteLine("Button Global - Alt + F11");
                        ChangeFpsOverlayPosition();
                    }
                }
            }
            catch { }
        }
    }
}