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

        private async void Btn_Monitor_Switch_Extend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Notification_Show_Status("MonitorSwitch", "Extending display monitor");

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
                Notification_Show_Status("MonitorSwitch", "Duplicating display monitor");

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
                Notification_Show_Status("MonitorSwitch", "Switching secondary monitor");

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
                Notification_Show_Status("MonitorSwitch", "Switching primary monitor");

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
                if (dataBindApp.LaunchEnableDisplayHDR || dataBindApp.LaunchEnableAutoHDR)
                {
                    //Enable monitor HDR
                    await AllMonitorSwitchHDR(true, false);
                }

                if (dataBindApp.LaunchEnableAutoHDR)
                {
                    //Enable Windows auto HDR feature
                    EnableWindowsAutoHDRFeature();

                    //Allow auto HDR for application
                    EnableApplicationAutoHDR(dataBindApp);
                }

                //Wait for HDR initialization
                if (dataBindApp.LaunchEnableDisplayHDR || dataBindApp.LaunchEnableAutoHDR)
                {
                    await Task.Delay(500);
                }
            }
            catch { }
        }

        //Enable or disable monitor HDR
        async Task AllMonitorSwitchHDR(bool enableHDR, bool waitHDR)
        {
            try
            {
                if (enableHDR)
                {
                    Notification_Show_Status("MonitorHDR", "Enabling monitor HDR");
                    Debug.WriteLine("Enabling monitor HDR.");
                }
                else
                {
                    Notification_Show_Status("MonitorHDR", "Disabling monitor HDR");
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
                Notification_Show_Status("MonitorHDR", "Failed switching HDR");
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