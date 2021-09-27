using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static DirectXInput.SettingsNotify;

namespace DirectXInput.KeypadCode
{
    public partial class WindowKeypad
    {
        //Switch keypad profile
        public async Task ControllerInteractionKeypadProfile()
        {
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_KeypadProfile)
                {
                    //Check if the keypad process changed
                    string processNameLower = vProcessForeground.Name.ToLower();
                    string processTitleLower = vProcessForeground.Title.ToLower();
                    if (processNameLower != vKeypadPreviousProcessName || processTitleLower != vKeypadPreviousProcessTitle)
                    {
                        Debug.WriteLine("Keypad process changed to: " + processNameLower + "/" + processTitleLower);
                        vKeypadPreviousProcessName = processNameLower;
                        vKeypadPreviousProcessTitle = processTitleLower;

                        //Set the keypad mapping profile
                        await SetKeypadMappingProfile();

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

                    vControllerDelay_KeypadProfile = GetSystemTicksMs() + vControllerDelayLongestTicks;
                }
            }
            catch { }
        }
    }
}