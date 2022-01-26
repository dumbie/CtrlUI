using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class WindowMain
    {
        async Task vTaskLoop_UpdateWindowStatus()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateWindowStatus))
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
                while (TaskCheckLoop(vTask_ControllerMonitor))
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
                while (TaskCheckLoop(vTask_ControllerTimeout))
                {
                    CheckControllersTimeout();

                    //Delay the loop task
                    await TaskDelayLoop(1000, vTask_ControllerTimeout);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerLedColor()
        {
            try
            {
                while (TaskCheckLoop(vTask_ControllerLedColor))
                {
                    ControllerLedColor(vController0);
                    ControllerLedColor(vController1);
                    ControllerLedColor(vController2);
                    ControllerLedColor(vController3);

                    //Delay the loop task
                    await TaskDelayLoop(1000, vTask_ControllerLedColor);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerBattery()
        {
            try
            {
                while (TaskCheckLoop(vTask_ControllerBattery))
                {
                    await CheckAllControllersLowBattery(false);

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
                while (TaskCheckLoop(vTask_ControllerPreview))
                {
                    UpdateControllerPreview();

                    //Delay the loop task
                    await TaskDelayLoop(50, vTask_ControllerPreview);
                }
            }
            catch { }
        }
    }
}