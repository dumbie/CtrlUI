﻿using ArnoldVinkCode;
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
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show process window
        async Task ShowProcessWindowAuto(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                Debug.WriteLine("Showing the application: " + dataBindApp.Name + " / Identifier: " + processMulti.Identifier);

                //Check if application has multiple windows
                ProcessWindowAction windowAction = await SelectProcessWindow(dataBindApp, processMulti, false);

                //Check if application window has been found
                if (windowAction.Action == ProcessWindowActions.Single)
                {
                    //Check keyboard controller launch
                    string fileNameNoExtension = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                    bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower() || x.String1.ToLower() == dataBindApp.PathExe.ToLower() || x.String1.ToLower() == dataBindApp.AppUserModelId.ToLower());
                    bool keyboardLaunch = (keyboardProcess || dataBindApp.LaunchKeyboard) && vControllerAnyConnected();

                    //Focus on application window
                    await ShowProcessWindow(dataBindApp.Name, windowAction.WindowHandle, true, false, keyboardLaunch);
                }
                else if (windowAction.Action == ProcessWindowActions.Cancel)
                {
                    Debug.WriteLine("Cancelled window selection.");
                }
                else
                {
                    Debug.WriteLine("Show application has no window.");
                    await Notification_Send_Status("Close", "Application has no window");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to show application: " + ex.Message);
            }
        }

        //Show process window
        async Task ShowProcessWindow(string processName, IntPtr windowHandleTarget, bool minimizeCtrlUI, bool skipNotification, bool launchKeyboard)
        {
            try
            {
                //Check if process is available
                if (windowHandleTarget == IntPtr.Zero)
                {
                    if (!skipNotification)
                    {
                        await Notification_Send_Status("Close", "Application cannot be shown");
                    }
                    Debug.WriteLine("Application cannot be shown, window handle is empty.");
                    return;
                }

                Debug.WriteLine("Showing application window: " + processName + "/" + windowHandleTarget);

                //Update the interface status
                if (!skipNotification)
                {
                    await Notification_Send_Status("AppMiniMaxi", "Showing " + processName);
                }

                //Minimize CtrlUI window
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
            catch (Exception ex)
            {
                await Notification_Send_Status("Close", "Failed showing application");
                Debug.WriteLine("Failed showing the application, no longer running? " + ex.Message);
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
                    socketSend.SetObject("KeyboardShow");
                }
                else
                {
                    socketSend.SetObject("KeyboardHideShow");
                }

                //Request controller status
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), 26760);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }
    }
}