﻿using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    public partial class WindowMain
    {
        async Task vTaskLoop_UpdateClock()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateClock))
                {
                    UpdateClockTime();

                    //Delay the loop task
                    await TaskDelay(5000, vTask_UpdateClock);
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateWindowStatus()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateWindowStatus))
                {
                    await UpdateWindowStatus();

                    //Delay the loop task
                    await TaskDelay(500, vTask_UpdateWindowStatus);
                }
            }
            catch { }
        }

        async Task vTaskLoop_ControllerConnected()
        {
            try
            {
                while (TaskCheckLoop(vTask_ControllerConnected))
                {
                    if (vAppActivated)
                    {
                        await UpdateControllerConnected();

                        //Delay the loop task
                        await TaskDelay(2000, vTask_ControllerConnected);
                    }
                    else
                    {
                        //Delay the loop task
                        await TaskDelay(1000, vTask_ControllerConnected);
                    }
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateAppRunningTime()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateAppRunningTime))
                {
                    UpdateAppRunningTime();

                    //Delay the loop task
                    await TaskDelay(60000, vTask_UpdateAppRunningTime);
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateMediaInformation()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateMediaInformation))
                {
                    if (vAppActivated)
                    {
                        UpdateCurrentVolumeInformation();

                        //Delay the loop task
                        await TaskDelay(2000, vTask_UpdateMediaInformation);
                    }
                    else
                    {
                        //Delay the loop task
                        await TaskDelay(1000, vTask_UpdateMediaInformation);
                    }
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateProcesses()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateProcesses))
                {
                    if (vAppActivated)
                    {
                        await RefreshProcesses();

                        //Delay the loop task
                        await TaskDelay(3000, vTask_UpdateProcesses);
                    }
                    else
                    {
                        //Delay the loop task
                        await TaskDelay(1000, vTask_UpdateProcesses);
                    }
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateLaunchers()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateLaunchers))
                {
                    if (vAppActivated)
                    {
                        await LoadListLaunchers();

                        //Delay the loop task
                        await TaskDelay(60000, vTask_UpdateLaunchers);
                    }
                    else
                    {
                        //Delay the loop task
                        await TaskDelay(1000, vTask_UpdateLaunchers);
                    }
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateShortcuts()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateShortcuts))
                {
                    if (vAppActivated)
                    {
                        await RefreshListShortcuts(false);

                        //Delay the loop task
                        await TaskDelay(30000, vTask_UpdateShortcuts);
                    }
                    else
                    {
                        //Delay the loop task
                        await TaskDelay(1000, vTask_UpdateShortcuts);
                    }
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateListStatus()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateListStatus))
                {
                    if (vAppActivated)
                    {
                        await RefreshListStatus();

                        //Delay the loop task
                        await TaskDelay(2000, vTask_UpdateListStatus);
                    }
                    else
                    {
                        //Delay the loop task
                        await TaskDelay(1000, vTask_UpdateListStatus);
                    }
                }
            }
            catch { }
        }
    }
}