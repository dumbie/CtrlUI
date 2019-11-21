using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
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
        //Launch a win32 application manually
        async Task ProcessLauncherWin32Prepare(string PathExe, string PathLaunch, string Argument, bool Silent, bool IgnoreRunning, bool AllowMinimize, bool runAsAdmin, bool createNoWindow)
        {
            try
            {
                //Check if the application exists
                if (!File.Exists(PathExe))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("App exe not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    Debug.WriteLine("Launch executable not found");
                    return;
                }

                //Check if process is running
                if (!IgnoreRunning)
                {
                    if (CheckRunningProcessByNameOrTitle(Path.GetFileNameWithoutExtension(PathExe), false))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Run new instance";
                        Answers.Add(Answer1);

                        DataBindString cancelString = new DataBindString();
                        cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                        cancelString.Name = "Cancel";
                        Answers.Add(cancelString);

                        //Show the messagebox
                        DataBindString Result = await Popup_Show_MessageBox(Path.GetFileNameWithoutExtension(PathExe) + " is already running", "", "Would you like to run another instance?", Answers);
                        if (Result != null)
                        {
                            if (Result == cancelString)
                            {
                                Popup_Show_Status("App", Path.GetFileNameWithoutExtension(PathExe) + " is already running");
                                Debug.WriteLine(Path.GetFileNameWithoutExtension(PathExe) + " is already running");
                                return;
                            }
                        }
                        else
                        {
                            Popup_Show_Status("App", Path.GetFileNameWithoutExtension(PathExe) + " is already running");
                            Debug.WriteLine(Path.GetFileNameWithoutExtension(PathExe) + " is already running");
                            return;
                        }
                    }
                }

                //Show launching message
                if (!Silent)
                {
                    Popup_Show_Status("App", "Launching " + Path.GetFileNameWithoutExtension(PathExe));
                    Debug.WriteLine("Launching Win32: " + Path.GetFileNameWithoutExtension(PathExe));
                }

                //Launch the Win32 application
                ProcessLauncherWin32(PathExe, PathLaunch, Argument, runAsAdmin, createNoWindow);

                //Minimize the CtrlUI window
                if (AllowMinimize && ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True") { await AppMinimize(true); }

                //Force focus on the app
                //FocusWindowHandlePrepare(Path.GetFileNameWithoutExtension(PathExe), LaunchProcess.MainWindowHandle, 0, false, false, true, true, false, true);
            }
            catch
            {
                //Show failed launch messagebox
                await LaunchProcessFailed();
            }
        }

        //Check Win32 process has multiple processes
        async Task<ProcessMulti> CheckMultiProcessWin32(DataBindApp LaunchApp)
        {
            try
            {
                List<DataBindString> multiAnswers = new List<DataBindString>();
                Process[] multiVariables = GetProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(LaunchApp.PathExe), false, true);
                if (multiVariables.Any())
                {
                    if (multiVariables.Count() > 1)
                    {
                        foreach (Process multiProcess in multiVariables)
                        {
                            try
                            {
                                //Get the process title
                                string ProcessTitle = GetWindowTitleFromProcess(multiProcess);
                                if (ProcessTitle == "Unknown") { ProcessTitle += " (Hidden)"; }
                                if (multiAnswers.Where(x => x.Name.ToLower() == ProcessTitle.ToLower()).Any()) { ProcessTitle += " (" + multiAnswers.Count + ")"; }

                                DataBindString AnswerApp = new DataBindString();
                                AnswerApp.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                                AnswerApp.Name = ProcessTitle;
                                multiAnswers.Add(AnswerApp);
                            }
                            catch { }
                        }

                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Launch new instance";
                        multiAnswers.Add(Answer1);

                        DataBindString Answer2 = new DataBindString();
                        Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                        Answer2.Name = "Close all the instances";
                        multiAnswers.Add(Answer2);

                        DataBindString cancelString = new DataBindString();
                        cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                        cancelString.Name = "Cancel";
                        multiAnswers.Add(cancelString);

                        DataBindString Result = await Popup_Show_MessageBox(LaunchApp.Name + " has multiple running instances", "", "Please select the instance that you wish to interact with:", multiAnswers);
                        if (Result != null)
                        {
                            if (Result == Answer2)
                            {
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Status = "CloseAll";
                                return processMultiNew;
                            }
                            else if (Result == cancelString)
                            {
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Status = "Cancel";
                                return processMultiNew;
                            }
                            else
                            {
                                return ConvertProcessToProcessMulti(ProcessType.Win32, multiVariables[multiAnswers.IndexOf(Result)]);
                            }
                        }
                        else
                        {
                            ProcessMulti processMultiNew = new ProcessMulti();
                            processMultiNew.Status = "Cancel";
                            return processMultiNew;
                        }
                    }
                    else
                    {
                        return ConvertProcessToProcessMulti(ProcessType.Win32, multiVariables.FirstOrDefault());
                    }
                }
                else
                {
                    ProcessMulti processMultiNew = new ProcessMulti();
                    processMultiNew.Status = "NoProcess";
                    return processMultiNew;
                }
            }
            catch
            {
                ProcessMulti processMultiNew = new ProcessMulti();
                processMultiNew.Status = "NoProcess";
                return processMultiNew;
            }
        }

        //Check Win32 process has multiple windows
        async Task<IntPtr> CheckProcessMultiWindowWin32(string AppTitle, ProcessMulti ProcessTarget)
        {
            try
            {
                if (ProcessTarget.ProcessThreads.Count > 1)
                {
                    Debug.WriteLine("Found window threads: " + ProcessTarget.ProcessThreads.Count);

                    List<DataBindString> multiAnswers = new List<DataBindString>();
                    List<IntPtr> multiVariables = new List<IntPtr>();
                    foreach (ProcessThread ThreadProcess in ProcessTarget.ProcessThreads)
                    {
                        foreach (IntPtr ThreadWindowHandle in EnumThreadWindows(ThreadProcess.Id))
                        {
                            try
                            {
                                //Validate the window handle
                                if (ThreadWindowHandle == ProcessTarget.WindowHandle || ValidateWindowHandle(ThreadWindowHandle))
                                {
                                    //Get window title
                                    string ClassNameString = GetWindowTitleFromWindowHandle(ThreadWindowHandle);
                                    if (ThreadWindowHandle == ProcessTarget.WindowHandle) { ClassNameString += " (Main Window)"; }
                                    if (multiAnswers.Where(x => x.Name.ToLower() == ClassNameString.ToLower()).Any()) { ClassNameString += " (" + multiAnswers.Count + ")"; }

                                    DataBindString Answer1 = new DataBindString();
                                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                                    Answer1.Name = ClassNameString;

                                    //Add window to selection
                                    if (ThreadWindowHandle == ProcessTarget.WindowHandle)
                                    {
                                        multiAnswers.Insert(0, Answer1);
                                        multiVariables.Insert(0, ThreadWindowHandle);
                                    }
                                    else
                                    {
                                        multiAnswers.Add(Answer1);
                                        multiVariables.Add(ThreadWindowHandle);
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    //Check if there are multiple answers
                    if (multiVariables.Count == 1)
                    {
                        Debug.WriteLine("There is only one visible window, returning the default window.");
                        return multiVariables.FirstOrDefault();
                    }
                    else if (multiVariables.Count == 0)
                    {
                        return IntPtr.Zero;
                    }

                    DataBindString cancelString = new DataBindString();
                    cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                    cancelString.Name = "Cancel";
                    multiAnswers.Add(cancelString);

                    //Ask which window needs to be shown
                    DataBindString Result = await Popup_Show_MessageBox(AppTitle + " has multiple windows open", "", "Please select the window that you wish to be shown:", multiAnswers);
                    if (Result != null)
                    {
                        if (Result == cancelString)
                        {
                            return IntPtr.Zero;
                        }
                        else
                        {
                            return multiVariables[multiAnswers.IndexOf(Result)];
                        }
                    }
                    else
                    {
                        return IntPtr.Zero;
                    }
                }
                else
                {
                    Debug.WriteLine("Single thread process.");
                    return ProcessTarget.WindowHandle;
                }
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        //Check Win32 process status before launching (True = Continue)
        async Task<bool> LaunchProcessCheckWin32(DataBindApp LaunchApp)
        {
            try
            {
                Debug.WriteLine("Checking launch process Win32: " + LaunchApp.Name + " / " + LaunchApp.ProcessId + " / " + LaunchApp.WindowHandle);

                //Check Win32 process has multiple processes
                ProcessMulti processMultipleCheck = await CheckMultiProcessWin32(LaunchApp);
                if (processMultipleCheck.Status == "NoProcess") { return true; }
                if (processMultipleCheck.Status == "Cancel") { return false; }
                if (processMultipleCheck.Status == "CloseAll")
                {
                    Popup_Show_Status("Closing", "Closing " + LaunchApp.Name);
                    Debug.WriteLine("Closing processes: " + LaunchApp.Name + " / " + LaunchApp.ProcessId + " / " + LaunchApp.WindowHandle);

                    //Close the process
                    bool ClosedProcess = CloseProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(LaunchApp.PathExe), false);
                    if (ClosedProcess)
                    {
                        //Updating running status
                        LaunchApp.StatusRunning = Visibility.Collapsed;
                        LaunchApp.RunningTimeLastUpdate = 0;

                        //Update process count
                        LaunchApp.ProcessRunningCount = string.Empty;
                    }
                    else
                    {
                        Popup_Show_Status("Closing", "Failed to close the app");
                        Debug.WriteLine("Failed to close the application.");
                    }

                    return false;
                }

                //Focus or Close when process is already running
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MiniMaxi.png" }, IntPtr.Zero, -1);
                Answer1.Name = "Show application";
                Answers.Add(Answer1);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                Answer2.Name = "Close application";
                Answers.Add(Answer2);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Switch.png" }, IntPtr.Zero, -1);
                Answer3.Name = "Restart application";
                Answers.Add(Answer3);

                DataBindString Answer4 = new DataBindString();
                Answer4.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                Answer4.Name = "Launch new instance";
                Answers.Add(Answer4);

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

                //Check application category
                string ApplicationRuntime = string.Empty;
                if (LaunchApp.Category == AppCategory.Shortcut)
                {
                    ApplicationRuntime = ApplicationRuntimeString(ProcessRuntimeMinutes(GetProcessById(LaunchApp.ProcessId)), "shortcut process");
                }
                else
                {
                    ApplicationRuntime = ApplicationRuntimeString(LaunchApp.RunningTime, "application");
                }

                //Show the messagebox
                DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + LaunchApp.Name + "?", ApplicationRuntime, "", Answers);
                if (Result != null)
                {
                    if (Result == Answer1)
                    {
                        //Check if process has multiple windows
                        IntPtr processWindowHandle = await CheckProcessMultiWindowWin32(LaunchApp.Name, processMultipleCheck);
                        if (processWindowHandle != IntPtr.Zero)
                        {
                            //Minimize the CtrlUI window
                            if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True") { await AppMinimize(true); }

                            //Force focus on the app
                            FocusProcessWindowPrepare(LaunchApp.Name, processMultipleCheck.ProcessId, processWindowHandle, 0, false, false, false);
                        }
                        else
                        {
                            Popup_Show_Status("Close", "Application has no window");
                            Debug.WriteLine("Show application has no window.");
                        }
                        return false;
                    }
                    else if (Result == Answer2)
                    {
                        Popup_Show_Status("Closing", "Closing " + LaunchApp.Name);
                        Debug.WriteLine("Closing application: " + LaunchApp.Name);

                        //Close the process
                        if (processMultipleCheck != null)
                        {
                            CloseProcessById(processMultipleCheck.ProcessId);
                        }
                        else
                        {
                            bool ClosedProcess = CloseProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(LaunchApp.PathExe), false);
                            if (ClosedProcess)
                            {
                                //Updating running status
                                LaunchApp.StatusRunning = Visibility.Collapsed;
                                LaunchApp.RunningTimeLastUpdate = 0;

                                //Update process count
                                LaunchApp.ProcessRunningCount = string.Empty;
                            }
                            else
                            {
                                Popup_Show_Status("Closing", "Failed to close the app");
                                Debug.WriteLine("Failed to close the application.");
                            }
                        }

                        return false;
                    }
                    else if (Result == Answer3)
                    {
                        Popup_Show_Status("Switch", "Restarting " + LaunchApp.Name);
                        Debug.WriteLine("Restarting application: " + LaunchApp.Name + " / " + processMultipleCheck.ProcessId + " / " + processMultipleCheck.WindowHandle);

                        string LaunchArgument = LaunchApp.Argument;
                        if (LaunchApp.Category == AppCategory.Emulator)
                        {
                            if (string.IsNullOrWhiteSpace(LaunchApp.RomPath)) { LaunchArgument = string.Empty; }
                            else { LaunchArgument += LaunchApp.RomPath; }
                        }

                        await RestartProcessWin32(processMultipleCheck.ProcessId, LaunchApp.PathExe, LaunchApp.PathLaunch, LaunchArgument);
                        return false;
                    }
                    else if (Result == Answer4)
                    {
                        Debug.WriteLine("Running new application instance.");
                        return true;
                    }
                    else if (Result == cancelString)
                    {
                        Debug.WriteLine("Cancelling the process action.");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Close", "Failed showing or closing application");
                Debug.WriteLine("Failed closing or showing the application: " + ex.Message);
            }
            return true;
        }

        //Launch a win32 databind emulator with filepicker
        async Task<bool> LaunchProcessDatabindWin32Emulator(DataBindApp LaunchApp)
        {
            try
            {
                //Check if win32 process is running
                bool AlreadyRunning = await LaunchProcessCheckWin32(LaunchApp);
                if (!AlreadyRunning)
                {
                    Debug.WriteLine("Win32 process is already running, skipping the launch.");
                    return false;
                }

                //Check if the application exe file exists
                if (!File.Exists(LaunchApp.PathExe))
                {
                    LaunchApp.StatusAvailable = Visibility.Visible;

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("App exe not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    return false;
                }
                else
                {
                    LaunchApp.StatusAvailable = Visibility.Collapsed;
                }

                //Check if the rom folder location exists
                if (!Directory.Exists(LaunchApp.PathRoms))
                {
                    LaunchApp.StatusAvailable = Visibility.Visible;

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Rom folder not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    return false;
                }
                else
                {
                    LaunchApp.StatusAvailable = Visibility.Collapsed;
                }

                //Select a file from list to launch
                vFilePickerFilterIn = new string[] { };
                vFilePickerFilterOut = new string[] { "jpg", "png" };
                vFilePickerTitle = "Rom Selection";
                vFilePickerDescription = "Please select a rom file to load in " + LaunchApp.Name + ":";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = true;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker(LaunchApp.PathRoms, -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return false; }

                string LaunchArguments = string.Empty;
                if (!string.IsNullOrWhiteSpace(vFilePickerResult.PathFile))
                {
                    LaunchArguments = LaunchApp.Argument + " \"" + vFilePickerResult.PathFile + "\"";
                    LaunchApp.RomPath = " \"" + vFilePickerResult.PathFile + "\"";

                    Popup_Show_Status("App", "Launching " + LaunchApp.Name + " with the rom");
                    Debug.WriteLine("Launching emulator: " + LaunchApp.Name + " rom: " + LaunchArguments);
                }
                else
                {
                    LaunchApp.RomPath = string.Empty;

                    Popup_Show_Status("App", "Launching " + LaunchApp.Name);
                    Debug.WriteLine("Launching emulator: " + LaunchApp.Name + " without a rom");
                }

                //Launch the Win32 application
                await ProcessLauncherWin32Prepare(LaunchApp.PathExe, LaunchApp.PathLaunch, LaunchArguments, true, true, true, false, false);
                return true;
            }
            catch
            {
                //Updating running status
                LaunchApp.StatusRunning = Visibility.Collapsed;

                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }

        //Launch a win32 databind app with filepicker
        async Task<bool> LaunchProcessDatabindWin32FilePicker(DataBindApp LaunchApp)
        {
            try
            {
                //Check if win32 process is running
                bool AlreadyRunning = await LaunchProcessCheckWin32(LaunchApp);
                if (!AlreadyRunning)
                {
                    Debug.WriteLine("Win32 process is already running, skipping the launch.");
                    return false;
                }

                //Check if the application exe file exists
                if (!File.Exists(LaunchApp.PathExe))
                {
                    LaunchApp.StatusAvailable = Visibility.Visible;

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("App exe not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    return false;
                }
                else
                {
                    LaunchApp.StatusAvailable = Visibility.Collapsed;
                }

                //Select a file from list to launch
                vFilePickerFilterIn = new string[] { };
                vFilePickerFilterOut = new string[] { };
                vFilePickerTitle = "File Selection";
                vFilePickerDescription = "Please select a file to load in " + LaunchApp.Name + ":";
                vFilePickerShowNoFile = true;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return false; }

                string LaunchArguments = string.Empty;
                if (!string.IsNullOrWhiteSpace(vFilePickerResult.PathFile))
                {
                    LaunchArguments = LaunchApp.Argument + " \"" + vFilePickerResult.PathFile + "\"";
                    Popup_Show_Status("App", "Launching " + LaunchApp.Name + " with selected file");
                    Debug.WriteLine("Launching app: " + LaunchApp.Name + " file: " + LaunchArguments);
                }
                else
                {
                    LaunchArguments = LaunchApp.Argument;
                    Popup_Show_Status("App", "Launching " + LaunchApp.Name);
                    Debug.WriteLine("Launching app: " + LaunchApp.Name + " without a file");
                }

                //Launch the Win32 application
                await ProcessLauncherWin32Prepare(LaunchApp.PathExe, LaunchApp.PathLaunch, LaunchArguments, true, true, true, false, false);
                return true;
            }
            catch
            {
                //Updating running status
                LaunchApp.StatusRunning = Visibility.Collapsed;

                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }

        //Launch a win32 databind app
        async Task<bool> LaunchProcessDatabindWin32(DataBindApp LaunchApp)
        {
            try
            {
                //Check if Win32 process is running
                bool alreadyRunning = await LaunchProcessCheckWin32(LaunchApp);
                if (!alreadyRunning)
                {
                    Debug.WriteLine("Win32 process is already running, skipping the launch.");
                    return false;
                }

                //Check if the application exe file exists
                if (!File.Exists(LaunchApp.PathExe))
                {
                    LaunchApp.StatusAvailable = Visibility.Visible;

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("App exe not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    return false;
                }
                else
                {
                    LaunchApp.StatusAvailable = Visibility.Collapsed;
                }

                //Show application launch message
                Popup_Show_Status("App", "Launching " + LaunchApp.Name);
                Debug.WriteLine("Launching Win32: " + LaunchApp.Name + " from: " + LaunchApp.Category + " path: " + LaunchApp.PathExe);

                //Launch the Win32 application
                await ProcessLauncherWin32Prepare(LaunchApp.PathExe, LaunchApp.PathLaunch, LaunchApp.Argument, true, true, true, false, false);
                return true;
            }
            catch
            {
                //Updating running status
                LaunchApp.StatusRunning = Visibility.Collapsed;

                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }

        //Get all the active Win32 processes and update the list
        async Task ListLoadProcessesWin32(IEnumerable<Process> ProcessesList, bool ShowStatus)
        {
            try
            {
                if (ConfigurationManager.AppSettings["ShowOtherProcesses"] == "False")
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        sp_Processes.Visibility = Visibility.Collapsed;
                    });
                    List_Processes.Clear();
                    GC.Collect();
                    return;
                }

                //Check if processes list is provided
                if (ProcessesList == null) { ProcessesList = Process.GetProcesses(); }

                //Check if there are active processes
                if (ProcessesList.Any())
                {
                    //Show refresh status message
                    if (ShowStatus) { Popup_Show_Status("Refresh", "Refreshing desktop apps"); }
                    //Debug.WriteLine("Checking desktop processes.");

                    List<int> ActiveProcesses = new List<int>();
                    IEnumerable<DataBindApp> CurrentApps = CombineAppLists(true, false);

                    //Add new running process if needed
                    foreach (Process AllProcess in ProcessesList)
                    {
                        try
                        {
                            ProcessType applicationType = ProcessType.Win32;
                            Visibility storeStatus = Visibility.Collapsed;

                            //Get the process title
                            string ProcessTitle = GetWindowTitleFromProcess(AllProcess);

                            //Check if application title is blacklisted
                            if (vAppsBlacklistProcess.Any(x => x.ToLower() == ProcessTitle.ToLower()))
                            {
                                continue;
                            }

                            //Check if application process is blacklisted
                            if (vAppsBlacklistProcess.Any(x => x.ToLower() == AllProcess.ProcessName.ToLower()))
                            {
                                continue;
                            }

                            //Validate the process state
                            if (!ValidateProcessState(AllProcess, true, true))
                            {
                                continue;
                            }

                            //Validate the window handle
                            if (!ValidateWindowHandle(AllProcess.MainWindowHandle))
                            {
                                continue;
                            }

                            //Get the executable path
                            string ProcessExecutablePath = GetExecutablePathFromProcess(AllProcess);

                            //Check if already in combined list and remove it
                            if (ConfigurationManager.AppSettings["HideAppProcesses"] == "True")
                            {
                                if (CurrentApps.Any(x => Path.GetFileNameWithoutExtension(x.PathExe.ToLower()) == Path.GetFileNameWithoutExtension(ProcessExecutablePath.ToLower())))
                                {
                                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                                    {
                                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => Path.GetFileNameWithoutExtension(x.PathExe.ToLower()) == Path.GetFileNameWithoutExtension(ProcessExecutablePath.ToLower()));
                                    });
                                    continue;
                                }
                            }

                            //Add active process
                            ActiveProcesses.Add(AllProcess.Id);

                            //Check the process running time
                            int ProcessRunningTime = ProcessRuntimeMinutes(AllProcess);

                            //Check if process is already in process list and update it
                            DataBindApp ProcessApp = List_Processes.Where(x => x.ProcessId == AllProcess.Id && (x.Type == ProcessType.Win32 || x.Type == ProcessType.Win32Store)).FirstOrDefault();
                            if (ProcessApp != null)
                            {
                                if (ProcessApp.Name != ProcessTitle) { ProcessApp.Name = ProcessTitle; }
                                ProcessApp.RunningTime = ProcessRunningTime;
                                continue;
                            }

                            //Get icon image from the exe path
                            BitmapImage IconBitmapImage = FileToBitmapImage(new string[] { ProcessTitle, AllProcess.ProcessName, ProcessExecutablePath }, AllProcess.MainWindowHandle, 90);

                            //Get process launch arguments
                            string ProcessArgument = GetLaunchArgumentsFromProcess(AllProcess, ProcessExecutablePath);

                            //Check if the process is a Win32Store app
                            string appUserModelId = GetAppUserModelIdFromProcess(AllProcess);
                            if (!string.IsNullOrWhiteSpace(appUserModelId))
                            {
                                ProcessExecutablePath = appUserModelId;
                                storeStatus = Visibility.Visible;
                                applicationType = ProcessType.Win32Store;
                                //Debug.WriteLine("Process " + ProcessTitle + " is a Win32Store application.");
                            }

                            //Add the process to the list
                            AVActions.ActionDispatcherInvoke(delegate
                            {
                                List_Processes.Add(new DataBindApp() { Type = applicationType, Category = AppCategory.Process, ProcessId = AllProcess.Id, ImageBitmap = IconBitmapImage, Name = ProcessTitle, ProcessName = AllProcess.ProcessName, PathExe = ProcessExecutablePath, Argument = ProcessArgument, StatusStore = storeStatus, RunningTime = ProcessRunningTime });
                            });
                        }
                        catch { }
                    }

                    //Remove no longer running and invalid processes
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => !ActiveProcesses.Contains(x.ProcessId) && (x.Type == ProcessType.Win32 || x.Type == ProcessType.Win32Store));
                    });
                }
            }
            catch { }
        }
    }
}