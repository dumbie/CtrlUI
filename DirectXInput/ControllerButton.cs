using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Update and check button press times
        void UpdateCheckButtonPressTimes(ControllerStatus controllerStatus)
        {
            try
            {
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.ButtonGuide);
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.ButtonTouchpad);
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.ButtonThumbLeftLeft);
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.ButtonThumbLeftUp);
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.ButtonThumbLeftRight);
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.ButtonThumbLeftDown);
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.DPadLeft);
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.DPadUp);
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.DPadRight);
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.DPadDown);
            }
            catch { }
        }

        //Update and check button press time
        void UpdateCheckButtonPressTime(ControllerButtonDetails buttonDetails)
        {
            try
            {
                //Update button press times
                if (buttonDetails.PressedRaw)
                {
                    long currentSystemTicksMs = GetSystemTicksMs();
                    if (buttonDetails.PressTimeStart == 0)
                    {
                        buttonDetails.PressTimeStart = currentSystemTicksMs;
                    }
                    else
                    {
                        buttonDetails.PressTimeCurrent = currentSystemTicksMs - buttonDetails.PressTimeStart;
                    }
                    //Debug.WriteLine("Holding button press: " + buttonDetails.PressTimeCurrent);
                }
                else
                {
                    if (buttonDetails.PressTimeDone)
                    {
                        buttonDetails.PressTimePrevious = 0;
                    }
                    else
                    {
                        buttonDetails.PressTimePrevious = buttonDetails.PressTimeCurrent;
                    }
                    buttonDetails.PressTimeDone = false;
                    buttonDetails.PressTimeStart = 0;
                    buttonDetails.PressTimeCurrent = 0;
                    //Debug.WriteLine("Releasing button press: " + buttonDetails.PressTimePrevious);
                }

                //Check button press times
                buttonDetails.PressedShort = false;
                buttonDetails.PressedLong = false;
                if (!buttonDetails.PressTimeDone)
                {
                    if (buttonDetails.PressTimePrevious > 0 && buttonDetails.PressTimePrevious <= vControllerButtonPressShort)
                    {
                        //Debug.WriteLine("Button press short: " + buttonDetails.PressTimePrevious);
                        buttonDetails.PressedShort = true;
                        buttonDetails.PressTimeDone = true;
                    }
                    else if (buttonDetails.PressTimeCurrent >= vControllerButtonPressLong)
                    {
                        //Debug.WriteLine("Button press long: " + buttonDetails.PressTimeCurrent);
                        buttonDetails.PressedLong = true;
                        buttonDetails.PressTimeDone = true;
                    }
                }
            }
            catch { }
        }
    }
}