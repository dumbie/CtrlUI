using ArnoldVinkCode;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVTaskbarInformation;
using static DirectXInput.AppVariables;

namespace DirectXInput.KeypadCode
{
    partial class WindowKeypad
    {
        async Task vTaskLoop_MonitorTaskbar()
        {
            try
            {
                while (TaskCheckLoop(vTask_MonitorTaskbar))
                {
                    try
                    {
                        //Check taskbar visibility
                        AVTaskbarInformation taskbarInfo = new AVTaskbarInformation();

                        //Check if auto hide is enabled
                        if (taskbarInfo.IsAutoHide && taskbarInfo.IsVisible)
                        {
                            //Get the current active screen
                            int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));
                            DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);

                            //Get the current taskbar size
                            int taskbarSize = 0;
                            if (taskbarInfo.Position == AppBarPosition.ABE_BOTTOM)
                            {
                                AVActions.DispatcherInvoke(delegate
                                {
                                    try
                                    {
                                        //Update taskbar margin
                                        taskbarSize = (int)(taskbarInfo.Bounds.Height / displayMonitorSettings.DpiScaleVertical);
                                        grid_Application.Margin = new Thickness(0, 0, 0, taskbarSize);
                                    }
                                    catch { }
                                });
                                continue;
                            }
                        }

                        //Reset taskbar margin
                        AVActions.DispatcherInvoke(delegate
                        {
                            try
                            {
                                grid_Application.Margin = new Thickness(0);
                            }
                            catch { }
                        });
                    }
                    catch { }
                    finally
                    {
                        //Delay the loop task
                        await TaskDelay(1000, vTask_MonitorTaskbar);
                    }
                }
            }
            catch { }
        }
    }
}