using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
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
                        ProcessMulti foregroundProcess = Get_ProcessMultiByWindowHandle(GetForegroundWindow());
                        if (foregroundProcess == null)
                        {
                            Debug.WriteLine("No active or valid process found.");

                            //Reset the fps counter
                            ResetFpsCounter();

                            //Hide the application name and frames
                            HideApplicationNameFrames();

                            continue;
                        }

                        Debug.WriteLine("Checking process: (" + foregroundProcess.Identifier + ") " + foregroundProcess.ExeNameNoExt);

                        //Check if the foreground window has changed
                        if (vTargetProcess.Identifier == foregroundProcess.Identifier)
                        {
                            Debug.WriteLine("Foreground window has not changed.");

                            //Update the current target process
                            vTargetProcess = foregroundProcess;

                            //Update the application name
                            UpdateApplicationName(foregroundProcess.WindowTitleMain);

                            continue;
                        }

                        Debug.WriteLine("New foreground window detected (" + foregroundProcess.Identifier + ") " + foregroundProcess.ExeNameNoExt);

                        //Reset the fps counter
                        ResetFpsCounter();

                        //Update fps and crosshair overlay position and visibility
                        UpdateFpsOverlayPositionVisibility(foregroundProcess.ExeNameNoExt);
                        UpdateCrosshairOverlayPositionVisibility(foregroundProcess.ExeNameNoExt, false);

                        //Check if the foreground window is fps overlayer
                        if (vProcessCurrent.Identifier == foregroundProcess.Identifier)
                        {
                            Debug.WriteLine("Current process is fps overlayer.");

                            //Hide the application name and frames
                            HideApplicationNameFrames();
                        }
                        else
                        {
                            //Update application name
                            UpdateApplicationName(foregroundProcess.WindowTitleMain);

                            //Update windows on change
                            UpdateWindowsOnChange();
                        }

                        //Update the current target process
                        vTargetProcess = foregroundProcess;
                    }
                    catch { }
                    finally
                    {
                        //Delay the loop task
                        await TaskDelay(1000, vTask_MonitorProcess);
                    }
                }
            }
            catch { }
        }

        //Update windows on change
        void UpdateWindowsOnChange()
        {
            try
            {
                //Update fps window style
                WindowUpdateStyle(vInteropWindowHandle, true, true, true);

                //Update browser window style
                WindowUpdateStyle(vWindowBrowser.vInteropWindowHandle, true, vBrowserWindowBlockInteract, vBrowserWindowBlockInteract);

                //Update window position
                UpdateWindowPosition();
            }
            catch { }
        }

        //Hide the application name and frames
        void HideApplicationNameFrames()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    stackpanel_CurrentApp.Visibility = Visibility.Collapsed;
                    stackpanel_CurrentFps.Visibility = Visibility.Collapsed;
                    stackpanel_CurrentFrametime.Visibility = Visibility.Collapsed;
                });
            }
            catch { }
        }

        //Update the current time
        void UpdateCurrentTime()
        {
            try
            {
                string currentTimeString = string.Empty;
                bool showTime = SettingLoad(vConfigurationFpsOverlayer, "TimeShowCurrentTime", typeof(bool));
                bool showDate = SettingLoad(vConfigurationFpsOverlayer, "TimeShowCurrentDate", typeof(bool));

                if (showTime)
                {
                    currentTimeString = DateTime.Now.ToShortTimeString();
                }

                if (showDate)
                {
                    if (string.IsNullOrWhiteSpace(currentTimeString))
                    {
                        currentTimeString += DateTime.Now.ToShortDateString();
                    }
                    else
                    {
                        currentTimeString += " (" + DateTime.Now.ToShortDateString() + ")";
                    }
                }

                //Check if time string is empty
                if (string.IsNullOrWhiteSpace(currentTimeString))
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentTime.Visibility = Visibility.Collapsed;
                    });
                }
                else
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        textblock_CurrentTime.Text = currentTimeString;
                        stackpanel_CurrentTime.Visibility = Visibility.Visible;
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
                string customText = SettingLoad(vConfigurationFpsOverlayer, "CustomTextString", typeof(string));
                //Debug.WriteLine("Setting custom text: " + customText);

                if (!string.IsNullOrWhiteSpace(customText))
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        textblock_CustomText.Text = customText;
                        stackpanel_CustomText.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.DispatcherInvoke(delegate
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
                AVActions.DispatcherInvoke(delegate
                {
                    if (SettingLoad(vConfigurationFpsOverlayer, "AppShowName", typeof(bool)))
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
                //Reset the target process
                vTargetProcess = new ProcessMulti(0, 0);

                //Reset fps variables
                vLastFrameTimeStamp = 0;
                vListFrameTimes.Clear();

                //Reset frametime variables
                vFrametimeCurrent = 0;
                AVActions.DispatcherInvoke(delegate
                {
                    vPointFrameTimes.Clear();
                });

                Debug.WriteLine("Reset the frames per second counter.");
            }
            catch { }
        }
    }
}