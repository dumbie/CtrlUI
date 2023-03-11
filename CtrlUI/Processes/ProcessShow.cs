using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVImage;
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
                Debug.WriteLine("Showing the application: " + dataBindApp.Name);

                //Check if application has multiple windows
                IntPtr processWindowHandle = await CheckProcessWindowsAuto(dataBindApp, processMulti);

                //Check if application window has been found
                if (processWindowHandle == new IntPtr(-50))
                {
                    await LaunchProcessDatabindAuto(dataBindApp);
                }
                else if (processWindowHandle == new IntPtr(-75))
                {
                    await RestartProcessAuto(processMulti, dataBindApp, true, false, false);
                }
                else if (processWindowHandle == new IntPtr(-80))
                {
                    await RestartProcessAuto(processMulti, dataBindApp, false, true, false);
                }
                else if (processWindowHandle == new IntPtr(-85))
                {
                    await RestartProcessAuto(processMulti, dataBindApp, false, false, true);
                }
                else if (processWindowHandle == new IntPtr(-100))
                {
                    await CloseSingleProcessAuto(processMulti, dataBindApp, true, false);
                }
                else if (processWindowHandle == new IntPtr(-200))
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
                    await PrepareShowProcessWindow(dataBindApp.Name, processMulti.Identifier, processWindowHandle, true, false, keyboardLaunch);
                }
                else
                {
                    Debug.WriteLine("Show application has no window.");

                    //Set messagebox answers
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString AnswerLaunch = new DataBindString();
                    AnswerLaunch.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerLaunch.Name = "Launch new instance";
                    Answers.Add(AnswerLaunch);

                    //Check if processmulti is available
                    string launchInformation = string.Empty;
                    DataBindString AnswerRestartCurrent = new DataBindString();
                    DataBindString AnswerRestartDefault = new DataBindString();
                    DataBindString AnswerRestartWithout = new DataBindString();
                    DataBindString AnswerClose = new DataBindString();
                    if (processMulti != null)
                    {
                        bool currentArgument = !string.IsNullOrWhiteSpace(processMulti.Argument);
                        if (currentArgument)
                        {
                            AnswerRestartCurrent.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            AnswerRestartCurrent.Name = "Restart application";
                            AnswerRestartCurrent.NameSub = "(Current argument)";
                            Answers.Add(AnswerRestartCurrent);
                        }

                        bool availableArgument = !string.IsNullOrWhiteSpace(dataBindApp.Argument);
                        bool emulatorArgument = dataBindApp.Category == AppCategory.Emulator && !dataBindApp.LaunchSkipRom;
                        bool filepickerArgument = dataBindApp.Category != AppCategory.Emulator && dataBindApp.LaunchFilePicker;
                        bool defaultArgument = availableArgument || emulatorArgument || filepickerArgument;
                        if (defaultArgument)
                        {
                            AnswerRestartDefault.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            AnswerRestartDefault.Name = "Restart application";
                            AnswerRestartDefault.NameSub = "(Default argument)";
                            Answers.Add(AnswerRestartDefault);
                        }

                        AnswerRestartWithout.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        AnswerRestartWithout.Name = "Restart application";
                        AnswerRestartWithout.NameSub = "(Without argument)";
                        Answers.Add(AnswerRestartWithout);

                        AnswerClose.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        AnswerClose.Name = "Close application";
                        Answers.Add(AnswerClose);

                        //Get launch information
                        if (processMulti.Type == ProcessType.UWP || processMulti.Type == ProcessType.Win32Store)
                        {
                            launchInformation = processMulti.AppUserModelId;
                        }
                        else
                        {
                            launchInformation = processMulti.ExePath;
                        }

                        //Add process identifier
                        if (processMulti.Identifier != 0)
                        {
                            launchInformation += " (" + processMulti.Identifier + ")";
                        }

                        //Add launch argument
                        if (currentArgument)
                        {
                            launchInformation += "\nCurrent argument: " + AVFunctions.StringCut(processMulti.Argument, 50, "...");
                        }
                        if (defaultArgument)
                        {
                            if (emulatorArgument)
                            {
                                launchInformation += "\nDefault argument: Select a rom";
                            }
                            else if (filepickerArgument)
                            {
                                launchInformation += "\nDefault argument: Select a file";
                            }
                            else
                            {
                                launchInformation += "\nDefault argument: " + dataBindApp.Argument;
                            }
                        }
                    }

                    //Show the messagebox
                    DataBindString messageResult = await Popup_Show_MessageBox("Application has no window", launchInformation, "", Answers);
                    if (messageResult != null)
                    {
                        if (messageResult == AnswerClose)
                        {
                            await CloseSingleProcessAuto(processMulti, dataBindApp, true, false);
                        }
                        else if (messageResult == AnswerRestartCurrent)
                        {
                            await RestartProcessAuto(processMulti, dataBindApp, true, false, false);
                        }
                        else if (messageResult == AnswerRestartDefault)
                        {
                            await RestartProcessAuto(processMulti, dataBindApp, false, true, false);
                        }
                        else if (messageResult == AnswerRestartWithout)
                        {
                            await RestartProcessAuto(processMulti, dataBindApp, false, false, true);
                        }
                        else if (messageResult == AnswerLaunch)
                        {
                            await LaunchProcessDatabindAuto(dataBindApp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to show application: " + ex.Message);
            }
        }

        //Show process window
        async Task PrepareShowProcessWindow(string processName, int processIdTarget, IntPtr windowHandleTarget, bool minimizeCtrlUI, bool silentFocus, bool launchKeyboard)
        {
            try
            {
                if (!vBusyChangingWindow)
                {
                    vBusyChangingWindow = true;

                    //Check if process is available
                    if (windowHandleTarget == null)
                    {
                        if (!silentFocus) { await Notification_Send_Status("Close", "Application no longer running"); }
                        Debug.WriteLine("Show application no longer seems to be running.");
                        return;
                    }

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
                    Debug.WriteLine("Showing application window: " + processName + "/" + processIdTarget + "/" + windowHandleTarget);

                    //Minimize the CtrlUI window
                    if (minimizeCtrlUI)
                    {
                        await AppWindowMinimize(true, true);
                    }

                    //Focus on application window handle
                    bool windowFocused = await AVProcess.Show_ProcessByProcessIdAndWindowHandle(processIdTarget, windowHandleTarget);
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