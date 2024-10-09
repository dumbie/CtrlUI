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
                if (Controller.Connected() && !Controller.TimeoutIgnore && Controller.ControllerDataInput != null && Controller.TicksInputLast != 0 && Controller.TicksInputPrev != 0)
                {
                    if (Controller.SupportedCurrent.HasInputOnDemand)
                    {
                        if (Controller.ReadFailureCount > Controller.ReadFailureCountTarget)
                        {
                            Debug.WriteLine("Controller " + Controller.NumberId + " has timed out: " + Controller.ReadFailureCount + " failures.");
                            await StopController(Controller, "timeout", "Disconnected timed out controller " + Controller.NumberId);
                            return true;
                        }
                    }
                    else
                    {
                        long lastMs = GetSystemTicksMs() - Controller.TicksInputLast;
                        if (lastMs > Controller.TicksTimeoutTarget)
                        {
                            Debug.WriteLine("Controller " + Controller.NumberId + " has timed out: " + lastMs + "/" + Controller.TicksTimeoutTarget + "ms.");
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