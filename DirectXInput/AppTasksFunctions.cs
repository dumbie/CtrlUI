using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput
{
    public partial class WindowMain
    {
        async void vTaskAction_UpdateWindowStatus()
        {
            try
            {
                while (TaskRunningCheck(vTaskToken_UpdateWindowStatus))
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
                while (TaskRunningCheck(vTaskToken_ControllerMonitor))
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
                while (TaskRunningCheck(vTaskToken_ControllerTimeout))
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
                while (TaskRunningCheck(vTaskToken_ControllerBattery))
                {
                    CheckAllControllersLowBattery();
                    await Task.Delay(5000);
                }
            }
            catch { }
        }
    }
}