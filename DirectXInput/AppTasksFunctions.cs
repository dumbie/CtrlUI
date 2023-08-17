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
                    await TaskDelay(500, vTask_UpdateWindowStatus);
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
                    await TaskDelay(2000, vTask_ControllerMonitor);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerDisconnect()
        {
            try
            {
                while (TaskCheckLoop(vTask_ControllerDisconnect))
                {
                    await CheckAllControllersTimeout();
                    await CheckAllControllersIdle();

                    //Delay the loop task
                    await TaskDelay(1000, vTask_ControllerDisconnect);
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
                    //Controller update led color
                    ControllerLedColor(vController0);
                    ControllerLedColor(vController1);
                    ControllerLedColor(vController2);
                    ControllerLedColor(vController3);

                    //Delay the loop task
                    await TaskDelay(1000, vTask_ControllerLedColor);
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
                    //Read controller battery level
                    ControllerReadBatteryLevel(vController0);
                    ControllerReadBatteryLevel(vController1);
                    ControllerReadBatteryLevel(vController2);
                    ControllerReadBatteryLevel(vController3);

                    //Check controller low battery level
                    CheckAllControllersLowBattery(false);

                    //Delay the loop task
                    await TaskDelay(2000, vTask_ControllerBattery);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerInformation()
        {
            try
            {
                while (TaskCheckLoop(vTask_ControllerInformation))
                {
                    UpdateControllerInformation();

                    //Delay the loop task
                    await TaskDelay(100, vTask_ControllerInformation);
                }
            }
            catch { }
        }
    }
}