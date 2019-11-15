using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        //Focus on an application window handle
        void FocusWindowHandlePrepare(string TitleTarget, IntPtr TargetWindowHandle, int ShowCommand, bool SetWindowState, bool SwitchWindow, bool ToTopWindow, bool SetForeground, bool TempTopMost, bool Silent)
        {
            try
            {
                if (!vChangingWindow)
                {
                    vChangingWindow = true;

                    //Check if process is available
                    if (TargetWindowHandle == null)
                    {
                        if (!Silent) { Popup_Show_Status("Close", "App no longer running"); }
                        Debug.WriteLine("Show application no longer seems to be running.");
                        vChangingWindow = false;
                        return;
                    }

                    //Check if process is available
                    if (TargetWindowHandle == IntPtr.Zero)
                    {
                        if (!Silent) { Popup_Show_Status("Close", "App can't be shown"); }
                        Debug.WriteLine("Application can't be shown, window handle is empty.");
                        vChangingWindow = false;
                        return;
                    }

                    //Update the interface status
                    if (!Silent) { Popup_Show_Status("MiniMaxi", "Showing " + TitleTarget); }
                    Debug.WriteLine("Showing application window: " + TitleTarget);

                    //Focus on an application window handle
                    async void TaskAction()
                    {
                        try
                        {
                            await FocusWindowHandle(TitleTarget, TargetWindowHandle, ShowCommand, SetWindowState, SwitchWindow, ToTopWindow, SetForeground, TempTopMost);
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

        //Check which launch method needs to be used
        async Task LaunchProcessSelector(DataBindApp LaunchApp)
        {
            try
            {
                if (LaunchApp.Category == "Process")
                {
                    //Check if process has multiple windows
                    IntPtr ProcessWindowHandle = IntPtr.Zero;
                    if (LaunchApp.Type == "UWP")
                    {
                        ProcessWindowHandle = LaunchApp.WindowHandle;
                    }
                    else
                    {
                        Process ShowProcess = GetProcessById(LaunchApp.ProcessId);
                        ProcessWindowHandle = await CheckMultiWindowWin32(LaunchApp.Name, ShowProcess);
                    }

                    if (ProcessWindowHandle != IntPtr.Zero)
                    {
                        //Minimize the CtrlUI window
                        if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True") { await AppMinimize(true); }

                        //Force focus on the app
                        FocusWindowHandlePrepare(LaunchApp.Name, ProcessWindowHandle, 0, false, true, true, true, false, false);
                    }
                    else
                    {
                        Popup_Show_Status("Close", "Application has no window");
                        Debug.WriteLine("Show application has no window.");
                    }
                }
                else if (LaunchApp.StatusLauncher == Visibility.Visible)
                {
                    Popup_Show_Status("App", "Launching " + LaunchApp.Name);
                    Debug.WriteLine("Launching url protocol: " + LaunchApp.PathExe);
                    Process.Start(LaunchApp.PathExe);
                }
                else if (LaunchApp.Category == "Emulator")
                {
                    await LaunchProcessDatabindWin32Emulator(LaunchApp);
                }
                else if (LaunchApp.FilePickerLaunch)
                {
                    await LaunchProcessDatabindWin32FilePicker(LaunchApp);
                }
                else
                {
                    if (LaunchApp.Type == "UWP")
                    {
                        await LaunchProcessDatabindUwp(LaunchApp);
                    }
                    else
                    {
                        await LaunchProcessDatabindWin32(LaunchApp);
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

        //Check if process is valid by name
        bool ValidateProcessByName(string ProcessName, bool CheckEmpty, bool CheckBlacklist)
        {
            try
            {
                if (CheckEmpty && string.IsNullOrWhiteSpace(ProcessName)) { return false; }
                if (CheckBlacklist && vAppsBlacklistProcess.Any(x => x.ToLower() == ProcessName.ToLower())) { return false; }
            }
            catch { }
            return true;
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
                            CloseProcessesByName(CloseLauncher, false);
                        }
                        catch { }
                    }

                    //Refresh the application lists
                    await RefreshApplicationLists(false, false, false, false);
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
                    CloseProcessesByName("steam", false);

                    //Disconnect GeForce Experience
                    CloseProcessesByName("nvstreamer", false);

                    //Disconnect Parsec Streaming
                    KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.F3, false);

                    //Disconnect Remote Desktop
                    //LaunchProcess(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\tsdiscon.exe", "", "", "");
                }
            }
            catch { }
        }

        //Close or show the Fps Overlayer
        async Task CloseShowFpsOverlayer()
        {
            try
            {
                //Close the fps overlayer
                if (CheckRunningProcessByName("FpsOverlayer", false))
                {
                    Popup_Show_Status("Close", "Closing Fps Overlayer");
                    Debug.WriteLine("Closing Fps Overlayer");
                    CloseProcessesByName("FpsOverlayer", false);
                }
                else
                {
                    //Launch the fps overlayer
                    Popup_Show_Status("Fps", "Showing Fps Overlayer");
                    Debug.WriteLine("Showing Fps Overlayer");
                    ProcessLauncherWin32("FpsOverlayer-Admin.exe", "", "", true, false);
                    await Task.Delay(1000);

                    //Force focus on CtrlUI
                    FocusWindowHandlePrepare("CtrlUI", vProcessCurrent.MainWindowHandle, 0, false, true, true, true, true, true);
                }
            }
            catch { }
        }

        //Close or show the keyboard controller
        async Task CloseShowKeyboardController()
        {
            try
            {
                //Close the keyboard controller
                if (CheckRunningProcessByName("KeyboardController", false))
                {
                    Popup_Show_Status("Close", "Closing on screen keyboard");
                    Debug.WriteLine("Closing on screen keyboard");
                    CloseProcessesByName("KeyboardController", false);
                }
                else
                {
                    //Launch the keyboard controller
                    Popup_Show_Status("Keyboard", "Showing on screen keyboard");
                    Debug.WriteLine("Showing on screen keyboard");
                    ProcessLauncherWin32("KeyboardController-Admin.exe", "", "", true, false);
                    await Task.Delay(1000);

                    //Force focus on CtrlUI
                    FocusWindowHandlePrepare("CtrlUI", vProcessCurrent.MainWindowHandle, 0, false, true, true, true, true, true);
                }
            }
            catch { }
        }
    }
}