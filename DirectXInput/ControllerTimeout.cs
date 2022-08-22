using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check if a controller has timed out
        async Task ControllerTimeout(ControllerStatus Controller)
        {
            try
            {
                //Debug.WriteLine("Checking if controller " + Controller.NumberId + " has timed out for " + Controller.MilliSecondsTimeout + " ms.");
                if (Controller.Connected() && Controller.InputReport != null && Controller.LastInputTicks != 0 && Controller.PrevInputTicks != 0)
                {
                    long latencyMs = GetSystemTicksMs() - Controller.LastInputTicks;
                    if (latencyMs > Controller.MilliSecondsTimeout)
                    {
                        Debug.WriteLine("Controller " + Controller.NumberId + " has timed out, stopping and removing the controller.");
                        await StopController(Controller, "timeout", "Controller " + Controller.NumberId + " has timed out.");
                    }
                }
            }
            catch { }
        }

        //Check for timed out controllers
        async Task CheckControllersTimeout()
        {
            try
            {
                await ControllerTimeout(vController0);
                await ControllerTimeout(vController1);
                await ControllerTimeout(vController2);
                await ControllerTimeout(vController3);
            }
            catch { }
        }
    }
}