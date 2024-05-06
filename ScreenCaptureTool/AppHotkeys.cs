using System.Collections.Generic;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputHotkey;
using static ArnoldVinkCode.AVSettings;
using static ScreenCapture.AppVariables;

namespace ScreenCapture
{
    public partial class AppHotkeys
    {
        public static async void EventHotkeyPressed(List<KeysVirtual> keysPressed)
        {
            try
            {
                //Check hotkeys
                List<KeysVirtual> usedKeysCaptureImage = new List<KeysVirtual>
                {
                    (KeysVirtual)SettingLoad(vConfiguration, "Hotkey0CaptureImage", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfiguration, "Hotkey1CaptureImage", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfiguration, "Hotkey2CaptureImage", typeof(byte))
                };
                List<KeysVirtual> usedKeysCaptureVideo = new List<KeysVirtual>
                {
                    (KeysVirtual)SettingLoad(vConfiguration, "Hotkey0CaptureVideo", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfiguration, "Hotkey1CaptureVideo", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfiguration, "Hotkey2CaptureVideo", typeof(byte))
                };

                //Check presses
                if (CheckHotkeyPress(keysPressed, usedKeysCaptureImage))
                {
                    Debug.WriteLine("Button Global - Capture image");
                    await CaptureScreen.CaptureImageProcess(0);
                }
                else if (CheckHotkeyPress(keysPressed, usedKeysCaptureVideo))
                {
                    Debug.WriteLine("Button Global - Capture video");
                    await CaptureScreen.CaptureVideoProcess(0);
                }
            }
            catch { }
        }
    }
}