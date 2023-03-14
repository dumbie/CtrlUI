using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show process window
        async Task ShowProcessWindowAuto(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                Debug.WriteLine("Showing the application: " + dataBindApp.Name);

                //Check if application has multiple windows
                IntPtr processWindowHandle = await SelectProcessWindow(dataBindApp, processMulti);

                //Check if application window has been found
                if (processWindowHandle == new IntPtr(-200))
                {
                    Debug.WriteLine("Cancelled window selection.");
                }
                else if (processWindowHandle != IntPtr.Zero)
                {
                    //Check keyboard controller launch
                    string fileNameNoExtension = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                    bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower() || x.String1.ToLower() == dataBindApp.PathExe.ToLower() || x.String1.ToLower() == dataBindApp.AppUserModelId.ToLower());
                    bool keyboardLaunch = (keyboardProcess || dataBindApp.LaunchKeyboard) && vControllerAnyConnected();

                    //Force focus on the app
                    await PrepareShowProcessWindow(dataBindApp.Name, processWindowHandle, true, false, keyboardLaunch);
                }
                else
                {
                    Debug.WriteLine("Show application has no window.");
                    await Notification_Send_Status("Close", "Show application has no window");
                    //await SelectProcessAction(dataBindApp, processMulti);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to show application: " + ex.Message);
            }
        }

        //Show process window
        async Task PrepareShowProcessWindow(string processName, IntPtr windowHandleTarget, bool minimizeCtrlUI, bool silentFocus, bool launchKeyboard)
        {
            try
            {
                if (!vBusyChangingWindow)
                {
                    vBusyChangingWindow = true;

                    //Check if process is available
                    if (windowHandleTarget == IntPtr.Zero)
                    {
                        if (!silentFocus) { await Notification_Send_Status("Close", "Application cannot be shown"); }
                        Debug.WriteLine("Application cannot be shown, window handle is empty.");
                        return;
                    }

                    //Update the interface status
                    if (!silentFocus)
                    {
                        if (!(processName.ToLower() == "ctrlui" && vAppActivated))
                        {
                            await Notification_Send_Status("AppMiniMaxi", "Showing " + processName);
                        }
                    }
                    Debug.WriteLine("Showing application window: " + processName + "/" + windowHandleTarget);

                    //Minimize the CtrlUI window
                    if (minimizeCtrlUI)
                    {
                        await AppWindowMinimize(true, true);
                    }

                    //Focus on application window handle
                    bool windowFocused = await AVProcess.Show_ProcessByWindowHandle(windowHandleTarget);
                    if (!windowFocused)
                    {
                        await Notification_Send_Status("Close", "Failed showing application");
                        Debug.WriteLine("Failed showing the application, no longer running?");
                        return;
                    }

                    //Launch the keyboard controller
                    if (launchKeyboard)
                    {
                        await ShowHideKeyboardController(true);
                    }
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Close", "Failed showing application");
                Debug.WriteLine("Failed showing the application, no longer running? " + ex.Message);
            }
            finally
            {
                vBusyChangingWindow = false;
            }
        }

        //Show or hide the keyboard controller
        async Task ShowHideKeyboardController(bool forceShow)
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                if (forceShow)
                {
                    socketSend.Object = "KeyboardShow";
                }
                else
                {
                    socketSend.Object = "KeyboardHideShow";
                }

                //Request controller status
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), vArnoldVinkSockets.vSocketServerPort + 1);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }
    }
}