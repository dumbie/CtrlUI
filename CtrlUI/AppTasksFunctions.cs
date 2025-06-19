using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace CtrlUI
{
    public partial class WindowMain
    {
        async Task vTaskLoop_UpdateClock()
        {
            try
            {
                while (await TaskCheckLoop(vTask_UpdateClock, 5000))
                {
                    UpdateClockTime();
                }
            }
            catch { }
        }

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

        async Task vTaskLoop_ControllerConnected()
        {
            try
            {
                while (await TaskCheckLoop(vTask_ControllerConnected, 2000))
                {
                    await UpdateControllerConnected();
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateAppRunningTime()
        {
            try
            {
                while (await TaskCheckLoop(vTask_UpdateAppRunningTime, 60000))
                {
                    UpdateAppRunningTime();
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateMediaInformation()
        {
            try
            {
                while (await TaskCheckLoop(vTask_UpdateMediaInformation, 2000))
                {
                    UpdateCurrentVolumeInformation();
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateGallery()
        {
            try
            {
                while (await TaskCheckLoop(vTask_UpdateGallery, 2000))
                {
                    await RefreshListGallery(false);
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateProcesses()
        {
            try
            {
                while (await TaskCheckLoop(vTask_UpdateProcesses, 2000))
                {
                    await RefreshProcesses();
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateLaunchers()
        {
            try
            {
                while (await TaskCheckLoop(vTask_UpdateLaunchers, 2000))
                {
                    await LoadListLaunchers();
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateShortcuts()
        {
            try
            {
                while (await TaskCheckLoop(vTask_UpdateShortcuts, 2000))
                {
                    await RefreshListShortcuts(false);
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateListStatus()
        {
            try
            {
                while (await TaskCheckLoop(vTask_UpdateListStatus, 2000))
                {
                    await RefreshListStatus();
                }
            }
            catch { }
        }
    }
}