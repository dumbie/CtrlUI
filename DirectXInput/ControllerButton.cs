using static ArnoldVinkCode.AVActions;
using static LibraryShared.Classes;
using static LibraryShared.ControllerTimings;

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
                UpdateCheckButtonPressTime(controllerStatus.InputCurrent.ButtonMedia);
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
                        buttonDetails.PressTimeDone = false;
                        buttonDetails.PressTimeStart = currentSystemTicksMs;
                        buttonDetails.PressTimeCurrent = 0;
                    }
                    else
                    {
                        buttonDetails.PressTimeCurrent = currentSystemTicksMs - buttonDetails.PressTimeStart;
                        //Debug.WriteLine("Holding button press: " + buttonDetails.PressTimeCurrent);
                    }
                }
                else
                {
                    buttonDetails.PressTimeStart = 0;
                }

                //Check button press times
                buttonDetails.PressedShort = false;
                buttonDetails.PressedLong = false;
                if (!buttonDetails.PressTimeDone)
                {
                    if (buttonDetails.PressedRaw)
                    {
                        if (buttonDetails.PressTimeCurrent >= vControllerButtonPressLong)
                        {
                            //Debug.WriteLine("Button press long: " + buttonDetails.PressTimeCurrent);
                            buttonDetails.PressedLong = true;
                            buttonDetails.PressTimeDone = true;
                        }
                    }
                    else
                    {
                        if (buttonDetails.PressTimeCurrent > 0 && buttonDetails.PressTimeCurrent <= vControllerButtonPressShort)
                        {
                            //Debug.WriteLine("Button press short: " + buttonDetails.PressTimeCurrent);
                            buttonDetails.PressedShort = true;
                            buttonDetails.PressTimeDone = true;
                        }
                    }
                }
            }
            catch { }
        }
    }
}