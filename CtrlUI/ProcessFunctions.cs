using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;
using static LibraryShared.OutputKeyboard;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Focus on a process window
        void FocusProcessWindowPrepare(string titleTarget, int processIdTarget, IntPtr windowHandleTarget, int windowStateCommand, bool setWindowState, bool setTempTopMost, bool Silent)
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
                    if (!Silent) { Popup_Show_Status("MiniMaxi", "Showing " + titleTarget); }
                    Debug.WriteLine("Showing application window: " + titleTarget);

                    //Focus on an application window handle
                    async void TaskAction()
                    {
                        try
                        {
                            await FocusProcessWindow(titleTarget, processIdTarget, windowHandleTarget, windowStateCommand, setWindowState, setTempTopMost);
                        }
                        catch { }
                    }
                    AVActions.TaskStart(TaskAction, null);

                    vChangingWindow = false;
                }
            }
            catch
            {
                Popup_Show_Status("Close", "Failed showing the app");
                Debug.WriteLine("Failed showing the application, perhaps it is no longer running?");
                vChangingWindow = false;
            }
        }

        //Convert Process to a ProcessMulti
        ProcessMulti ConvertProcessToProcessMulti(Process convertProcess)
        {
            ProcessMulti convertedProcess = new ProcessMulti();
            try
            {
                convertedProcess.ProcessId = convertProcess.Id;
                convertedProcess.ProcessThreads = convertProcess.Threads;
                convertedProcess.WindowHandle = convertProcess.MainWindowHandle;
            }
            catch { }
            return convertedProcess;
        }

        //Check which launch method needs to be used
        async Task LaunchProcessSelector(DataBindApp LaunchApp)
        {
            try
            {
                if (LaunchApp.Category == AppCategory.Process)
                {
                    //Check if process has multiple windows
                    IntPtr ProcessWindowHandle = IntPtr.Zero;
                    if (LaunchApp.Type == ProcessType.UWP)
                    {
                        ProcessWindowHandle = LaunchApp.WindowHandle;
                    }
                    else
                    {
                        ProcessMulti multiProcess = ConvertProcessToProcessMulti(GetProcessById(LaunchApp.ProcessId));
                        ProcessWindowHandle = await CheckProcessMultiWindowWin32(LaunchApp.Name, multiProcess);
                    }

                    if (ProcessWindowHandle != IntPtr.Zero)
                    {
                        //Minimize the CtrlUI window
                        if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True") { await AppMinimize(true); }

                        //Force focus on the app
                        FocusProcessWindowPrepare(LaunchApp.Name, LaunchApp.ProcessId, ProcessWindowHandle, 0, false, false, false);
                    }
                    else
                    {
                        Popup_Show_Status("Close", "Application has no window");
                        Debug.WriteLine("Show application has no window.");
                    }
                }
                else
                {
                    bool appLaunched = false;
                    if (LaunchApp.StatusLauncher == Visibility.Visible)
                    {
                        Popup_Show_Status("App", "Launching " + LaunchApp.Name);
                        Debug.WriteLine("Launching url protocol: " + LaunchApp.PathExe);
                        Process.Start(LaunchApp.PathExe);
                        appLaunched = true;
                    }
                    else if (LaunchApp.Category == AppCategory.Emulator)
                    {
                        appLaunched = await LaunchProcessDatabindWin32Emulator(LaunchApp);
                    }
                    else if (LaunchApp.LaunchFilePicker)
                    {
                        appLaunched = await LaunchProcessDatabindWin32FilePicker(LaunchApp);
                    }
                    else
                    {
                        if (LaunchApp.Type == ProcessType.UWP || LaunchApp.Type == ProcessType.Win32Store)
                        {
                            appLaunched = await LaunchProcessDatabindUwp(LaunchApp);
                        }
                        else
                        {
                            appLaunched = await LaunchProcessDatabindWin32(LaunchApp);
                        }
                    }

                    //Launch the keyboard controller
                    if (appLaunched && LaunchApp.LaunchKeyboard)
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
                await ProcessLauncherWin32Prepare(vFilePickerResult.PathFile, "", "", false, false, true, false, false);
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
                    foreach (string CloseLauncher in vAppsOtherLaunchers)
                    {
                        try
                        {
                            CloseProcessesByNameOrTitle(CloseLauncher, false);
                        }
                        catch { }
                    }

                    //Refresh the application lists
                    await RefreshApplicationLists(false, false, false, false, false);
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