using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        void StartMonitorProcess()
        {
            try
            {
                vTaskToken_MonitorProcess = new CancellationTokenSource();
                vTask_MonitorProcess = AVActions.TaskStart(MonitorProcess, vTaskToken_MonitorProcess);

                Debug.WriteLine("Started monitoring processes.");
            }
            catch { }
        }

        async void MonitorProcess()
        {
            try
            {
                while (TaskRunningCheck(vTaskToken_MonitorProcess))
                {
                    try
                    {
                        //Update the current time
                        UpdateCurrentTime();

                        //Get and check the focused process
                        ProcessMulti foregroundProcess = GetFocusedProcess();
                        if (foregroundProcess == null)
                        {
                            Debug.WriteLine("No active or valid process found.");

                            //Reset the fps counter
                            ResetFpsCounter();

                            //Hide the application name and frames
                            HideApplicationNameFrames();

                            await Task.Delay(1000);
                            continue;
                        }

                        Debug.WriteLine("Checking process: (" + foregroundProcess.Identifier + ") " + foregroundProcess.Name);

                        //Check if the foreground window has changed
                        if (vTargetProcess.Identifier == foregroundProcess.Identifier)
                        {
                            Debug.WriteLine("Foreground window has not changed.");

                            //Update the current target process
                            vTargetProcess = foregroundProcess;

                            //Update the application name
                            UpdateApplicationName(foregroundProcess.Title);

                            await Task.Delay(1000);
                            continue;
                        }

                        Debug.WriteLine("New foreground window detected (" + foregroundProcess.Identifier + ") " + foregroundProcess.Name);

                        //Reset the fps counter
                        ResetFpsCounter();

                        //Update the stats text position
                        UpdateWindowTextPosition(foregroundProcess.Name);

                        //Check if the foreground window is fps overlayer
                        if (vProcessCurrent.Id == foregroundProcess.Identifier)
                        {
                            Debug.WriteLine("Current process is fps overlayer.");

                            //Hide the application name and frames
                            HideApplicationNameFrames();

                            await Task.Delay(1000);
                            continue;
                        }

                        //Update the application name
                        UpdateApplicationName(foregroundProcess.Title);

                        //Update the current target process
                        vTargetProcess = foregroundProcess;
                    }
                    catch { }
                    await Task.Delay(1000);
                }
            }
            catch { }
        }

        //Hide the application name and frames
        void HideApplicationNameFrames()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    stackpanel_CurrentApp.Visibility = Visibility.Collapsed;
                    stackpanel_CurrentFps.Visibility = Visibility.Collapsed;
                });
            }
            catch { }
        }

        //Update the current time
        void UpdateCurrentTime()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["TimeShowCurrentTime"]))
                    {
                        textblock_CurrentTime.Text = DateTime.Now.ToShortTimeString();
                        stackpanel_CurrentTime.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackpanel_CurrentTime.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch { }
        }

        //Update the application name
        void UpdateApplicationName(string processTitle)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["AppShowName"]))
                    {
                        if (!string.IsNullOrWhiteSpace(processTitle) && processTitle != "Unknown")
                        {
                            textblock_CurrentApp.Text = processTitle;
                            stackpanel_CurrentApp.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            stackpanel_CurrentApp.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        stackpanel_CurrentApp.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch { }
        }

        void ResetFpsCounter()
        {
            try
            {
                //Debug.WriteLine("Resetting the frames per second counter.");

                //Reset the target process
                vTargetProcess = new ProcessMulti();

                //Reset the frames variables
                vLastFrameTimeStamp = 0;
                vListFrameTime.Clear();
            }
            catch { }
        }
    }
}