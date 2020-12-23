using static ArnoldVinkCode.AVActions;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Update and check button press times
        void UpdateCheckButtonPressTimes(ControllerButtonDetails buttonDetails)
        {
            try
            {
                //Update button press times
                if (buttonDetails.PressedRaw)
                {
                    if (buttonDetails.PressTimeCurrent == 0)
                    {
                        //Debug.WriteLine("Starting button press.");
                        buttonDetails.PressTimeCurrent = GetSystemTicksMs();
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
                        buttonDetails.PressTimePrevious = GetSystemTicksMs() - buttonDetails.PressTimeCurrent;
                    }

                    buttonDetails.PressTimeDone = false;
                    buttonDetails.PressTimeCurrent = 0;
                    //Debug.WriteLine("Releasing button press: " + buttonDetails.PressTimePrevious);
                }

                //Check button press times
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
                        long pressTimeCurrentMs = GetSystemTicksMs() - buttonDetails.PressTimeCurrent;
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