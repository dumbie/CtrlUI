using System.Collections.Generic;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputHotkey;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        private void EventHotkeyPressed(List<KeysVirtual> keysPressed)
        {
            try
            {
                //Check hotkeys
                List<KeysVirtual> usedKeysShowHideBrowser = new List<KeysVirtual>
                {
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey0ShowHideBrowser", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey1ShowHideBrowser", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey2ShowHideBrowser", typeof(byte))
                };
                List<KeysVirtual> usedKeysShowHideCrosshair = new List<KeysVirtual>
                {
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey0ShowHideCrosshair", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey1ShowHideCrosshair", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey2ShowHideCrosshair", typeof(byte))
                };
                List<KeysVirtual> usedKeysShowHideFpsStats = new List<KeysVirtual>
                {
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey0ShowHideFpsStats", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey1ShowHideFpsStats", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey2ShowHideFpsStats", typeof(byte))
                };
                List<KeysVirtual> usedKeysPositionFpsStats = new List<KeysVirtual>
                {
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey0PositionFpsStats", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey1PositionFpsStats", typeof(byte)),
                    (KeysVirtual)SettingLoad(vConfigurationFpsOverlayer, "Hotkey2PositionFpsStats", typeof(byte))
                };

                //Check presses
                if (CheckHotkeyPress(keysPressed, usedKeysShowHideBrowser))
                {
                    Debug.WriteLine("Button Global - ShowHideBrowser");
                    vWindowBrowser.Browser_Switch_Visibility();
                }
                else if (CheckHotkeyPress(keysPressed, usedKeysShowHideCrosshair))
                {
                    Debug.WriteLine("Button Global - ShowHideCrosshair");
                    SwitchCrosshairVisibility(true);
                }
                else if (CheckHotkeyPress(keysPressed, usedKeysShowHideFpsStats))
                {
                    Debug.WriteLine("Button Global - ShowHideFpsStats");
                    SwitchFpsOverlayVisibility();
                }
                else if (CheckHotkeyPress(keysPressed, usedKeysPositionFpsStats))
                {
                    Debug.WriteLine("Button Global - PositionFpsStats");
                    ChangeFpsOverlayPosition();
                }
            }
            catch { }
        }
    }
}