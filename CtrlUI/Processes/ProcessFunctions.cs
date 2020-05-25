using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Focus on a process window
        async Task PrepareFocusProcessWindow(string processName, int processIdTarget, IntPtr windowHandleTarget, int windowStateCommand, bool setWindowState, bool setTempTopMost, bool silentFocus, bool launchKeyboard)
        {
            try
            {
                if (!vChangingWindow)
                {
                    vChangingWindow = true;

                    //Check if process is available
                    if (windowHandleTarget == null)
                    {
                        if (!silentFocus) { await Notification_Send_Status("Close", "App no longer running"); }
                        Debug.WriteLine("Show application no longer seems to be running.");
                        vChangingWindow = false;
                        return;
                    }

                    //Check if process is available
                    if (windowHandleTarget == IntPtr.Zero)
                    {
                        if (!silentFocus) { await Notification_Send_Status("Close", "App can't be shown"); }
                        Debug.WriteLine("Application can't be shown, window handle is empty.");
                        vChangingWindow = false;
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
                    Debug.WriteLine("Showing application window: " + processName);

                    //Focus on application window handle
                    bool windowFocused = await FocusProcessWindow(processName, processIdTarget, windowHandleTarget, windowStateCommand, setWindowState, setTempTopMost);
                    if (!windowFocused)
                    {
                        await Notification_Send_Status("Close", "Failed showing the app");
                        Debug.WriteLine("Failed showing the application, no longer running?");
                        vChangingWindow = false;
                        return;
                    }

                    //Launch the keyboard controller
                    if (launchKeyboard)
                    {
                        await KeyboardControllerHideShow(true);
                    }

                    vChangingWindow = false;
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Close", "Failed showing the app");
                Debug.WriteLine("Failed showing the application, no longer running? " + ex.Message);
                vChangingWindow = false;
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
                    if (dataBindApp.StatusLauncher == Visibility.Visible)
                    {
                        await LaunchProcessUrlProtocol(dataBindApp);
                        return;
                    }

                    //Refresh the processes list
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

        async Task<bool> LaunchProcessUrlProtocol(DataBindApp dataBindApp)
        {
            try
            {
                await Notification_Send_Status("AppLaunch", "Launching " + dataBindApp.Name);
                Debug.WriteLine("Launching url protocol: " + dataBindApp.PathExe + " / " + dataBindApp.PathLaunch);

                Process LaunchProcess = new Process();
                LaunchProcess.StartInfo.FileName = dataBindApp.PathExe;
                LaunchProcess.StartInfo.WorkingDirectory = dataBindApp.PathLaunch;
                LaunchProcess.Start();
                return true;
            }
            catch { }
            return false;
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
                    //Minimize the CtrlUI window
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["MinimizeAppOnShow"]))
                    {
                        await AppMinimize(true);
                    }

                    //Check keyboard controller launch
                    string fileNameNoExtension = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                    bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower() || x.String1.ToLower() == dataBindApp.PathExe.ToLower());
                    bool keyboardLaunch = (keyboardProcess || dataBindApp.LaunchKeyboard) && vControllerAnyConnected();

                    //Force focus on the app
                    await PrepareFocusProcessWindow(dataBindApp.Name, processMulti.Identifier, processWindowHandle, 0, false, false, false, keyboardLaunch);
                }
                else
                {
                    Debug.WriteLine("Show application has no window.");

                    //Focus or Close when process is already running
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString AnswerClose = new DataBindString();
                    AnswerClose.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/AppClose.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerClose.Name = "Close application";
                    Answers.Add(AnswerClose);

                    DataBindString AnswerLaunch = new DataBindString();
                    AnswerLaunch.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/AppLaunch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerLaunch.Name = "Launch new instance";
                    Answers.Add(AnswerLaunch);

                    DataBindString AnswerRestartCurrent = new DataBindString();
                    if (!string.IsNullOrWhiteSpace(processMulti.Argument))
                    {
                        AnswerRestartCurrent.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/AppRestart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        AnswerRestartCurrent.Name = "Restart application";
                        AnswerRestartCurrent.NameSub = "(Current argument)";
                        Answers.Add(AnswerRestartCurrent);
                    }

                    DataBindString AnswerRestartWithout = new DataBindString();
                    AnswerRestartWithout.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/AppRestart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
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
            catch { }
        }

        //Show failed launch messagebox
        async Task LaunchProcessFailed()
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Check.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Alright";
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
                vFilePickerFilterIn = new List<string> { "exe", "bat" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Launch Executable";
                vFilePickerDescription = "Please select an executable file to run:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Check keyboard controller launch
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(vFilePickerResult.PathFile);
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower());
                bool keyboardLaunch = keyboardProcess && vControllerAnyConnected();

                //Launch the Win32 application
                await PrepareProcessLauncherWin32Async(fileNameNoExtension, vFilePickerResult.PathFile, "", "", false, true, false, false, keyboardLaunch, false);
            }
            catch { }
        }

        //Run a selected store application
        async Task RunStoreApplication()
        {
            try
            {
                vFilePickerFilterIn = new List<string>();
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Windows Store Applications";
                vFilePickerDescription = "Please select a Windows store application to run:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = false;
                vFilePickerShowDirectories = false;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("UWP", 0, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Check keyboard controller launch
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == vFilePickerResult.PathFile.ToLower());
                bool keyboardLaunch = keyboardProcess && vControllerAnyConnected();

                //Launch the UWP or Win32Store application
                await PrepareProcessLauncherUwpAndWin32StoreAsync(vFilePickerResult.Name, vFilePickerResult.PathFile, string.Empty, false, true, keyboardLaunch);
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
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/AppClose.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Close launchers";
                Answers.Add(Answer1);

                DataBindString messageResult = null;
                if (!SilentClose)
                {
                    messageResult = await Popup_Show_MessageBox("Do you want to close other running launchers?", "You can edit the launchers that are closing in the profile manager.", "This includes launchers like Steam, Origin, Uplay, GoG, Battle.net, Bethesda and Epic Games.", Answers);
                }

                if (SilentClose || (messageResult != null && messageResult == Answer1))
                {
                    await Notification_Send_Status("AppClose", "Closing other launchers");

                    //Close all known other launchers
                    foreach (ProfileShared closeLauncher in vCtrlCloseLaunchers)
                    {
                        try
                        {
                            CloseProcessesByNameOrTitle(closeLauncher.String1, false);
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
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Stream.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Disconnect streams";
                Answers.Add(Answer1);

                DataBindString messageResult = await Popup_Show_MessageBox("Do you want to disconnect remote streams?", "", "This includes streams from GeForce Experience, Parsec and Steam In-Home Streaming.", Answers);
                if (messageResult != null && messageResult == Answer1)
                {
                    await Notification_Send_Status("Stream", "Disconnecting remote streams");

                    //Disconnect Steam Streaming
                    CloseProcessesByNameOrTitle("steam", false);

                    //Disconnect GeForce Experience
                    CloseProcessesByNameOrTitle("nvstreamer", false);

                    //Disconnect Parsec Streaming
                    await KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.F3, false);

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
                if (!CheckRunningProcessByNameOrTitle("DirectXInput", false))
                {
                    Debug.WriteLine("Launching DirectXInput");
                    if (!silentLaunch)
                    {
                        await Notification_Send_Status("DirectXInput", "Launching DirectXInput");
                    }

                    await ProcessLauncherWin32Async("DirectXInput-Admin.exe", "", "", true, false);
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
                if (CheckRunningProcessByNameOrTitle("FpsOverlayer", false))
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
                await Notification_Send_Status("Fps", "Closing Fps Overlayer");
                Debug.WriteLine("Closing Fps Overlayer");

                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vTcpListenerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vTcpListenerPort;
                socketSend.Object = "ApplicationExit";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vTcpListenerIp, vArnoldVinkSockets.vTcpListenerPort + 3, vArnoldVinkSockets.vTcpClientTimeout);
                await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vTcpClientTimeout, false);
            }
            catch { }
        }

        //Launch the Fps Overlayer
        async Task LaunchFpsOverlayer(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !CheckRunningProcessByNameOrTitle("FpsOverlayer", false))
                {
                    await Notification_Send_Status("Fps", "Showing Fps Overlayer");
                    Debug.WriteLine("Showing Fps Overlayer");
                    await ProcessLauncherWin32Async("FpsOverlayer-Admin.exe", "", "", true, false);
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
                socketSend.SourceIp = vArnoldVinkSockets.vTcpListenerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vTcpListenerPort;
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
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vTcpListenerIp, vArnoldVinkSockets.vTcpListenerPort + 1, vArnoldVinkSockets.vTcpClientTimeout);
                await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vTcpClientTimeout, false);
            }
            catch { }
        }
    }
}