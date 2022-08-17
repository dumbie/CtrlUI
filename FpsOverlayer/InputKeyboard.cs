using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        private async void EventHotKeyPressed(KeysModifier keysModifier, KeysVirtual keysVirtual)
        {
            try
            {
                if (keysModifier == KeysModifier.Alt && keysVirtual == KeysVirtual.F9)
                {
                    Debug.WriteLine("Button Global - F9");
                    SwitchCrosshairVisibility();
                }
                else if (keysModifier == KeysModifier.Alt && keysVirtual == KeysVirtual.F10)
                {
                    Debug.WriteLine("Button Global - F10");
                    await ChangeFpsOverlayPosition();
                }
                else if (keysModifier == KeysModifier.Alt && keysVirtual == KeysVirtual.F11)
                {
                    Debug.WriteLine("Button Global - F11");
                    SwitchFpsOverlayVisibility();
                }
            }
            catch { }
        }
    }
}