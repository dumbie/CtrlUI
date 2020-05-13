using static ArnoldVinkCode.AVActions;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    public partial class WindowMain
    {
        async void vTaskLoop_UpdateClock()
        {
            try
            {
                while (vTask_UpdateClock.Status == AVTaskStatus.Running)
                {
                    UpdateClockTime();

                    //Delay the loop task
                    await TaskDelayLoop(5000, vTask_UpdateClock);
                }
            }
            catch { }
            finally
            {
                vTask_UpdateClock.Status = AVTaskStatus.Stopped;
            }
        }

        async void vTaskLoop_UpdateWindowStatus()
        {
            try
            {
                while (vTask_UpdateWindowStatus.Status == AVTaskStatus.Running)
                {
                    await UpdateWindowStatus();

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

        async void vTaskLoop_ControllerConnected()
        {
            try
            {
                while (vTask_ControllerConnected.Status == AVTaskStatus.Running)
                {
                    await UpdateControllerConnected();

                    //Delay the loop task
                    await TaskDelayLoop(2000, vTask_ControllerConnected);
                }
            }
            catch { }
            finally
            {
                vTask_ControllerConnected.Status = AVTaskStatus.Stopped;
            }
        }

        async void vTaskLoop_UpdateAppRunningTime()
        {
            try
            {
                while (vTask_UpdateAppRunningTime.Status == AVTaskStatus.Running)
                {
                    UpdateAppRunningTime();

                    //Delay the loop task
                    await TaskDelayLoop(60000, vTask_UpdateAppRunningTime);
                }
            }
            catch { }
            finally
            {
                vTask_UpdateAppRunningTime.Status = AVTaskStatus.Stopped;
            }
        }

        async void vTaskLoop_UpdateMediaInformation()
        {
            try
            {
                while (vTask_UpdateMediaInformation.Status == AVTaskStatus.Running)
                {
                    await UpdateCurrentMediaInformation();

                    //Delay the loop task
                    await TaskDelayLoop(1000, vTask_UpdateMediaInformation);
                }
            }
            catch { }
            finally
            {
                vTask_UpdateMediaInformation.Status = AVTaskStatus.Stopped;
            }
        }

        async void vTaskLoop_UpdateProcesses()
        {
            try
            {
                while (vTask_UpdateProcesses.Status == AVTaskStatus.Running)
                {
                    if (vAppActivated)
                    {
                        await RefreshListProcessesWithWait(false);

                        //Delay the loop task
                        await TaskDelayLoop(3000, vTask_UpdateProcesses);
                    }
                    else
                    {
                        //Delay the loop task
                        await TaskDelayLoop(500, vTask_UpdateProcesses);
                    }
                }
            }
            catch { }
            finally
            {
                vTask_UpdateProcesses.Status = AVTaskStatus.Stopped;
            }
        }

        async void vTaskLoop_UpdateShortcuts()
        {
            try
            {
                while (vTask_UpdateShortcuts.Status == AVTaskStatus.Running)
                {
                    if (vAppActivated)
                    {
                        await RefreshListShortcuts(false);

                        //Delay the loop task
                        await TaskDelayLoop(6000, vTask_UpdateShortcuts);
                    }
                    else
                    {
                        //Delay the loop task
                        await TaskDelayLoop(500, vTask_UpdateShortcuts);
                    }
                }
            }
            catch { }
            finally
            {
                vTask_UpdateShortcuts.Status = AVTaskStatus.Stopped;
            }
        }

        async void vTaskLoop_UpdateListStatus()
        {
            try
            {
                while (vTask_UpdateListStatus.Status == AVTaskStatus.Running)
                {
                    if (vAppActivated)
                    {
                        RefreshListStatus();

                        //Delay the loop task
                        await TaskDelayLoop(2000, vTask_UpdateListStatus);
                    }
                    else
                    {
                        //Delay the loop task
                        await TaskDelayLoop(500, vTask_UpdateListStatus);
                    }
                }
            }
            catch { }
            finally
            {
                vTask_UpdateListStatus.Status = AVTaskStatus.Stopped;
            }
        }

        async void vTaskLoop_ShowHideMouse()
        {
            try
            {
                while (vTask_ShowHideMouse.Status == AVTaskStatus.Running)
                {
                    await MouseCursorCheckMovement();

                    //Delay the loop task
                    await TaskDelayLoop(3000, vTask_ShowHideMouse);
                }
            }
            catch { }
            finally
            {
                vTask_ShowHideMouse.Status = AVTaskStatus.Stopped;
            }
        }
    }
}