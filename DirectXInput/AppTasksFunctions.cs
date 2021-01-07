using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput
{
    public partial class WindowMain
    {
        async Task vTaskLoop_UpdateWindowStatus()
        {
            try
            {
                while (!vTask_UpdateWindowStatus.TaskStopRequest)
                {
                    UpdateWindowStatus();

                    //Delay the loop task
                    await TaskDelayLoop(500, vTask_UpdateWindowStatus);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerMonitor()
        {
            try
            {
                while (!vTask_ControllerMonitor.TaskStopRequest)
                {
                    await MonitorController();
                    MonitorVolumeMute();

                    //Delay the loop task
                    await TaskDelayLoop(2000, vTask_ControllerMonitor);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerTimeout()
        {
            try
            {
                while (!vTask_ControllerTimeout.TaskStopRequest)
                {
                    CheckControllersTimeout();

                    //Delay the loop task
                    await TaskDelayLoop(1000, vTask_ControllerTimeout);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerBattery()
        {
            try
            {
                while (!vTask_ControllerBattery.TaskStopRequest)
                {
                    CheckAllControllersLowBattery();

                    //Delay the loop task
                    await TaskDelayLoop(5000, vTask_ControllerBattery);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerPreview()
        {
            try
            {
                while (!vTask_ControllerPreview.TaskStopRequest)
                {
                    UpdateControllerPreview();

                    //Delay the loop task
                    await TaskDelayLoop(100, vTask_ControllerPreview);
                }
            }
            catch { }
        }
    }
}