using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput
{
    public partial class WindowMain
    {
        Task vTaskLoop_UpdateWindowStatus()
        {
            try
            {
                while (!vTask_UpdateWindowStatus.TaskStopRequest)
                {
                    UpdateWindowStatus();

                    //Delay the loop task
                    TaskDelayLoop(500, vTask_UpdateWindowStatus);
                }
            }
            catch { }
            return Task.FromResult(0);
        }

        async Task vTaskLoop_ControllerMonitor()
        {
            try
            {
                while (!vTask_ControllerMonitor.TaskStopRequest)
                {
                    await MonitorControllers();

                    //Delay the loop task
                    TaskDelayLoop(2000, vTask_ControllerMonitor);
                }
            }
            catch { }
        }

        Task vTaskLoop_ControllerTimeout()
        {
            try
            {
                while (!vTask_ControllerTimeout.TaskStopRequest)
                {
                    CheckControllersTimeout();

                    //Delay the loop task
                    TaskDelayLoop(1000, vTask_ControllerTimeout);
                }
            }
            catch { }
            return Task.FromResult(0);
        }

        Task vTaskLoop_ControllerBattery()
        {
            try
            {
                while (!vTask_ControllerBattery.TaskStopRequest)
                {
                    CheckAllControllersLowBattery();

                    //Delay the loop task
                    TaskDelayLoop(5000, vTask_ControllerBattery);
                }
            }
            catch { }
            return Task.FromResult(0);
        }
    }
}