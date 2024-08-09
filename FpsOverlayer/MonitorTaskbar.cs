﻿using ArnoldVinkCode;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVTaskbarInformation;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        void StartMonitorTaskbar()
        {
            try
            {
                AVActions.TaskStartLoop(LoopMonitorTaskbar, vTask_MonitorTaskbar);
                Debug.WriteLine("Started monitoring taskbar.");
            }
            catch { }
        }

        async Task LoopMonitorTaskbar()
        {
            try
            {
                while (await TaskCheckLoop(vTask_MonitorTaskbar, 1000))
                {
                    try
                    {
                        //Check taskbar setting
                        if (!SettingLoad(vConfigurationFpsOverlayer, "CheckTaskbarVisible", typeof(bool)))
                        {
                            vTaskBarAdjustMargin = 0;
                            continue;
                        }

                        //Check taskbar visibility
                        AVTaskbarInformation taskbarInfo = new AVTaskbarInformation();
                        vTaskBarPosition = taskbarInfo.Position;

                        //Check if auto hide is enabled
                        if (taskbarInfo.IsAutoHide && taskbarInfo.IsVisible)
                        {
                            //Get the current active screen
                            int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));
                            DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);

                            //Get the current taskbar size
                            int taskbarSize = 0;
                            if (vTaskBarPosition == AppBarPosition.ABE_TOP || vTaskBarPosition == AppBarPosition.ABE_BOTTOM)
                            {
                                taskbarSize = (int)(taskbarInfo.Bounds.Height / displayMonitorSettings.DpiScaleVertical);
                            }
                            else
                            {
                                taskbarSize = (int)(taskbarInfo.Bounds.Width / displayMonitorSettings.DpiScaleHorizontal);
                            }

                            //Check the taskbar margin
                            if (vTaskBarAdjustMargin != taskbarSize)
                            {
                                vTaskBarAdjustMargin = taskbarSize;

                                //Update fps overlay position and visibility
                                UpdateFpsOverlayPositionVisibility(vTargetProcess.ExeNameNoExt);
                            }
                        }
                        else
                        {
                            //Check the taskbar margin
                            if (vTaskBarAdjustMargin != 0)
                            {
                                vTaskBarAdjustMargin = 0;

                                //Update fps overlay position and visibility
                                UpdateFpsOverlayPositionVisibility(vTargetProcess.ExeNameNoExt);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}