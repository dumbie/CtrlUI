using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVProcess;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Hide process window
        async Task HideProcessWindowAuto(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                Debug.WriteLine("Hiding the application: " + dataBindApp.Name);

                //Check if application has multiple windows
                ProcessWindowAction windowAction = await SelectProcessWindow(dataBindApp, processMulti, true);

                //Check if application window has been found
                if (windowAction.Action == ProcessWindowActions.Single)
                {
                    await HideProcessWindow(dataBindApp.Name, windowAction.WindowHandle, true, false);
                }
                else if (windowAction.Action == ProcessWindowActions.Multiple)
                {
                    await HideAllProcessWindows(dataBindApp.Name, windowAction.WindowHandles, true, false);
                }
                else if (windowAction.Action == ProcessWindowActions.Cancel)
                {
                    Debug.WriteLine("Cancelled window selection.");
                }
                else
                {
                    Debug.WriteLine("Hide application has no window.");
                    await Notification_Send_Status("Close", "Hide application has no window");
                }

                //Focus on CtrlUI window
                await AppWindowShow(true, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to hide application: " + ex.Message);
            }
        }

        //Hide process window
        async Task HideProcessWindow(string processName, IntPtr windowHandleTarget, bool hideDelay, bool skipNotification)
        {
            try
            {
                //Check if window is available
                if (windowHandleTarget == IntPtr.Zero)
                {
                    if (!skipNotification)
                    {
                        await Notification_Send_Status("Close", "Hide application has no window");
                    }
                    Debug.WriteLine("Application cannot be hidden, window handle is empty.");
                    return;
                }

                //Update the interface status
                if (!skipNotification)
                {
                    await Notification_Send_Status("AppMinimize", "Hiding " + processName);
                }
                Debug.WriteLine("Hiding application window: " + processName + "/" + windowHandleTarget);

                //Hide application window handle
                bool windowHidden = await AVProcess.Hide_ProcessByWindowHandle(windowHandleTarget);
                if (!windowHidden)
                {
                    await Notification_Send_Status("Close", "Failed hiding application");
                    Debug.WriteLine("Failed hiding the application, no longer running?");
                    return;
                }

                //Wait for process to hide
                if (hideDelay)
                {
                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Close", "Failed hiding application");
                Debug.WriteLine("Failed hiding the application, no longer running? " + ex.Message);
            }
        }

        //Hide all process windows
        async Task HideAllProcessWindows(string processName, List<IntPtr> windowHandleTargets, bool hideDelay, bool skipNotification)
        {
            try
            {
                //Check if window is available
                if (!windowHandleTargets.Any())
                {
                    if (!skipNotification)
                    {
                        await Notification_Send_Status("Close", "Hide application has no window");
                    }
                    Debug.WriteLine("Application cannot be hidden, window handle is empty.");
                    return;
                }

                //Update the interface status
                if (!skipNotification)
                {
                    await Notification_Send_Status("AppMinimize", "Hiding all " + processName);
                }
                Debug.WriteLine("Hiding all application windows: " + processName);

                //Hide application window handles
                bool windowHidden = true;
                foreach (IntPtr windowHandle in windowHandleTargets)
                {
                    try
                    {
                        bool hideResult = await AVProcess.Hide_ProcessByWindowHandle(windowHandle);
                        if (windowHidden)
                        {
                            windowHidden = hideResult;
                        }
                    }
                    catch { }
                }

                if (!windowHidden)
                {
                    await Notification_Send_Status("Close", "Failed hiding application");
                    Debug.WriteLine("Failed hiding the application, no longer running?");
                    return;
                }

                //Wait for process to hide
                if (hideDelay)
                {
                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Close", "Failed hiding application");
                Debug.WriteLine("Failed hiding the application, no longer running? " + ex.Message);
            }
        }
    }
}