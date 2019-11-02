using System.Threading.Tasks;

namespace DirectXInput
{
    public partial class WindowMain
    {
        async void vTaskAction_UpdateWindowStatus()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_UpdateWindowStatus))
                {
                    UpdateWindowStatus();
                    await Task.Delay(500);
                }
            }
            catch { }
        }

        async void vTaskAction_ControllerMonitor()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_ControllerMonitor))
                {
                    await MonitorControllers();
                    await Task.Delay(2000);
                }
            }
            catch { }
        }

        async void vTaskAction_ControllerTimeout()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_ControllerTimeout))
                {
                    CheckControllersTimeout();
                    await Task.Delay(1000);
                }
            }
            catch { }
        }

        async void vTaskAction_ControllerBattery()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_ControllerBattery))
                {
                    await CheckControllersLowBattery();
                    await Task.Delay(10000);
                }
            }
            catch { }
        }
    }
}