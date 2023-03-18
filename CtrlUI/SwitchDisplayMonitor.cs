using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        private async void Btn_Monitor_HDR_Disable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await AllMonitorSwitchHDR(false, false);
            }
            catch { }
        }

        private async void Btn_Monitor_HDR_Enable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await AllMonitorSwitchHDR(true, false);
            }
            catch { }
        }

        //Move application to the next monitor
        private async void Btn_Monitor_Move_App_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check if there are multiple monitors
                int totalScreenCount = Screen.AllScreens.Count();
                if (totalScreenCount == 1)
                {
                    Debug.WriteLine("Only one monitor");
                    await Notification_Send_Status("MonitorNext", "Only one monitor");

                    //Save the new monitor number
                    SettingSave(vConfigurationCtrlUI, "DisplayMonitor", "1");

                    //Update window style
                    WindowUpdateStyle(vInteropWindowHandle, true, false, false);

                    //Update window position
                    await UpdateWindowPosition(true, false);
                    return;
                }

                //Check the next target monitor
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));
                if (monitorNumber >= totalScreenCount)
                {
                    monitorNumber = 1;
                }
                else
                {
                    monitorNumber++;
                }

                //Save the new monitor number
                SettingSave(vConfigurationCtrlUI, "DisplayMonitor", monitorNumber.ToString());

                //Update window style
                WindowUpdateStyle(vInteropWindowHandle, true, false, false);

                //Update window position
                await UpdateWindowPosition(true, false);

                //Focus on CtrlUI window
                await AppWindowShow(true, true);
            }
            catch { }
        }

        private async void Btn_Monitor_Switch_Extend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Notification_Send_Status("MonitorSwitch", "Extending display monitor");

                //Enable monitor extend mode
                EnableMonitorExtendMode();

                //Focus on CtrlUI window
                await AppWindowShow(true, true);
            }
            catch { }
        }

        private async void Btn_Monitor_Switch_Duplicate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Notification_Send_Status("MonitorSwitch", "Duplicating display monitor");

                //Enable monitor clone mode
                EnableMonitorCloneMode();

                //Focus on CtrlUI window
                await AppWindowShow(true, true);
            }
            catch { }
        }

        private async void Btn_Monitor_Switch_Secondary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Notification_Send_Status("MonitorSwitch", "Switching secondary monitor");

                //Switch secondary monitor
                EnableMonitorSecond();

                //Focus on CtrlUI window
                await AppWindowShow(true, true);
            }
            catch { }
        }

        private async void Btn_Monitor_Switch_Primary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Notification_Send_Status("MonitorSwitch", "Switching primary monitor");

                //Switch primary monitor
                EnableMonitorFirst();

                //Focus on CtrlUI window
                await AppWindowShow(true, true);
            }
            catch { }
        }

        //Enable monitor HDR
        async Task EnableHDRDatabindAuto(DataBindApp dataBindApp)
        {
            try
            {
                if (dataBindApp.LaunchEnableHDR)
                {
                    await AllMonitorSwitchHDR(true, true);
                }
            }
            catch { }
        }

        //Enable or disable HDR
        async Task AllMonitorSwitchHDR(bool enableHDR, bool waitHDR)
        {
            try
            {
                if (enableHDR)
                {
                    await Notification_Send_Status("MonitorHDR", "Enabling monitor HDR");
                    Debug.WriteLine("Enabling monitor HDR.");
                }
                else
                {
                    await Notification_Send_Status("MonitorHDR", "Disabling monitor HDR");
                    Debug.WriteLine("Disabling monitor HDR.");
                }

                //Switch hdr for all monitors
                int screenCount = Screen.AllScreens.Count();
                for (int i = 0; i < screenCount; i++)
                {
                    SetMonitorHDR(i, enableHDR);
                }

                //Wait for hdr to have enabled
                if (waitHDR)
                {
                    await Task.Delay(500);
                }
            }
            catch
            {
                Debug.WriteLine("Failed to switch monitor HDR.");
                await Notification_Send_Status("MonitorHDR", "Failed switching HDR");
            }
        }

        //Prevent or allow monitor sleep
        void UpdateMonitorSleepAuto()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    if (SettingLoad(vConfigurationCtrlUI, "MonitorPreventSleep", typeof(bool)))
                    {
                        Debug.WriteLine("Preventing monitor to sleep.");
                        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED);
                    }
                    else
                    {
                        Debug.WriteLine("Allowing monitor to sleep.");
                        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
                    }
                });
            }
            catch { }
        }

        //Allow monitor sleep
        void UpdateMonitorSleepAllow()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    Debug.WriteLine("Allowing monitor to sleep.");
                    SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
                });
            }
            catch { }
        }
    }
}