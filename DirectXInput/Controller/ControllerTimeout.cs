using System.Diagnostics;
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
                //Check if controller is connected and has data
                if (Controller.Connected() && !Controller.SupportedCurrent.HasInputOnDemand && !Controller.TimeoutIgnore && Controller.ControllerDataInput != null && Controller.TicksInputLast != 0 && Controller.TicksInputPrev != 0)
                {
                    long lastMs = GetSystemTicksMs() - Controller.TicksInputLast;
                    if (lastMs > Controller.TicksTargetTimeout)
                    {
                        Debug.WriteLine("Controller " + Controller.NumberId + " has timed out: " + lastMs + "/" + Controller.TicksTargetTimeout + "ms.");
                        await StopController(Controller, "timeout", "Disconnected timed out controller " + Controller.NumberId);
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }
    }
}