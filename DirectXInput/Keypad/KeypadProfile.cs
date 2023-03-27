using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static DirectXInput.SettingsNotify;

namespace DirectXInput.KeypadCode
{
    public partial class WindowKeypad
    {
        //Switch keypad profile
        public async Task SwitchKeypadProfile()
        {
            try
            {
                //Check if the keypad process changed
                string processNameLower = vProcessForeground.ExeNameNoExt.ToLower();
                string processTitleLower = vProcessForeground.WindowTitleMain.ToLower().Replace(" ", string.Empty);
                if (processNameLower != vKeypadPreviousProcessName || processTitleLower != vKeypadPreviousProcessTitle)
                {
                    Debug.WriteLine("Keypad process changed to: " + processNameLower + "/" + processTitleLower);
                    vKeypadPreviousProcessName = processNameLower;
                    vKeypadPreviousProcessTitle = processTitleLower;

                    //Set the keypad mapping profile
                    SetKeypadMappingProfile();

                    //Update the key names
                    UpdateKeypadNames();

                    //Update the keypad opacity
                    UpdatePopupOpacity();

                    //Update the keypad style
                    UpdateKeypadStyle();

                    //Update the keypad size
                    double keypadHeight = UpdateKeypadSize();

                    //Notify - Fps Overlayer keypad size changed
                    await NotifyFpsOverlayerKeypadSizeChanged(Convert.ToInt32(keypadHeight));
                }
            }
            catch { }
        }
    }
}