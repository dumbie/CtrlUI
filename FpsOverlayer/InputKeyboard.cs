using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        private void EventHotKeyPressed(KeysModifier keysModifier, KeysVirtual keysVirtual)
        {
            try
            {
                if (keysModifier == KeysModifier.Alt && keysVirtual == KeysVirtual.F9)
                {
                    Debug.WriteLine("Button Global - F9");
                    SwitchFpsOverlayVisibilityManual();
                }
                else if (keysModifier == KeysModifier.Alt && keysVirtual == KeysVirtual.F10)
                {
                    Debug.WriteLine("Button Global - F10");
                    SwitchCrosshairVisibility();
                }
                else if (keysModifier == KeysModifier.Alt && keysVirtual == KeysVirtual.F11)
                {
                    Debug.WriteLine("Button Global - F11");
                    ChangeFpsOverlayPosition();
                }
            }
            catch { }
        }
    }
}