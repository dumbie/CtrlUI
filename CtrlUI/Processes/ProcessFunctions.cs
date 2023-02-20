using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Focus on a process window
        async Task PrepareFocusProcessWindow(string processName, int processIdTarget, IntPtr windowHandleTarget, bool minimizeCtrlUI, bool silentFocus, bool launchKeyboard)
        {
            try
            {
                if (!vBusyChangingWindow)
                {
                    vBusyChangingWindow = true;

                    //Check if process is available
                    if (windowHandleTarget == null)
                    {
                        if (!silentFocus) { await Notification_Send_Status("Close", "App no longer running"); }
                        Debug.WriteLine("Show application no longer seems to be running.");
                        return;
                    }

                    //Check if process is available
                    if (windowHandleTarget == IntPtr.Zero)
                    {
                        if (!silentFocus) { await Notification_Send_Status("Close", "App can't be shown"); }
                        Debug.WriteLine("Application can't be shown, window handle is empty.");
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

                    //Focus on application window handle
                    bool windowFocused = AVProcessTool.Show_ProcessIdHwnd(processIdTarget, windowHandleTarget);
                    if (!windowFocused)
                    {
                        await Notification_Send_Status("Close", "Failed showing application");
                        Debug.WriteLine("Failed showing the application, no longer running?");
                        return;
                    }

                    //Minimize the CtrlUI window
                    if (minimizeCtrlUI)
                    {
                        await AppWindowMinimize(true, true);
                    }

                    //Launch the keyboard controller
                    if (launchKeyboard)
                    {
                        await KeyboardControllerHideShow(true);
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

        //Check which launch method needs to be used
        async Task LaunchProcessSelector(DataBindApp dataBindApp)
        {
            try
            {
                //Check if the shortcut is available
                if (dataBindApp.Category == AppCategory.Shortcut && dataBindApp.StatusAvailable == Visibility.Visible)
                {
                    Debug.WriteLine("Remove shortcut prompt: " + dataBindApp.ShortcutPath);
                    await RemoveShortcutFilePrompt(dataBindApp);
                    return;
                }

                //Check the application category
                if (dataBindApp.Category == AppCategory.Process)
                {
                    //Get the process multi
                    ProcessMulti processMulti = null;
                    foreach (ProcessMulti processMultiFor in dataBindApp.ProcessMulti)
                    {
                        if (processMultiFor.WindowHandle != IntPtr.Zero)
                        {
                            processMulti = processMultiFor;
                        }
                    }

                    //Show the process window
                    await ShowProcessWindow(dataBindApp, processMulti);
                }
                else
                {
                    //Run process url protocol
                    if (Check_PathUrlProtocol(dataBindApp.PathExe))
                    {
                        await PrepareProcessLauncherUrlProtocolAsync(dataBindApp, false, false, false);
                        return;
                    }

                    //Refresh the processes list
                    await Notification_Send_Status("AppLaunch", "Preparing launch");
                    await RefreshListProcessesWithWait(true);

                    //Check if process is running
                    ProcessMulti processMulti = await SelectProcessMulti(dataBindApp, true);
                    if (processMulti == null)
                    {
                        Debug.WriteLine("Process is not running, launching the application.");
                        await LaunchProcessDatabindAuto(dataBindApp);
                    }
                    else if (processMulti.Action == "Cancel")
                    {
                        Debug.WriteLine("Process is already running, skipping the launch.");
                    }
                    else if (processMulti.Action == "CloseAll")
                    {
                        Debug.WriteLine("Closing all processes, skipping the launch.");
                        await CloseAllProcessesAuto(processMulti, dataBindApp, true, false);
                    }
                    else
                    {
                        Debug.WriteLine("Process is already running, checking the app.");
                        await CheckLaunchProcessStatus(dataBindApp, processMulti);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Failed launch process selector.");
            }
        }

        async Task ShowProcessWindow(DataBindApp dataBindApp, ProcessMulti processMulti)
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
                    await RestartProcessAuto(processMulti, dataBindApp, true);
                }
                else if (processWindowHandle == new IntPtr(-80))
                {
                    await RestartProcessAuto(processMulti, dataBindApp, false);
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
                    bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower() || x.String1.ToLower() == dataBindApp.PathExe.ToLower());
                    bool keyboardLaunch = (keyboardProcess || dataBindApp.LaunchKeyboard) && vControllerAnyConnected();

                    //Force focus on the app
                    await PrepareFocusProcessWindow(dataBindApp.Name, processMulti.Identifier, processWindowHandle, true, false, keyboardLaunch);
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
                    DataBindString AnswerRestartCurrent = new DataBindString();
                    DataBindString AnswerRestartWithout = new DataBindString();
                    DataBindString AnswerClose = new DataBindString();
                    if (processMulti != null)
                    {
                        if (!string.IsNullOrWhiteSpace(processMulti.Argument))
                        {
                            AnswerRestartCurrent.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            AnswerRestartCurrent.Name = "Restart application";
                            AnswerRestartCurrent.NameSub = "(Current argument)";
                            Answers.Add(AnswerRestartCurrent);
                        }

                        AnswerRestartWithout.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        AnswerRestartWithout.Name = "Restart application";
                        if (!string.IsNullOrWhiteSpace(dataBindApp.Argument) || dataBindApp.Category == AppCategory.Shortcut || dataBindApp.Category == AppCategory.Emulator || dataBindApp.LaunchFilePicker)
                        {
                            AnswerRestartWithout.NameSub = "(Default argument)";
                        }
                        else
                        {
                            AnswerRestartWithout.NameSub = "(Without argument)";
                        }
                        Answers.Add(AnswerRestartWithout);

                        AnswerClose.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        AnswerClose.Name = "Close application";
                        Answers.Add(AnswerClose);
                    }

                    //Show the messagebox
                    DataBindString messageResult = await Popup_Show_MessageBox("Application has no window", "", "", Answers);
                    if (messageResult != null)
                    {
                        if (messageResult == AnswerClose)
                        {
                            await CloseSingleProcessAuto(processMulti, dataBindApp, true, false);
                        }
                        else if (messageResult == AnswerRestartCurrent)
                        {
                            await RestartProcessAuto(processMulti, dataBindApp, true);
                        }
                        else if (messageResult == AnswerRestartWithout)
                        {
                            await RestartProcessAuto(processMulti, dataBindApp, false);
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

        //Show failed launch messagebox
        async Task LaunchProcessFailed()
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Ok";
                Answers.Add(Answer1);

                await Popup_Show_MessageBox("Failed to launch the application", "", "Perhaps the application has moved to a new location or has closed.", Answers);
            }
            catch { }
        }

        //Run an selected executable file
        async Task RunExecutableFile()
        {
            try
            {
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "exe", "bat", "cmd" };
                vFilePickerSettings.Title = "Launch Executable";
                vFilePickerSettings.Description = "Please select an executable file to run:";
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Check keyboard controller launch
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(vFilePickerResult.PathFile);
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower());
                bool keyboardLaunch = keyboardProcess && vControllerAnyConnected();

                //Launch the Win32 application
                await PrepareProcessLauncherWin32Async(fileNameNoExtension, vFilePickerResult.PathFile, "", "", false, false, keyboardLaunch);
            }
            catch { }
        }

        //Run a selected store application
        async Task RunStoreApplication()
        {
            try
            {
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.Title = "Windows Store Applications";
                vFilePickerSettings.Description = "Please select a Windows store application to run:";
                await Popup_Show_FilePicker("UWP", 0, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Check keyboard controller launch
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == vFilePickerResult.PathFile.ToLower());
                bool keyboardLaunch = keyboardProcess && vControllerAnyConnected();

                //Launch the UWP or Win32Store application
                await PrepareProcessLauncherUwpAndWin32StoreAsync(vFilePickerResult.Name, vFilePickerResult.PathFile, string.Empty, false, keyboardLaunch);
            }
            catch { }
        }

        //Close other running launchers
        async Task CloseLaunchers(bool SilentClose)
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Close launchers";
                Answers.Add(Answer1);

                DataBindString messageResult = null;
                if (!SilentClose)
                {
                    messageResult = await Popup_Show_MessageBox("Do you want to close other running launchers?", "You can edit the launchers that are closing in the profile manager.", "This includes launchers like Steam, EA Desktop, Ubisoft, GoG, Battle.net and Epic.", Answers);
                }

                if (SilentClose || (messageResult != null && messageResult == Answer1))
                {
                    await Notification_Send_Status("AppClose", "Closing other launchers");

                    //Close all known other launchers
                    foreach (ProfileShared closeLauncher in vCtrlCloseLaunchers)
                    {
                        try
                        {
                            AVProcessTool.Close_ProcessName(closeLauncher.String1);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        //Close the remote streamers
        async Task CloseStreamers()
        {
            try
            {
                //Ask if the user really wants to disconnect remote streams
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Stream.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Disconnect streams";
                Answers.Add(Answer1);

                DataBindString messageResult = await Popup_Show_MessageBox("Do you want to disconnect remote streams?", "", "This includes streams from GeForce Experience, Parsec and Steam In-Home Streaming.", Answers);
                if (messageResult != null && messageResult == Answer1)
                {
                    await Notification_Send_Status("Stream", "Disconnecting remote streams");

                    //Disconnect Steam Streaming
                    AVProcessTool.Close_ProcessName("steam.exe");

                    //Disconnect GeForce Experience
                    AVProcessTool.Close_ProcessName("nvstreamer.exe");

                    //Disconnect Parsec Streaming
                    KeyPressReleaseCombo(KeysVirtual.Control, KeysVirtual.F3);

                    //Disconnect Remote Desktop
                    //LaunchProcess(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\tsdiscon.exe", "", "", "");
                }
            }
            catch { }
        }

        //Launch DirectXInput application
        async Task LaunchDirectXInput(bool silentLaunch)
        {
            try
            {
                if (!CheckRunningProcessByNameOrTitle("DirectXInput", false, true))
                {
                    Debug.WriteLine("Launching DirectXInput");
                    if (!silentLaunch)
                    {
                        await Notification_Send_Status("DirectXInput", "Launching DirectXInput");
                    }

                    AVProcessTool.Launch_Exe("DirectXInput-Launcher.exe", "", "", false, true, false);
                }
                else
                {
                    if (!silentLaunch)
                    {
                        await Notification_Send_Status("DirectXInput", "DirectXInput is already running");
                    }
                }
            }
            catch { }
        }

        //Launch or close the Fps Overlayer
        async Task LaunchCloseFpsOverlayer()
        {
            try
            {
                if (CheckRunningProcessByNameOrTitle("FpsOverlayer", false, true))
                {
                    //Close the Fps Overlayer
                    await CloseFpsOverlayer();
                }
                else
                {
                    //Launch the Fps Overlayer
                    await LaunchFpsOverlayer(true);
                }
            }
            catch { }
        }

        //Close the Fps Overlayer
        async Task CloseFpsOverlayer()
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Hiding Fps Overlayer");

                //Show notification
                await Notification_Send_Status("Fps", "Hiding Fps Overlayer");

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "ApplicationExit";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), vArnoldVinkSockets.vSocketServerPort + 2);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }

        //Launch the Fps Overlayer
        async Task LaunchFpsOverlayer(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !CheckRunningProcessByNameOrTitle("FpsOverlayer", false, true))
                {
                    Debug.WriteLine("Showing Fps Overlayer");

                    //Show notification
                    await Notification_Send_Status("Fps", "Showing Fps Overlayer");

                    //Launch Fps Overlayer
                    AVProcessTool.Launch_Exe("FpsOverlayer-Launcher.exe", "", "", false, true, false);
                }
            }
            catch { }
        }

        //Hide or show the keyboard controller
        async Task KeyboardControllerHideShow(bool forceShow)
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