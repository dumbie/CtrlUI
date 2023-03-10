using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
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
                        if (processMultiFor.WindowHandleMain != IntPtr.Zero)
                        {
                            processMulti = processMultiFor;
                        }
                    }

                    //Show the process window
                    await ShowProcessWindowAuto(dataBindApp, processMulti);
                }
                else if (Check_PathUrlProtocol(dataBindApp.PathExe))
                {
                    //Run process url protocol
                    await PrepareProcessLauncherUrlProtocolAsync(dataBindApp, false, false, false);
                    return;
                }
                else
                {
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
                        await CloseAllProcessesAuto(dataBindApp, true, false);
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

        //Launch databind process
        async Task LaunchProcessDatabindAuto(DataBindApp dataBindApp)
        {
            try
            {
                //Check keyboard controller launch
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower() || x.String1.ToLower() == dataBindApp.PathExe.ToLower() || x.String1.ToLower() == dataBindApp.AppUserModelId.ToLower());
                bool keyboardLaunch = (keyboardProcess || dataBindApp.LaunchKeyboard) && vControllerAnyConnected();

                //Check if databind paths are available
                if (!await CheckDatabindPathAuto(dataBindApp))
                {
                    return;
                }

                //Launch the databind process
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    await EnableHDRDatabindAuto(dataBindApp);
                    await PrepareProcessLauncherUwpAndWin32StoreAsync(dataBindApp, string.Empty, false, keyboardLaunch);
                }
                else if (dataBindApp.LaunchFilePicker)
                {
                    string launchArgument = await GetLaunchArgumentFilePicker(dataBindApp);
                    if (launchArgument == "Cancel") { return; }
                    await EnableHDRDatabindAuto(dataBindApp);
                    await PrepareProcessLauncherWin32Async(dataBindApp, launchArgument, false, false, keyboardLaunch);
                }
                else if (dataBindApp.Category == AppCategory.Emulator && !dataBindApp.LaunchSkipRom)
                {
                    string launchArgument = await GetLaunchArgumentEmulator(dataBindApp);
                    if (launchArgument == "Cancel") { return; }
                    await EnableHDRDatabindAuto(dataBindApp);
                    await PrepareProcessLauncherWin32Async(dataBindApp, launchArgument, false, false, keyboardLaunch);
                }
                else
                {
                    await EnableHDRDatabindAuto(dataBindApp);
                    await PrepareProcessLauncherWin32Async(dataBindApp, string.Empty, false, false, keyboardLaunch);
                }
            }
            catch { }
        }

        //Launch selected executable file
        async Task LaunchExecutableFile()
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

        //Launch selected store application
        async Task LaunchStoreApplication()
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

        //Show process launch failed message
        async Task ShowProcessLaunchFailedMessage()
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Ok";
                Answers.Add(Answer1);

                await Popup_Show_MessageBox("Failed to launch application", "", "Perhaps the application has moved to a new location or has closed.", Answers);
            }
            catch { }
        }

        //Launch DirectXInput application
        async Task LaunchDirectXInput(bool silentLaunch)
        {
            try
            {
                if (!Check_RunningProcessByName("DirectXInput", true))
                {
                    Debug.WriteLine("Launching DirectXInput");
                    if (!silentLaunch)
                    {
                        await Notification_Send_Status("DirectXInput", "Launching DirectXInput");
                    }

                    AVProcess.Launch_ExecuteInherit("DirectXInput-Launcher.exe", "", "", true);
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

        //Launch Fps Overlayer application
        async Task LaunchFpsOverlayer(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !Check_RunningProcessByName("FpsOverlayer", true))
                {
                    Debug.WriteLine("Showing Fps Overlayer");

                    //Show notification
                    await Notification_Send_Status("Fps", "Showing Fps Overlayer");

                    //Launch Fps Overlayer
                    AVProcess.Launch_ExecuteInherit("FpsOverlayer-Launcher.exe", "", "", true);
                }
            }
            catch { }
        }

        //Launch or close Fps Overlayer application
        async Task LaunchCloseFpsOverlayer()
        {
            try
            {
                if (Check_RunningProcessByName("FpsOverlayer", true))
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
    }
}