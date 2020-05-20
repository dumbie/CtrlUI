using System;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Update the button press times
        void UpdateButtonPressTimes(ControllerButtonDetails buttonDetails)
        {
            try
            {
                if (buttonDetails.PressedRaw)
                {
                    if (buttonDetails.PressTimeCurrent == 0)
                    {
                        //Debug.WriteLine("Starting button press.");
                        buttonDetails.PressTimeCurrent = Environment.TickCount;
                    }
                }
                else
                {
                    if (buttonDetails.PressTimeDone)
                    {
                        buttonDetails.PressTimePrevious = 0;
                    }
                    else if (buttonDetails.PressTimeCurrent > 0)
                    {
                        buttonDetails.PressTimePrevious = Environment.TickCount - buttonDetails.PressTimeCurrent;
                    }

                    buttonDetails.PressTimeDone = false;
                    buttonDetails.PressTimeCurrent = 0;
                    //Debug.WriteLine("Releasing button press: " + buttonDetails.PressTimePrevious);
                }
            }
            catch { }
        }

        //Check the button press times
        void CheckButtonPressTimes(ControllerButtonDetails buttonDetails)
        {
            try
            {
                buttonDetails.PressedShort = false;
                buttonDetails.PressedLong = false;

                if (!buttonDetails.PressTimeDone)
                {
                    if (buttonDetails.PressTimePrevious > 0 && buttonDetails.PressTimePrevious <= 500)
                    {
                        buttonDetails.PressedShort = true;
                        buttonDetails.PressTimeDone = true;
                        return;
                    }

                    if (buttonDetails.PressTimeCurrent > 0)
                    {
                        int pressTimeCurrentMs = Environment.TickCount - buttonDetails.PressTimeCurrent;
                        if (pressTimeCurrentMs >= 800)
                        {
                            buttonDetails.PressedLong = true;
                            buttonDetails.PressTimeDone = true;
                            return;
                        }
                    }
                }
            }
            catch { }
        }
    }
}