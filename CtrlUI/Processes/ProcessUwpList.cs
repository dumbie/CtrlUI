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

                //Check if there are active processes
                Process allProcess = GetProcessByNameOrTitle("ApplicationFrameHost", false);
                if (allProcess != null)
                {
                    //Show refresh status message
                    if (ShowStatus) { Popup_Show_Status("Refresh", "Refreshing store apps"); }
                    //Debug.WriteLine("Checking store processes.");

                    List<IntPtr> activeProcesses = new List<IntPtr>();
                    IEnumerable<DataBindApp> currentListApps = CombineAppLists(true, false);

                    //Add new running process if needed
                    foreach (ProcessThread threadProcess in allProcess.Threads)
                    {
                        try
                        {
                            int processId = -1;
                            IntPtr processWindowHandle = IntPtr.Zero;
                            string processTitle = string.Empty;
                            bool processUserRun = false;

                            foreach (IntPtr threadWindowHandle in EnumThreadWindows(threadProcess.Id))
                            {
                                try
                                {
                                    //Get class name
                                    string classNameString = GetClassNameFromWindowHandle(threadWindowHandle);

                                    //Get information from frame window
                                    if (classNameString == "ApplicationFrameWindow")
                                    {
                                        //Set window handle
                                        processWindowHandle = threadWindowHandle;

                                        //Get window title
                                        processTitle = GetWindowTitleFromWindowHandle(threadWindowHandle);

                                        //Get process id
                                        IntPtr ThreadWindowHandleEx = FindWindowEx(threadWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                                        if (ThreadWindowHandleEx != IntPtr.Zero)
                                        {
                                            GetWindowThreadProcessId(ThreadWindowHandleEx, out processId);
                                        }
                                    }

                                    //Check if user started uwp application
                                    if (classNameString == "MSCTFIME UI")
                                    {
                                        processUserRun = true;
                                    }
                                }
                                catch { }
                            }

                            //Check if user started uwp application
                            if (processUserRun)
                            {
                                //Check if application title is blacklisted
                                if (vAppsBlacklistProcess.Any(x => x.ToLower() == processTitle.ToLower()))
                                {
                                    continue;
                                }

                                //Check if application process is blacklisted
                                if (vAppsBlacklistProcess.Any(x => x.ToLower() == allProcess.ProcessName.ToLower()))
                                {
                                    continue;
                                }

                                //Get the application user model id
                                string processAppUserModelId = GetAppUserModelIdFromWindowHandle(processWindowHandle);
                                string processAppUserModelIdLower = processAppUserModelId.ToLower();

                                //Get detailed application information
                                Package appPackage = UwpGetAppPackageFromAppUserModelId(processAppUserModelId);
                                AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);

                                //Check if already in combined list and remove it
                                if (ConfigurationManager.AppSettings["HideAppProcesses"] == "True")
                                {
                                    if (currentListApps.Any(x => x.PathExe.ToLower() == processAppUserModelIdLower))
                                    {
                                        //Debug.WriteLine("Process is already in other list: " + ProcessExecutablePathLower);
                                        await AVActions.ActionDispatcherInvokeAsync(async delegate
                                        {
                                            await ListBoxRemoveAll(lb_Processes, List_Processes, x => x.PathExe.ToLower() == processAppUserModelIdLower);
                                        });
                                        continue;
                                    }
                                }

                                //Add active process
                                activeProcesses.Add(processWindowHandle);

                                //Check if process id has been found
                                if (processId <= 0)
                                {
                                    string processNameExe = Path.GetFileNameWithoutExtension(appxDetails.ExecutableName);
                                    Process uwpProcess = GetUwpProcessByProcessNameAndAppUserModelId(processNameExe, processAppUserModelId);
                                    if (uwpProcess != null)
                                    {
                                        processId = uwpProcess.Id;
                                    }
                                }

                                //Check if process is already in process list and update it
                                DataBindApp existingProcessApp = List_Processes.Where(x => x.ProcessMulti.WindowHandle == processWindowHandle && x.Type == ProcessType.UWP).FirstOrDefault();
                                if (existingProcessApp != null)
                                {
                                    if (processId > 0) { existingProcessApp.ProcessMulti.Identifier = processId; }
                                    if (existingProcessApp.Name != processTitle) { existingProcessApp.Name = processTitle; }

                                    //Get the current process
                                    Process currentProcess1 = GetProcessById(existingProcessApp.ProcessMulti.Identifier);

                                    //Check the process running time
                                    if (currentProcess1 != null)
                                    {
                                        existingProcessApp.RunningTime = ProcessRuntimeMinutes(currentProcess1);
                                    }

                                    //Check if the process is suspended
                                    if (currentProcess1 != null && CheckProcessSuspended(currentProcess1.Threads))
                                    {
                                        existingProcessApp.StatusSuspended = Visibility.Visible;
                                    }
                                    else
                                    {
                                        existingProcessApp.StatusSuspended = Visibility.Collapsed;
                                    }

                                    continue;
                                }

                                //Get the current process
                                Process currentProcess2 = GetProcessById(processId);

                                //Check the process running time
                                int processRunningTime = -1;
                                if (currentProcess2 != null)
                                {
                                    processRunningTime = ProcessRuntimeMinutes(currentProcess2);
                                }

                                //Check if the process is suspended
                                Visibility suspendedStatus = Visibility.Collapsed;
                                if (currentProcess2 != null && CheckProcessSuspended(currentProcess2.Threads))
                                {
                                    suspendedStatus = Visibility.Visible;
                                }

                                //Load the application image
                                BitmapImage AppBitmapImage = FileToBitmapImage(new string[] { processTitle, allProcess.ProcessName, appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, IntPtr.Zero, 90);

                                //Set Windows Store Status
                                Visibility StoreStatus = Visibility.Visible;

                                //Create new ProcessMulti
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Type = ProcessType.UWP;
                                processMultiNew.Identifier = processId;
                                processMultiNew.WindowHandle = processWindowHandle;

                                //Add the process to the list
                                AVActions.ActionDispatcherInvoke(delegate
                                {
                                    List_Processes.Add(new DataBindApp() { Type = ProcessType.UWP, Category = AppCategory.Process, ProcessMulti = processMultiNew, ImageBitmap = AppBitmapImage, Name = processTitle, PathExe = processAppUserModelId, StatusStore = StoreStatus, StatusSuspended = suspendedStatus, RunningTime = processRunningTime });
                                });
                            }
                        }
                        catch { }
                    }

                    //Remove no longer running and invalid processes
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => !activeProcesses.Contains(x.ProcessMulti.WindowHandle) && x.Type == ProcessType.UWP);
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

        //List all uwp applications
        void ListLoadAllUwpApplications(ObservableCollection<DataBindFile> targetList)
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
    }
}