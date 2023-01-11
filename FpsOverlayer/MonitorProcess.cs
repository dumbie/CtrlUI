using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVWindowFunctions;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        void StartMonitorProcess()
        {
            try
            {
                AVActions.TaskStartLoop(LoopMonitorProcess, vTask_MonitorProcess);
                Debug.WriteLine("Started monitoring processes.");
            }
            catch { }
        }

        async Task LoopMonitorProcess()
        {
            try
            {
                while (TaskCheckLoop(vTask_MonitorProcess))
                {
                    try
                    {
                        //Update the current time
                        UpdateCurrentTime();

                        //Update custom text
                        UpdateCustomText();

                        //Get and check the focused process
                        ProcessMulti foregroundProcess = GetProcessMultiFromWindowHandle(GetForegroundWindow());
                        if (foregroundProcess == null)
                        {
                            Debug.WriteLine("No active or valid process found.");

                            //Reset the fps counter
                            ResetFpsCounter();

                            //Hide the application name and frames
                            HideApplicationNameFrames();

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
                            UpdateApplicationName(foregroundProcess.WindowTitle);

                            continue;
                        }

                        Debug.WriteLine("New foreground window detected (" + foregroundProcess.Identifier + ") " + foregroundProcess.Name);

                        //Reset the fps counter
                        ResetFpsCounter();

                        //Update the fps overlayer position
                        UpdateFpsOverlayPosition(foregroundProcess.Name);

                        //Check if the foreground window is fps overlayer
                        if (vProcessCurrent.Id == foregroundProcess.Identifier)
                        {
                            Debug.WriteLine("Current process is fps overlayer.");

                            //Hide the application name and frames
                            HideApplicationNameFrames();
                        }
                        else
                        {
                            //Update the application name
                            UpdateApplicationName(foregroundProcess.WindowTitle);

                            //Update fps window style
                            if (vWindowVisible)
                            {
                                await WindowUpdateStyleVisible(vInteropWindowHandle, true, true, true);
                            }

                            //Update browser window style
                            if (vWindowBrowser.vWindowVisible)
                            {
                                await WindowUpdateStyleVisible(vWindowBrowser.vInteropWindowHandle, true, true, vBrowserWindowClickThrough);
                            }
                        }

                        //Update the current target process
                        vTargetProcess = foregroundProcess;
                    }
                    catch { }
                    finally
                    {
                        //Delay the loop task
                        await TaskDelayLoop(1000, vTask_MonitorProcess);
                    }
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
                if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "TimeShowCurrentTime")))
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_CurrentTime.Text = DateTime.Now.ToShortTimeString();
                        stackpanel_CurrentTime.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        stackpanel_CurrentTime.Visibility = Visibility.Collapsed;
                    });
                }
            }
            catch { }
        }

        //Update custom text
        void UpdateCustomText()
        {
            try
            {
                string customText = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "CustomTextString"));
                //Debug.WriteLine("Setting custom text: " + customText);

                if (!string.IsNullOrWhiteSpace(customText))
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_CustomText.Text = customText;
                        stackpanel_CustomText.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        stackpanel_CustomText.Visibility = Visibility.Collapsed;
                    });
                }
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
                    if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "AppShowName")))
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