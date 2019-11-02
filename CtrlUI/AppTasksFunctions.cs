using System.Threading.Tasks;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    public partial class WindowMain
    {
        async void vTaskAction_UpdateClock()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_UpdateClock))
                {
                    UpdateClock();
                    await Task.Delay(5000);
                }
            }
            catch { }
        }

        async void vTaskAction_UpdateWindowStatus()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_UpdateWindowStatus))
                {
                    await UpdateWindowStatus();
                    await Task.Delay(500);
                }
            }
            catch { }
        }

        async void vTaskAction_ControllerConnected()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_ControllerConnected))
                {
                    UpdateControllerConnected();
                    await Task.Delay(2500);
                }
            }
            catch { }
        }

        async void vTaskAction_UpdateAppRunningStatus()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_UpdateAppRunningStatus))
                {
                    CheckAppRunningStatus(null);
                    await Task.Delay(5000);
                }
            }
            catch { }
        }

        async void vTaskAction_UpdateAppRunningTime()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_UpdateAppRunningTime))
                {
                    UpdateAppRunningTime();
                    await Task.Delay(30000);
                }
            }
            catch { }
        }

        async void vTaskAction_UpdateApplications()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_UpdateApplications))
                {
                    if (vAppActivated)
                    {
                        await RefreshApplicationLists(false, false, false, true);
                    }
                    await Task.Delay(8000);
                }
            }
            catch { }
        }

        async void vTaskAction_ShowHideMouse()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_ShowHideMouse))
                {
                    await MouseCursorHide();
                    await Task.Delay(3000);
                }
            }
            catch { }
        }
    }
}