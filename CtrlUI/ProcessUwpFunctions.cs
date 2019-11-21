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
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch an uwp application manually
        async Task ProcessLauncherUwpPrepare(string Name, string PathExe, string Argument, bool Silent, bool AllowMinimize)
        {
            try
            {
                //Check if the application exists
                if (UwpGetAppPackageFromAppUserModelId(PathExe) == null)
                {
                    Popup_Show_Status("Close", "Application not found");
                    Debug.WriteLine("Launch application not found, possibly uninstalled.");
                    return;
                }

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

        //Check Uwp process has multiple processes
        async Task<ProcessMulti> CheckMultiProcessUwp(DataBindApp LaunchApp)
        {
            try
            {
                List<DataBindString> multiAnswers = new List<DataBindString>();
                List<ProcessMulti> multiVariables = UwpGetProcessFromAppUserModelId(LaunchApp.PathExe);
                if (multiVariables.Any())
                {
                    if (multiVariables.Count > 1)
                    {
                        foreach (ProcessMulti multiProcess in multiVariables)
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
                                return multiVariables[multiAnswers.IndexOf(Result)];
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
                        return multiVariables.FirstOrDefault();
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

        //Check Uwp process status before launching (True = Continue)
        async Task<bool> LaunchProcessCheckUwp(DataBindApp LaunchApp)
        {
            try
            {
                Debug.WriteLine("Checking launch process UWP: " + LaunchApp.Name + " / " + LaunchApp.ProcessId + " / " + LaunchApp.WindowHandle);

                //Check if uwp app has any processes
                ProcessMulti processMultipleCheck = await CheckMultiProcessUwp(LaunchApp);
                if (processMultipleCheck.Status == "NoProcess")
                {
                    //Check if Win32Store app has any processes
                    Debug.WriteLine("Found no uwp process, checking for Win32Store process.");
                    processMultipleCheck = await CheckMultiProcessWin32(LaunchApp);
                }

                //Check the multiple check result
                if (processMultipleCheck.Status == "NoProcess") { return true; }
                if (processMultipleCheck.Status == "Cancel") { return false; }
                if (processMultipleCheck.Status == "CloseAll")
                {
                    Popup_Show_Status("Closing", "Closing " + LaunchApp.Name);
                    Debug.WriteLine("Closing uwp processes: " + LaunchApp.Name + " / " + LaunchApp.ProcessId + " / " + LaunchApp.WindowHandle);

                    //Get the process id by window handle
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
                        FocusProcessWindowPrepare(LaunchApp.Name, processMultipleCheck.ProcessId, processMultipleCheck.WindowHandle, 0, false, false, false);

                        return false;
                    }
                    else if (Result == Answer2)
                    {
                        Popup_Show_Status("Closing", "Closing " + LaunchApp.Name);
                        Debug.WriteLine("Closing uwp application: " + LaunchApp.Name + " / " + processMultipleCheck.ProcessId + " / " + processMultipleCheck.WindowHandle);

                        //Close the process or app
                        bool ClosedProcess = await CloseProcessUwpByWindowHandle(LaunchApp.Name, processMultipleCheck.ProcessId, processMultipleCheck.WindowHandle);
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

                        await RestartProcessUwp(LaunchApp.Name, LaunchApp.PathExe, LaunchApp.Argument, processMultipleCheck.ProcessId, processMultipleCheck.WindowHandle);
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
                Popup_Show_Status("Close", "Failed showing or closing app");
                Debug.WriteLine("Failed closing or showing the app: " + ex.Message);
            }
            return true;
        }

        //Launch an uwp databind app
        async Task<bool> LaunchProcessDatabindUwp(DataBindApp LaunchApp)
        {
            try
            {
                //Check if UWP process is running
                bool alreadyRunning = await LaunchProcessCheckUwp(LaunchApp);
                if (!alreadyRunning)
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

                Process AllProcess = GetProcessByNameOrTitle("ApplicationFrameHost", false);
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

                                //Get the executable path
                                string ProcessExecutablePath = GetAppUserModelIdFromWindowHandle(WindowHandle);

                                //Check if already in combined list and remove it
                                if (ConfigurationManager.AppSettings["HideAppProcesses"] == "True")
                                {
                                    if (CurrentApps.Any(x => x.PathExe.ToLower() == ProcessExecutablePath.ToLower()))
                                    {
                                        await AVActions.ActionDispatcherInvokeAsync(async delegate
                                        {
                                            await ListBoxRemoveAll(lb_Processes, List_Processes, x => x.PathExe.ToLower() == ProcessExecutablePath.ToLower());
                                        });
                                        continue;
                                    }
                                }

                                //Add active process
                                ActiveProcesses.Add(WindowHandle);

                                //Check the process running time
                                int ProcessRunningTime = ProcessRuntimeMinutes(GetProcessById(ProcessId));

                                //Check if process is already in process list and update it
                                DataBindApp ProcessApp = List_Processes.Where(z => z.WindowHandle == WindowHandle && z.Type == ProcessType.UWP).FirstOrDefault();
                                if (ProcessApp != null)
                                {
                                    if (ProcessId != -1 && ProcessApp.ProcessId != ProcessId) { ProcessApp.ProcessId = ProcessId; }
                                    if (ProcessApp.Name != ProcessTitle) { ProcessApp.Name = ProcessTitle; }
                                    ProcessApp.RunningTime = ProcessRunningTime;
                                    continue;
                                }

                                //Get detailed application information
                                Package appPackage = UwpGetAppPackageFromAppUserModelId(ProcessExecutablePath);
                                AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);

                                //Load the application image
                                BitmapImage AppBitmapImage = FileToBitmapImage(new string[] { ProcessTitle, AllProcess.ProcessName, appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, IntPtr.Zero, 90);

                                //Set Windows Store Status
                                Visibility StoreStatus = Visibility.Visible;

                                //Add the process to the list
                                AVActions.ActionDispatcherInvoke(delegate
                                {
                                    List_Processes.Add(new DataBindApp() { Type = ProcessType.UWP, Category = AppCategory.Process, ProcessId = ProcessId, WindowHandle = WindowHandle, ImageBitmap = AppBitmapImage, Name = ProcessTitle, ProcessName = ProcessTitle, PathExe = ProcessExecutablePath, StatusStore = StoreStatus, RunningTime = ProcessRunningTime });
                                });
                            }
                        }
                        catch { }
                    }

                    //Remove no longer running and invalid processes
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => !ActiveProcesses.Contains(x.WindowHandle) && x.Type == ProcessType.UWP);
                    });
                }
                else
                {
                    //There is no uwp application running
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => x.Type == ProcessType.UWP);
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
                //Set uwp application filters
                string[] whiteListFamilyName = new string[] { "Microsoft.MicrosoftEdge_8wekyb3d8bbwe" };
                string[] blackListFamilyNameId = new string[] { "Microsoft.MicrosoftEdge_8wekyb3d8bbwe!PdfReader" };

                //Get all the installed uwp apps
                PackageManager deployPackageManager = new PackageManager();
                string currentUserIdentity = WindowsIdentity.GetCurrent().User.Value;
                IEnumerable<Package> appPackages = deployPackageManager.FindPackagesForUser(currentUserIdentity);
                foreach (Package appPackage in appPackages)
                {
                    try
                    {
                        //Get basic application information
                        string appFamilyName = appPackage.Id.FamilyName;

                        //Check if the application is in whitelist
                        if (!whiteListFamilyName.Contains(appFamilyName))
                        {
                            //Filter out system apps and others
                            if (appPackage.IsBundle) { continue; }
                            if (appPackage.IsOptional) { continue; }
                            if (appPackage.IsFramework) { continue; }
                            if (appPackage.IsResourcePackage) { continue; }
                            if (appPackage.SignatureKind != PackageSignatureKind.Store) { continue; }
                        }

                        //Get detailed application information
                        AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);

                        //Check if application name is valid
                        if (string.IsNullOrWhiteSpace(appxDetails.DisplayName) || appxDetails.DisplayName.StartsWith("ms-resource"))
                        {
                            continue;
                        }

                        //Check if the application is in blacklist
                        if (blackListFamilyNameId.Contains(appxDetails.FamilyNameId))
                        {
                            continue;
                        }

                        //Load the application image
                        BitmapImage UwpListImage = FileToBitmapImage(new string[] { appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, IntPtr.Zero, 50);

                        //Add the application to the list
                        targetList.Add(new DataBindFile() { Type = "App", Name = appxDetails.DisplayName, NameExe = appxDetails.ExecutableName, PathFile = appxDetails.FamilyNameId, PathImage = appxDetails.SquareLargestLogoPath, ImageBitmap = UwpListImage });
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
                foreach (DataBindApp ListApp in CombineAppLists(false, false).Where(x => x.Type == ProcessType.UWP || x.Type == ProcessType.Win32Store))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(ListApp.PathImage) && !File.Exists(ListApp.PathImage))
                        {
                            Debug.WriteLine("Uwp application image not found: " + ListApp.PathImage);

                            //Get detailed application information
                            Package appPackage = UwpGetAppPackageFromAppUserModelId(ListApp.PathExe);
                            AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);

                            //Update the application icons
                            ListApp.PathImage = appxDetails.SquareLargestLogoPath;
                            ListApp.ImageBitmap = FileToBitmapImage(new string[] { ListApp.Name, appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, IntPtr.Zero, 90);
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