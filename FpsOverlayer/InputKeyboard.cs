using System.Collections.Generic;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
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
                    Debug.WriteLine("Button Global - Alt + F8");
                    vWindowBrowser.Browser_Switch_Visibility();
                }
                else if (altPressed && keysPressed.Contains(KeysVirtual.F9))
                {
                    Debug.WriteLine("Button Global - Alt + F9");
                    SwitchCrosshairVisibility();
                }
                else if (altPressed && keysPressed.Contains(KeysVirtual.F10))
                {
                    Debug.WriteLine("Button Global - Alt + F10");
                    SwitchFpsOverlayVisibilityManual();
                }
                else if (altPressed && keysPressed.Contains(KeysVirtual.F11))
                {
                    Debug.WriteLine("Button Global - Alt + F11");
                    ChangeFpsOverlayPosition();
                }
            }
            catch { }
        }
    }
}