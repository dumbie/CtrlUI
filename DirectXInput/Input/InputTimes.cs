using static ArnoldVinkCode.AVActions;
using static LibraryShared.Classes;
using static LibraryShared.ControllerTimings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Update and check button press time
        void CheckControllerButtonPressTimes(ControllerStatus controllerStatus)
        {
            try
            {
                foreach (ControllerButtonDetails buttonDetails in controllerStatus.InputCurrent.Buttons)
                {
                    CheckControllerButtonPressTime(buttonDetails);
                }
            }
            catch { }
        }

        //Update and check button press time
        void CheckControllerButtonPressTime(ControllerButtonDetails buttonDetails)
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