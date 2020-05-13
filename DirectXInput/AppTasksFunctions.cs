using static ArnoldVinkCode.AVActions;

namespace DirectXInput
{
    public partial class WindowMain
    {
        async void vTaskLoop_UpdateWindowStatus()
        {
            try
            {
                while (vTask_UpdateWindowStatus.Status == AVTaskStatus.Running)
                {
                    UpdateWindowStatus();

                    //Delay the loop task
                    await TaskDelayLoop(500, vTask_UpdateWindowStatus);
                }
            }
            catch { }
            finally
            {
                vTask_UpdateWindowStatus.Status = AVTaskStatus.Stopped;
            }
        }

        async void vTaskLoop_ControllerMonitor()
        {
            try
            {
                while (vTask_ControllerMonitor.Status == AVTaskStatus.Running)
                {
                    await MonitorControllers();

                    //Delay the loop task
                    await TaskDelayLoop(2000, vTask_ControllerMonitor);
                }
            }
            catch { }
            finally
            {
                vTask_ControllerMonitor.Status = AVTaskStatus.Stopped;
            }
        }

        async void vTaskLoop_ControllerTimeout()
        {
            try
            {
                while (vTask_ControllerTimeout.Status == AVTaskStatus.Running)
                {
                    CheckControllersTimeout();

                    //Delay the loop task
                    await TaskDelayLoop(1000, vTask_ControllerTimeout);
                }
            }
            catch { }
            finally
            {
                vTask_ControllerTimeout.Status = AVTaskStatus.Stopped;
            }
        }

        async void vTaskLoop_ControllerBattery()
        {
            try
            {
                while (vTask_ControllerBattery.Status == AVTaskStatus.Running)
                {
                    CheckAllControllersLowBattery();

                    //Delay the loop task
                    await TaskDelayLoop(5000, vTask_ControllerBattery);
                }
            }
            catch { }
            finally
            {
                vTask_ControllerBattery.Status = AVTaskStatus.Stopped;
            }
        }
    }
}