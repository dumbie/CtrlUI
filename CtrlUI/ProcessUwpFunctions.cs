using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using static CtrlUI.AppVariables;
using static LibraryShared.AppImport;
using static LibraryShared.Classes;
using static LibraryShared.Processes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch an uwp application manually
        async Task ProcessLauncherUwpPrepare(string Name, string PathExe, string Argument, bool Silent, bool AllowMinimize)
        {
            try
            {
                //Show launching message
                if (!Silent)
                {
                    Popup_Show_Status("App", "Launching " + Name);
                    Debug.WriteLine("Launching UWP: " + Name + "/" + PathExe);
                }

                //Minimize the CtrlUI window
                if (AllowMinimize && ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True") { await AppMinimize(true); }

                //Launch the UWP application
                ProcessLauncherUwp(PathExe, Argument);
            }
            catch
            {
                //Show failed launch messagebox
                await LaunchProcessFailed();
            }
        }

        //Get an uwp application process id by 
        async Task<int> GetUwpProcessIdByWindowHandle(string ProcessName, string PathExe, IntPtr ProcessWindowHandle)
        {
            try
            {
                //Show the uwp process
                FocusWindowHandlePrepare(ProcessName, ProcessWindowHandle, 0, false, true, true, true, false, true);
                await Task.Delay(500);

                //Get the process id
                ProcessUwp UwpRunningNew = GetUwpProcessFromAppUserModelId(PathExe).Where(x => x.WindowHandle == ProcessWindowHandle).FirstOrDefault();
                if (UwpRunningNew != null)
                {
                    Debug.WriteLine("Uwp workaround process id: " + UwpRunningNew.ProcessId);
                    return UwpRunningNew.ProcessId;
                }
            }
            catch { }
            return -1;
        }

        //Close an uwp application by window handle
        async Task<bool> CloseProcessUwpByWindowHandle(string ProcessName, int ProcessId, IntPtr ProcessWindowHandle)
        {
            try
            {
                if (ProcessWindowHandle != IntPtr.Zero)
                {
                    //Show the process
                    FocusWindowHandlePrepare(ProcessName, ProcessWindowHandle, 0, false, true, true, true, false, true);
                    await Task.Delay(500);

                    //Close the process or app
                    return CloseProcessByWindowHandle(ProcessWindowHandle);
                }
                else if (ProcessId > 0)
                {
                    //Close the process or app
                    return CloseProcessById(ProcessId);
                }
            }
            catch { }
            return false;
        }

        //Restart a uwp process or app
        async Task RestartProcessUwp(string ProcessName, string PathExe, string Argument, int ProcessId, IntPtr ProcessWindowHandle)
        {
            try
            {
                //Close the process or app
                await CloseProcessUwpByWindowHandle(ProcessName, ProcessId, ProcessWindowHandle);
                await Task.Delay(1000);

                //Relaunch the process or app
                await ProcessLauncherUwpPrepare(ProcessName, PathExe, Argument, true, true);
            }
            catch { }
        }

        //Check Uwp process has multiple processes
        async Task<ProcessMultipleCheck> CheckMultiProcessUwp(DataBindApp LaunchApp)
        {
            try
            {
                List<DataBindString> multiAnswers = new List<DataBindString>();
                List<ProcessUwp> multiVariables = GetUwpProcessFromAppUserModelId(LaunchApp.PathExe);
                if (multiVariables.Any())
                {
                    if (multiVariables.Count > 1)
                    {
                        foreach (ProcessUwp multiProcess in multiVariables)
                        {
                            try
                            {
                                //Get the process title
                                string ProcessTitle = GetWindowTitleFromWindowHandle(multiProcess.WindowHandle);
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
                                ProcessMultipleCheck ProcessMultipleCheckNew = new ProcessMultipleCheck();
                                ProcessMultipleCheckNew.Status = "CloseAll";
                                return ProcessMultipleCheckNew;
                            }
                            else if (Result == cancelString)
                            {
                                ProcessMultipleCheck ProcessMultipleCheckNew = new ProcessMultipleCheck();
                                ProcessMultipleCheckNew.Status = "Cancel";
                                return ProcessMultipleCheckNew;
                            }
                            else
                            {
                                ProcessMultipleCheck ProcessMultipleCheckNew = new ProcessMultipleCheck();
                                ProcessMultipleCheckNew.ProcessUwp = multiVariables[multiAnswers.IndexOf(Result)];
                                return ProcessMultipleCheckNew;
                            }
                        }
                        else
                        {
                            ProcessMultipleCheck ProcessMultipleCheckNew = new ProcessMultipleCheck();
                            ProcessMultipleCheckNew.Status = "Cancel";
                            return ProcessMultipleCheckNew;
                        }
                    }
                    else
                    {
                        ProcessMultipleCheck ProcessMultipleCheckNew = new ProcessMultipleCheck();
                        ProcessMultipleCheckNew.ProcessUwp = multiVariables.FirstOrDefault();
                        return ProcessMultipleCheckNew;
                    }
                }
                else
                {
                    ProcessMultipleCheck ProcessMultipleCheckNew = new ProcessMultipleCheck();
                    ProcessMultipleCheckNew.Status = "NoProcess";
                    return ProcessMultipleCheckNew;
                }
            }
            catch
            {
                ProcessMultipleCheck ProcessMultipleCheckNew = new ProcessMultipleCheck();
                ProcessMultipleCheckNew.Status = "NoProcess";
                return ProcessMultipleCheckNew;
            }
        }

        //Check Uwp process status before launching (True = Continue)
        async Task<bool> LaunchProcessCheckUwp(DataBindApp LaunchApp)
        {
            try
            {
                //Check Uwp process has multiple processes
                ProcessMultipleCheck ProcessMultipleCheck = await CheckMultiProcessUwp(LaunchApp);
                if (ProcessMultipleCheck.Status == "NoProcess") { return true; }
                if (ProcessMultipleCheck.Status == "Cancel") { return false; }

                //Close all the processes
                if (ProcessMultipleCheck.Status == "CloseAll")
                {
                    Popup_Show_Status("Closing", "Closing " + LaunchApp.Name);
                    Debug.WriteLine("Closing uwp processes: " + LaunchApp.Name + " / " + LaunchApp.ProcessId + " / " + LaunchApp.WindowHandle);

                    //Check the process id
                    if (LaunchApp.ProcessId <= 0)
                    {
                        LaunchApp.ProcessId = await GetUwpProcessIdByWindowHandle(LaunchApp.Name, LaunchApp.PathExe, LaunchApp.WindowHandle);
                    }

                    //Close the process or app
                    bool ClosedProcess = CloseProcessById(LaunchApp.ProcessId);
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
                        Debug.WriteLine("Failed to close the uwp application.");
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

                //Show the messagebox
                DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + LaunchApp.Name + "?", ApplicationRuntimeString(LaunchApp.RunningTime, "application"), "", Answers);
                if (Result != null)
                {
                    if (Result == Answer1)
                    {
                        //Minimize the CtrlUI window
                        if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True") { await AppMinimize(true); }

                        //Force focus on the app
                        FocusWindowHandlePrepare(LaunchApp.Name, ProcessMultipleCheck.ProcessUwp.WindowHandle, 0, false, true, true, true, false, false);
                        return false;
                    }
                    else if (Result == Answer2)
                    {
                        Popup_Show_Status("Closing", "Closing " + LaunchApp.Name);
                        Debug.WriteLine("Closing uwp application: " + LaunchApp.Name + " / " + ProcessMultipleCheck.ProcessUwp.ProcessId + " / " + ProcessMultipleCheck.ProcessUwp.WindowHandle);

                        //Close the process or app
                        bool ClosedProcess = await CloseProcessUwpByWindowHandle(LaunchApp.Name, ProcessMultipleCheck.ProcessUwp.ProcessId, ProcessMultipleCheck.ProcessUwp.WindowHandle);
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
                            Debug.WriteLine("Failed to close the uwp application.");
                        }

                        return false;
                    }
                    else if (Result == Answer3)
                    {
                        Popup_Show_Status("Switch", "Restarting " + LaunchApp.Name);
                        Debug.WriteLine("Restarting uwp application: " + LaunchApp.Name);

                        await RestartProcessUwp(LaunchApp.Name, LaunchApp.PathExe, LaunchApp.Argument, ProcessMultipleCheck.ProcessUwp.ProcessId, ProcessMultipleCheck.ProcessUwp.WindowHandle);
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
            catch
            {
                Popup_Show_Status("Close", "Failed showing or closing app");
                Debug.WriteLine("Failed closing or showing the application.");
            }
            return true;
        }

        //Launch an uwp databind app
        async Task<bool> LaunchProcessDatabindUwp(DataBindApp LaunchApp)
        {
            try
            {
                //Check if uwp process is running
                bool AlreadyRunning = await LaunchProcessCheckUwp(LaunchApp);
                if (!AlreadyRunning)
                {
                    Debug.WriteLine("Uwp process is already running, skipping the launch.");
                    return false;
                }

                //Show application launch message
                Popup_Show_Status("App", "Launching " + LaunchApp.Name);
                Debug.WriteLine("Launching UWP: " + LaunchApp.Name + " from: " + LaunchApp.Category + " path: " + LaunchApp.PathExe);

                //Launch the UWP application
                await ProcessLauncherUwpPrepare(LaunchApp.Name, LaunchApp.PathExe, LaunchApp.Argument, true, true);
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

        //Get all the active Uwp processes and update the list
        async Task ListLoadProcessesUwp(bool ShowStatus)
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

                Process AllProcess = GetProcessByName("ApplicationFrameHost", false);
                if (AllProcess != null)
                {
                    //Show refresh status message
                    if (ShowStatus) { Popup_Show_Status("Refresh", "Refreshing store apps"); }
                    //Debug.WriteLine("Checking store processes.");

                    List<IntPtr> ActiveProcesses = new List<IntPtr>();
                    IEnumerable<DataBindApp> CurrentApps = CombineAppLists(true, false);

                    //Add new running process if needed
                    foreach (ProcessThread ThreadProcess in AllProcess.Threads)
                    {
                        try
                        {
                            int ProcessId = -1;
                            IntPtr WindowHandle = IntPtr.Zero;
                            string ProcessTitle = string.Empty;
                            bool ProcessUserRun = false;

                            foreach (IntPtr ThreadWindowHandle in EnumThreadWindows(ThreadProcess.Id))
                            {
                                try
                                {
                                    //Get class name
                                    string ClassNameString = GetClassNameFromWindowHandle(ThreadWindowHandle);

                                    //Get information from frame window
                                    if (ClassNameString == "ApplicationFrameWindow")
                                    {
                                        //Get window handle
                                        WindowHandle = ThreadWindowHandle;

                                        //Get window title
                                        ProcessTitle = GetWindowTitleFromWindowHandle(ThreadWindowHandle);

                                        //Get process id
                                        IntPtr ThreadWindowHandleEx = FindWindowEx(ThreadWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                                        if (ThreadWindowHandleEx != IntPtr.Zero)
                                        {
                                            GetWindowThreadProcessId(ThreadWindowHandleEx, out ProcessId);
                                        }
                                    }

                                    //Check if user started uwp application
                                    if (ClassNameString == "MSCTFIME UI")
                                    {
                                        ProcessUserRun = true;
                                    }
                                }
                                catch { }
                            }

                            if (ProcessUserRun)
                            {
                                //Validate the process by string name
                                if (!ValidateProcessByName(ProcessTitle, true, true) || !ValidateProcessByName(AllProcess.ProcessName, true, true))
                                {
                                    continue;
                                }

                                //Get the executable path
                                string ProcessExecutablePath = GetAppUserModelIdFromWindowHandle(WindowHandle);

                                //Check if already in combined list and remove it
                                if (ConfigurationManager.AppSettings["HideAppProcesses"] == "True")
                                {
                                    if (CurrentApps.Any(x => x.PathExe.ToLower() == ProcessExecutablePath.ToLower()))
                                    {
                                        await AVActions.ActionDispatcherInvokeAsync(async delegate
                                        {
                                            await ListBoxRemoveAll(lb_Processes, List_Processes, x => x.PathExe.ToLower() == ProcessExecutablePath.ToLower(), true);
                                        });
                                        continue;
                                    }
                                }

                                //Add active process
                                ActiveProcesses.Add(WindowHandle);

                                //Check the process running time
                                int ProcessRunningTime = ProcessRuntimeMinutes(GetProcessById(ProcessId));

                                //Check if process is already in process list and update it
                                DataBindApp ProcessApp = List_Processes.Where(z => z.WindowHandle == WindowHandle && z.Type == "UWP").FirstOrDefault();
                                if (ProcessApp != null)
                                {
                                    if (ProcessId != -1 && ProcessApp.ProcessId != ProcessId) { ProcessApp.ProcessId = ProcessId; }
                                    if (ProcessApp.Name != ProcessTitle) { ProcessApp.Name = ProcessTitle; }
                                    ProcessApp.RunningTime = ProcessRunningTime;
                                    continue;
                                }

                                //Get image from the exe path
                                string UwpImagePath = GetUwpAppImagePath(ProcessExecutablePath);
                                BitmapImage AppBitmapImage = FileToBitmapImage(new string[] { ProcessTitle, AllProcess.ProcessName, UwpImagePath }, IntPtr.Zero, 90);

                                //Set Windows Store Status
                                Visibility StoreStatus = Visibility.Visible;

                                //Add the process to the list
                                AVActions.ActionDispatcherInvoke(delegate
                                {
                                    List_Processes.Add(new DataBindApp() { ProcessId = ProcessId, WindowHandle = WindowHandle, Category = "Process", Type = "UWP", ImageBitmap = AppBitmapImage, Name = ProcessTitle, ProcessName = ProcessTitle, PathExe = ProcessExecutablePath, StatusStore = StoreStatus, RunningTime = ProcessRunningTime });
                                });
                            }
                        }
                        catch { }
                    }

                    //Remove no longer running and invalid processes
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => !ActiveProcesses.Contains(x.WindowHandle) && x.Type == "UWP", true);
                    });
                }
                else
                {
                    //There is no uwp application running
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => x.Type == "UWP", true);
                    });
                }
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
                await ProcessLauncherUwpPrepare(vFilePickerResult.Name, vFilePickerResult.PathFile, string.Empty, false, true);
            }
            catch { }
        }

        //List all uwp applications
        void UwpAppListAll(ObservableCollection<DataBindFile> targetList)
        {
            try
            {
                //Set application filters
                string[] WhiteListFamily = new string[] { "Microsoft.MicrosoftEdge_8wekyb3d8bbwe" };
                string[] BlackListIdentifier = new string[] { "Microsoft.MicrosoftEdge_8wekyb3d8bbwe!PdfReader" };

                //Get all the installed UWP apps
                PackageManager DeployPackageManager = new PackageManager();
                string CurrentUserIdentity = WindowsIdentity.GetCurrent().User.Value;
                IEnumerable<Package> AppPackages = DeployPackageManager.FindPackagesForUser(CurrentUserIdentity);
                foreach (Package AppPackage in AppPackages)
                {
                    try
                    {
                        string AppName = string.Empty;
                        string AppImagePath = string.Empty;
                        string AppIdentifier = string.Empty;
                        string AppFamilyName = AppPackage.Id.FamilyName;

                        //Check if the application is in whitelist
                        if (!WhiteListFamily.Contains(AppFamilyName))
                        {
                            //Filter out system apps and others
                            if (AppPackage.IsBundle) { continue; }
                            if (AppPackage.IsOptional) { continue; }
                            if (AppPackage.IsFramework) { continue; }
                            if (AppPackage.IsResourcePackage) { continue; }
                            if (AppPackage.SignatureKind != PackageSignatureKind.Store) { continue; }
                        }

                        //Get detailed application information
                        GetUwpAppDetailsFromPackage(AppPackage, ref AppName, ref AppImagePath, ref AppIdentifier);

                        //Check if an application name is valid
                        if (string.IsNullOrWhiteSpace(AppName) || AppName.StartsWith("ms-resource")) { continue; }

                        //Check if the application is in blacklist
                        if (BlackListIdentifier.Contains(AppIdentifier)) { continue; }

                        //Load the application image
                        BitmapImage UwpListImage = FileToBitmapImage(new string[] { AppImagePath }, IntPtr.Zero, 50);

                        //Add the application to the list
                        targetList.Add(new DataBindFile() { Type = "App", Name = AppName, PathFile = AppIdentifier, PathImage = AppImagePath, ImageBitmap = UwpListImage });
                    }
                    catch { }
                }
            }
            catch { }
        }

        //Update all uwp application images
        void UpdateUwpAppImages()
        {
            try
            {
                Debug.WriteLine("Checking all uwp application images for changes.");
                bool UpdatedImages = false;

                //Update all the uwp apps image paths
                foreach (DataBindApp ListApp in CombineAppLists(false, false).Where(x => x.Type == "UWP"))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(ListApp.PathImage) && !File.Exists(ListApp.PathImage))
                        {
                            Debug.WriteLine("Uwp application image not found: " + ListApp.PathImage);
                            ListApp.PathImage = GetUwpAppImagePath(ListApp.PathExe);
                            ListApp.ImageBitmap = FileToBitmapImage(new string[] { ListApp.Name, ListApp.PathImage }, IntPtr.Zero, 90);
                            UpdatedImages = true;
                        }
                    }
                    catch { }
                }

                //Save the updated uwp application images
                if (UpdatedImages) { JsonSaveApps(); }
            }
            catch { }
        }
    }
}