using System;
using System.Diagnostics;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check if a controller has timed out
        void ControllerTimeout(ControllerStatus Controller)
        {
            try
            {
                //Debug.WriteLine("Checking if controller " + Controller.NumberId + " has timed out for " + Controller.TimeoutSeconds + " seconds.");
                if (Controller.Connected && Controller.InputReport != null && Controller.LastActive != 0)
                {
                    if ((Environment.TickCount - Controller.LastActive) > Controller.MilliSecondsTimeout)
                    {
                        Debug.WriteLine("Controller " + Controller.NumberId + " has timed out, stopping and removing the controller.");
                        StopControllerTask(Controller, false);
                    }
                }
            }
            catch { }
        }

        //Check for timed out controllers
        void CheckControllersTimeout()
        {
            try
            {
                ControllerTimeout(vController0);
                ControllerTimeout(vController1);
                ControllerTimeout(vController2);
                ControllerTimeout(vController3);
            }
            catch { }
        }
    }
}