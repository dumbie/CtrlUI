using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check for timed out controllers
        async Task CheckAllControllersTimeout()
        {
            try
            {
                await CheckControllerTimeout(vController0);
                await CheckControllerTimeout(vController1);
                await CheckControllerTimeout(vController2);
                await CheckControllerTimeout(vController3);
            }
            catch { }
        }

        //Check if controller has timed out
        async Task<bool> CheckControllerTimeout(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Connected() && Controller.ControllerDataInput != null && Controller.TicksInputLast != 0 && Controller.TicksInputPrev != 0)
                {
                    if (!Controller.TimeoutIgnore)
                    {
                        long lastMs = GetSystemTicksMs() - Controller.TicksInputLast;
                        //Debug.WriteLine("Controller " + Controller.NumberId + " time out check: " + lastMs + "/" + Controller.TicksTargetTimeout + "ms.");
                        if (lastMs > Controller.TicksTargetTimeout)
                        {
                            await StopController(Controller, "timeout", "Disconnected timed out controller " + Controller.NumberId);
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }
    }
}