using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    public partial class WindowMain
    {
        async void vTaskAction_UpdateClock()
        {
            try
            {
                while (TaskRunningCheck(vTaskToken_UpdateClock))
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
                while (TaskRunningCheck(vTaskToken_UpdateWindowStatus))
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
                while (TaskRunningCheck(vTaskToken_ControllerConnected))
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
                while (TaskRunningCheck(vTaskToken_UpdateAppRunningStatus))
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
                while (TaskRunningCheck(vTaskToken_UpdateAppRunningTime))
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
                while (TaskRunningCheck(vTaskToken_UpdateApplications))
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
                while (TaskRunningCheck(vTaskToken_ShowHideMouse))
                {
                    await MouseCursorHide();
                    await Task.Delay(3000);
                }
            }
            catch { }
        }
    }
}