using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Focus on a process window
        void FocusProcessWindowPrepare(string processName, int processIdTarget, IntPtr windowHandleTarget, int windowStateCommand, bool setWindowState, bool setTempTopMost, bool Silent)
        {
            try
            {
                if (!vChangingWindow)
                {
                    vChangingWindow = true;

                    //Check if process is available
                    if (windowHandleTarget == null)
                    {
                        if (!Silent) { Popup_Show_Status("Close", "App no longer running"); }
                        Debug.WriteLine("Show application no longer seems to be running.");
                        vChangingWindow = false;
                        return;
                    }

                    //Check if process is available
                    if (windowHandleTarget == IntPtr.Zero)
                    {
                        if (!Silent) { Popup_Show_Status("Close", "App can't be shown"); }
                        Debug.WriteLine("Application can't be shown, window handle is empty.");
                        vChangingWindow = false;
                        return;
                    }

                    //Update the interface status
                    if (!Silent) { Popup_Show_Status("MiniMaxi", "Showing " + processName); }
                    Debug.WriteLine("Showing application window: " + processName);

                    //Focus on an application window handle
                    async void TaskAction()
                    {
                        try
                        {
                            bool windowFocused = await FocusProcessWindow(processName, processIdTarget, windowHandleTarget, windowStateCommand, setWindowState, setTempTopMost);
                            if (!windowFocused)
                            {
                                Popup_Show_Status("Close", "Failed showing the app");
                            }
                        }
                        catch { }
                    }
                    AVActions.TaskStart(TaskAction, null);

                    vChangingWindow = false;
                }
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Close", "Failed showing the app");
                Debug.WriteLine("Failed showing the application, no longer running? " + ex.Message);
                vChangingWindow = false;
            }
        }

        //Check which launch method needs to be used
        async Task LaunchProcessSelector(DataBindApp dataBindApp)
        {
            try
            {
                if (dataBindApp.Category == AppCategory.Process)
                {
                    //Get the process multi
                    ProcessMulti processMulti = dataBindApp.ProcessMulti.FirstOrDefault();

                    //Show the process window
                    await ShowProcessWindow(dataBindApp, processMulti);
                }
                else
                {
                    Popup_Show_Status("App", "Preparing launch");
                    Debug.WriteLine("Preparing application launch: " + dataBindApp.PathExe);

                    //Run process url protocol
                    if (dataBindApp.StatusLauncher == Visibility.Visible)
                    {
                        LaunchProcessUrlProtocol(dataBindApp);
                        return;
                    }

                    //Status to see if app launched
                    bool appLaunched = false;

                    //Refresh the application lists
                    await RefreshApplicationLists(true, false, false, false, true, false, false);

                    //Check if process is running
                    ProcessMulti processMulti = await SelectProcessMulti(dataBindApp, true);
                    if (processMulti == null)
                    {
                        Debug.WriteLine("Process is not running, launching the application.");
                        if (dataBindApp.Category == AppCategory.Emulator)
                        {
                            appLaunched = await LaunchProcessDatabindWin32Emulator(dataBindApp);
                        }
                        else if (dataBindApp.LaunchFilePicker)
                        {
                            appLaunched = await LaunchProcessDatabindWin32FilePicker(dataBindApp);
                        }
                        else if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                        {
                            appLaunched = await LaunchProcessDatabindUwpAndWin32Store(dataBindApp);
                        }
                        else
                        {
                            appLaunched = await LaunchProcessDatabindWin32(dataBindApp);
                        }
                    }
                    else if (processMulti.Action == "Cancel")
                    {
                        Debug.WriteLine("Process is already running, skipping the launch.");
                        return;
                    }
                    else if (processMulti.Action == "CloseAll")
                    {
                        Debug.WriteLine("Closing all processes, skipping the launch.");
                        if (processMulti.Type == ProcessType.UWP)
                        {
                            await CloseAllProcessesUwp(dataBindApp, true, false);
                        }
                        else
                        {
                            await CloseAllProcessesWin32AndWin32Store(dataBindApp, true, false);
                        }
                        return;
                    }
                    else
                    {
                        Debug.WriteLine("Process is already running, checking the app.");
                        appLaunched = await CheckLaunchProcessStatus(dataBindApp, processMulti);
                    }

                    //Launch the keyboard controller
                    if (appLaunched && dataBindApp.LaunchKeyboard)
                    {
                        LaunchKeyboardController(true);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Failed launch process selector.");
            }
        }

        bool LaunchProcessUrlProtocol(DataBindApp dataBindApp)
        {
            try
            {
                Popup_Show_Status("App", "Launching " + dataBindApp.Name);
                Debug.WriteLine("Launching url protocol: " + dataBindApp.PathExe);

                Process.Start(dataBindApp.PathExe);
                return true;
            }
            catch { }
            return false;
        }

        async Task ShowProcessWindow(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                //Check if application has multiple windows
                IntPtr processWindowHandle = IntPtr.Zero;
                if (processMulti.Type == ProcessType.UWP)
                {
                    processWindowHandle = processMulti.WindowHandle;
                }
                else if (processMulti.Type == ProcessType.Win32 || processMulti.Type == ProcessType.Win32Store)
                {
                    processWindowHandle = await CheckProcessWindowsWin32AndWin32Store(dataBindApp, processMulti);
                }

                //Check if application window has been found
                if (processWindowHandle == new IntPtr(-100))
                {
                    Debug.WriteLine("Closing all " + processMulti.Type + " app windows, by closing the process.");
                    if (processMulti.Type == ProcessType.UWP)
                    {
                        await CloseSingleProcessUwp(dataBindApp, processMulti, true, false);
                    }
                    else if (processMulti.Type == ProcessType.Win32 || processMulti.Type == ProcessType.Win32Store)
                    {
                        await CloseSingleProcessWin32AndWin32Store(dataBindApp, processMulti, true, false);
                    }
                }
                else if (processWindowHandle == new IntPtr(-200))
                {
                    Debug.WriteLine("Cancelled window selection.");
                }
                else if (processWindowHandle != IntPtr.Zero)
                {
                    //Minimize the CtrlUI window
                    if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True")
                    {
                        await AppMinimize(true);
                    }

                    //Force focus on the app
                    FocusProcessWindowPrepare(dataBindApp.Name, processMulti.Identifier, processWindowHandle, 0, false, false, false);

                    ////Launch the keyboard controller
                    //if (dataBindApp.LaunchKeyboard)
                    //{
                    //    LaunchKeyboardController(true);
                    //}
                }
                else
                {
                    Popup_Show_Status("Close", "Application has no window");
                    Debug.WriteLine("Show application has no window.");
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
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
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
                vFilePickerFilterIn = new string[] { "exe", "bat" };
                vFilePickerFilterOut = new string[] { };
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

                //Launch the Win32 application
                await LaunchProcessManuallyWin32(vFilePickerResult.PathFile, "", "", false, true, false, false);
            }
            catch { }
        }

        //Run an selected uwp application
        async Task RunUwpApplication()
        {
            try
            {
                vFilePickerFilterIn = new string[] { };
                vFilePickerFilterOut = new string[] { };
                vFilePickerTitle = "Windows Store Applications";
                vFilePickerDescription = "Please select a Windows store application to run:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = false;
                vFilePickerShowDirectories = false;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("UWP", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Launch the selected uwp app
                await LaunchProcessManuallyUwpAndWin32Store(vFilePickerResult.Name, vFilePickerResult.PathFile, string.Empty, false, true);
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
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                Answer1.Name = "Close launchers";
                Answers.Add(Answer1);

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

                DataBindString Result = null;

                if (!SilentClose)
                {
                    Result = await Popup_Show_MessageBox("Do you want to close other running launchers?", "", "This includes launchers like Steam, Origin, Uplay, GoG, Battle.net, Bethesda and Epic Games.", Answers);
                }

                if (SilentClose || (Result != null && Result == Answer1))
                {
                    Popup_Show_Status("Closing", "Closing other launchers");

                    //Close all known other launchers
                    foreach (string CloseLauncher in vAppsCloseLaunchers)
                    {
                        try
                        {
                            CloseProcessesByNameOrTitle(CloseLauncher, false);
                        }
                        catch { }
                    }

                    //Refresh the application lists
                    await RefreshApplicationLists(true, false, false, false, false, false, false);
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
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Stream.png" }, IntPtr.Zero, -1);
                Answer1.Name = "Disconnect streams";
                Answers.Add(Answer1);

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

                DataBindString Result = await Popup_Show_MessageBox("Do you want to disconnect remote streams?", "", "This includes streams from GeForce Experience, Parsec and Steam In-Home Streaming.", Answers);
                if (Result != null && Result == Answer1)
                {
                    Popup_Show_Status("Stream", "Disconnecting remote streams");

                    //Disconnect Steam Streaming
                    CloseProcessesByNameOrTitle("steam", false);

                    //Disconnect GeForce Experience
                    CloseProcessesByNameOrTitle("nvstreamer", false);

                    //Disconnect Parsec Streaming
                    KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.F3, false);

                    //Disconnect Remote Desktop
                    //LaunchProcess(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\tsdiscon.exe", "", "", "");
                }
            }
            catch { }
        }

        //Launch DirectXInput application
        void LaunchDirectXInput()
        {
            try
            {
                if (!CheckRunningProcessByNameOrTitle("DirectXInput", false))
                {
                    Popup_Show_Status("Controller", "Launching DirectXInput");
                    Debug.WriteLine("Launching DirectXInput");

                    ProcessLauncherWin32("DirectXInput-Admin.exe", "", "", true, false);
                }
            }
            catch { }
        }

        //Close or show the Fps Overlayer
        void CloseShowFpsOverlayer()
        {
            try
            {
                //Close the fps overlayer
                if (CheckRunningProcessByNameOrTitle("FpsOverlayer", false))
                {
                    Popup_Show_Status("Close", "Closing Fps Overlayer");
                    Debug.WriteLine("Closing Fps Overlayer");
                    CloseProcessesByNameOrTitle("FpsOverlayer", false);
                }
                else
                {
                    //Launch the fps overlayer
                    Popup_Show_Status("Fps", "Showing Fps Overlayer");
                    Debug.WriteLine("Showing Fps Overlayer");
                    ProcessLauncherWin32("FpsOverlayer-Admin.exe", "", "", true, false);
                }
            }
            catch { }
        }

        //Launch the Fps Overlayer
        void LaunchFpsOverlayer()
        {
            try
            {
                if (!CheckRunningProcessByNameOrTitle("FpsOverlayer", false))
                {
                    //Launch the fps overlayer
                    Popup_Show_Status("Fps", "Showing Fps Overlayer");
                    Debug.WriteLine("Showing Fps Overlayer");
                    ProcessLauncherWin32("FpsOverlayer-Admin.exe", "", "", true, false);
                }
            }
            catch { }
        }

        //Close or show the keyboard controller
        void CloseShowKeyboardController()
        {
            try
            {
                //Close the keyboard controller
                if (CheckRunningProcessByNameOrTitle("KeyboardController", false))
                {
                    Popup_Show_Status("Close", "Closing on screen keyboard");
                    Debug.WriteLine("Closing on screen keyboard");
                    CloseProcessesByNameOrTitle("KeyboardController", false);
                }
                else
                {
                    //Launch the keyboard controller
                    Popup_Show_Status("Keyboard", "Showing on screen keyboard");
                    Debug.WriteLine("Showing on screen keyboard");
                    ProcessLauncherWin32("KeyboardController-Admin.exe", "", "", true, false);
                }
            }
            catch { }
        }

        //Launch the keyboard controller
        void LaunchKeyboardController(bool silent)
        {
            try
            {
                if (!CheckRunningProcessByNameOrTitle("KeyboardController", false))
                {
                    //Launch the keyboard controller
                    if (!silent) { Popup_Show_Status("Keyboard", "Showing on screen keyboard"); }
                    Debug.WriteLine("Showing on screen keyboard");
                    ProcessLauncherWin32("KeyboardController-Admin.exe", "", "", true, false);
                }
            }
            catch { }
        }
    }
}