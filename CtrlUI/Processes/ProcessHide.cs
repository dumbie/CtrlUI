using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

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
                IntPtr processWindowHandle = await SelectProcessWindow(dataBindApp, processMulti);

                //Check if application window has been found
                if (processWindowHandle == new IntPtr(-200))
                {
                    Debug.WriteLine("Cancelled window selection.");
                }
                else if (processWindowHandle != IntPtr.Zero)
                {
                    await PrepareHideProcessWindow(dataBindApp.Name, processWindowHandle, false);
                }
                else
                {
                    Debug.WriteLine("Hide application has no window.");
                    await Notification_Send_Status("Close", "Hide application has no window");
                    //await SelectProcessAction(dataBindApp, processMulti);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to hide application: " + ex.Message);
            }
        }

        //Hide process window
        async Task PrepareHideProcessWindow(string processName, IntPtr windowHandleTarget, bool silentHide)
        {
            try
            {
                if (!vBusyChangingWindow)
                {
                    vBusyChangingWindow = true;

                    //Check if process is available
                    if (windowHandleTarget == IntPtr.Zero)
                    {
                        if (!silentHide)
                        {
                            await Notification_Send_Status("Close", "Hide application has no window");
                        }
                        Debug.WriteLine("Application cannot be hidden, window handle is empty.");
                        return;
                    }

                    //Update the interface status
                    if (!silentHide)
                    {
                        await Notification_Send_Status("AppMiniMaxi", "Hiding " + processName);
                    }
                    Debug.WriteLine("Hiding application window: " + processName + "/" + windowHandleTarget);

                    //Hide application window handle
                    bool windowFocused = await AVProcess.Hide_ProcessByWindowHandle(windowHandleTarget);
                    if (!windowFocused)
                    {
                        await Notification_Send_Status("Close", "Failed hiding application");
                        Debug.WriteLine("Failed hiding the application, no longer running?");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Close", "Failed hiding application");
                Debug.WriteLine("Failed hiding the application, no longer running? " + ex.Message);
            }
            finally
            {
                vBusyChangingWindow = false;
            }
        }
    }
}