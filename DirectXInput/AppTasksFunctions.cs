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
                while (await TaskCheckLoop(vTask_UpdateWindowStatus, 500))
                {
                    UpdateWindowStatus();
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerMonitor()
        {
            try
            {
                while (await TaskCheckLoop(vTask_ControllerMonitor, 2000))
                {
                    await MonitorController();
                    MonitorVolumeMute();
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerDisconnect()
        {
            try
            {
                while (await TaskCheckLoop(vTask_ControllerDisconnect, 1000))
                {
                    await CheckAllControllersTimeout();
                    await CheckAllControllersIdle();
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerLedColor()
        {
            try
            {
                while (await TaskCheckLoop(vTask_ControllerLedColor, 1000))
                {
                    //Controller update led color
                    ControllerLedColor(vController0);
                    ControllerLedColor(vController1);
                    ControllerLedColor(vController2);
                    ControllerLedColor(vController3);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerBattery()
        {
            try
            {
                while (await TaskCheckLoop(vTask_ControllerBattery, 2000))
                {
                    //Read controller battery level
                    ControllerReadBatteryLevel(vController0);
                    ControllerReadBatteryLevel(vController1);
                    ControllerReadBatteryLevel(vController2);
                    ControllerReadBatteryLevel(vController3);

                    //Check controller low battery level
                    CheckAllControllersLowBattery(false);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerInformation()
        {
            try
            {
                while (await TaskCheckLoop(vTask_ControllerInformation, 100))
                {
                    UpdateControllerInformation();
                }
            }
            catch { }
        }
    }
}