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
        //Get all the active Uwp processes and update the lists
        async Task ListLoadCheckProcessesUwp(bool showStatus)
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
                Process frameHostProcess = GetProcessByNameOrTitle("ApplicationFrameHost", false);
                if (frameHostProcess != null)
                {
                    //Show refresh status message
                    if (showStatus) { Popup_Show_Status("Refresh", "Refreshing store apps"); }
                    //Debug.WriteLine("Checking store processes.");

                    List<IntPtr> activeProcessesWindow = new List<IntPtr>();
                    List<string> activeProcessesPathExe = new List<string>();
                    IEnumerable<DataBindApp> currentListApps = CombineAppLists(true, false);
                    currentListApps = currentListApps.Where(x => x.StatusLauncher == Visibility.Collapsed);
                    currentListApps = currentListApps.Where(x => x.Type == ProcessType.UWP);

                    //Add new running process if needed
                    foreach (ProcessThread threadProcess in frameHostProcess.Threads)
                    {
                        try
                        {
                            //Process variables
                            Process processApp = null;
                            bool processInterfaceCheck = false;
                            IntPtr processWindowHandle = IntPtr.Zero;

                            foreach (IntPtr threadWindowHandle in EnumThreadWindows(threadProcess.Id))
                            {
                                try
                                {
                                    //Get window class name
                                    string classNameString = GetClassNameFromWindowHandle(threadWindowHandle);

                                    //Get information from frame window
                                    if (classNameString == "ApplicationFrameWindow")
                                    {
                                        //Set window handle
                                        processWindowHandle = threadWindowHandle;

                                        //Get app process
                                        IntPtr threadWindowHandleEx = FindWindowEx(threadWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                                        if (threadWindowHandleEx != IntPtr.Zero)
                                        {
                                            GetWindowThreadProcessId(threadWindowHandleEx, out int processId);
                                            if (processId > 0)
                                            {
                                                processApp = GetProcessById(processId);
                                            }
                                        }
                                    }

                                    //Check if uwp application has interface
                                    if (classNameString == "MSCTFIME UI")
                                    {
                                        processInterfaceCheck = true;
                                    }
                                }
                                catch { }
                            }

                            //Check if uwp application has interface
                            if (processInterfaceCheck)
                            {
                                //Process application type
                                ProcessType processType = ProcessType.UWP;

                                //Process Windows Store Status
                                Visibility processStatusStore = Visibility.Visible;

                                //Get the process title
                                string processTitle = GetWindowTitleFromWindowHandle(processWindowHandle);

                                //Check if application title is blacklisted
                                if (vAppsBlacklistProcess.Any(x => x.ToLower() == processTitle.ToLower()))
                                {
                                    continue;
                                }

                                //Get the application user model id
                                string processAppUserModelId = GetAppUserModelIdFromWindowHandle(processWindowHandle);
                                string processAppUserModelIdLower = processAppUserModelId.ToLower();

                                //Check all the lists for the application
                                DataBindApp existingCombinedApp = currentListApps.Where(x => x.PathExe.ToLower() == processAppUserModelIdLower).FirstOrDefault();
                                DataBindApp existingProcessApp = List_Processes.Where(x => x.ProcessMulti.WindowHandle == processWindowHandle).FirstOrDefault();

                                //Get detailed application information
                                Package appPackage = UwpGetAppPackageFromAppUserModelId(processAppUserModelId);
                                AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);
                                string processName = Path.GetFileNameWithoutExtension(appxDetails.ExecutableName);
                                string processNameLower = processName.ToLower();

                                //Check if application process is blacklisted
                                if (vAppsBlacklistProcess.Any(x => x.ToLower() == processNameLower))
                                {
                                    continue;
                                }

                                //Add active process
                                activeProcessesWindow.Add(processWindowHandle);
                                activeProcessesPathExe.Add(processAppUserModelIdLower);

                                //Check if the process has been found
                                if (processApp == null)
                                {
                                    processApp = GetUwpProcessByProcessNameAndAppUserModelId(processName, processAppUserModelId);
                                }

                                //Check the process running time
                                int processRunningTime = ProcessRuntimeMinutes(processApp);

                                //Check if the process is suspended
                                Visibility processStatusRunning = Visibility.Visible;
                                Visibility processStatusSuspended = Visibility.Collapsed;
                                if (CheckProcessSuspended(processApp.Threads))
                                {
                                    processStatusRunning = Visibility.Collapsed;
                                    processStatusSuspended = Visibility.Visible;
                                }

                                //Check if process is in combined list and update it
                                if (existingCombinedApp != null)
                                {
                                    //Update the process running time
                                    existingCombinedApp.RunningTime = processRunningTime;

                                    //Update the process running status
                                    existingCombinedApp.StatusRunning = processStatusRunning;

                                    //Update the process suspended status
                                    existingCombinedApp.StatusSuspended = processStatusSuspended;

                                    //Update the application last runtime
                                    existingCombinedApp.RunningTimeLastUpdate = Environment.TickCount;
                                }

                                //Check if already in combined list and remove it
                                if (ConfigurationManager.AppSettings["HideAppProcesses"] == "True")
                                {
                                    if (existingCombinedApp != null)
                                    {
                                        await AVActions.ActionDispatcherInvokeAsync(async delegate
                                        {
                                            await ListBoxRemoveAll(lb_Processes, List_Processes, x => x.PathExe.ToLower() == processAppUserModelIdLower);
                                        });
                                        continue;
                                    }
                                }

                                //Check if process is in process list and update it
                                if (existingProcessApp != null)
                                {
                                    //Update the process title
                                    if (existingProcessApp.Name != processTitle) { existingProcessApp.Name = processTitle; }

                                    //Update the process running time
                                    existingProcessApp.RunningTime = processRunningTime;

                                    //Update the process suspended status
                                    existingProcessApp.StatusSuspended = processStatusSuspended;

                                    continue;
                                }

                                //Load the application image
                                BitmapImage processImage = FileToBitmapImage(new string[] { processTitle, processName, appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, processWindowHandle, 90);

                                //Convert Process To ProcessMulti
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Type = processType;
                                processMultiNew.Identifier = processApp.Id;
                                processMultiNew.WindowHandle = processWindowHandle;
                                processMultiNew.Threads = processApp.Threads;

                                //Add the process to the list
                                AVActions.ActionDispatcherInvoke(delegate
                                {
                                    List_Processes.Add(new DataBindApp() { Type = processType, Category = AppCategory.Process, ProcessMulti = processMultiNew, ImageBitmap = processImage, Name = processTitle, PathExe = processAppUserModelId, StatusStore = processStatusStore, StatusSuspended = processStatusSuspended, RunningTime = processRunningTime });
                                });
                            }
                        }
                        catch { }
                    }

                    //Update the application running count and status
                    foreach (DataBindApp listApp in currentListApps)
                    {
                        int processCount = activeProcessesPathExe.Count(x => x == listApp.PathExe.ToLower());
                        if (processCount > 1)
                        {
                            listApp.RunningProcessCount = Convert.ToString(processCount);
                        }
                        else
                        {
                            listApp.RunningProcessCount = string.Empty;
                        }

                        if (processCount == 0)
                        {
                            listApp.StatusRunning = Visibility.Collapsed;
                            listApp.StatusSuspended = Visibility.Collapsed;
                            listApp.RunningTimeLastUpdate = 0;
                        }
                    }

                    //Remove no longer running and invalid processes
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => !activeProcessesWindow.Contains(x.ProcessMulti.WindowHandle) && x.Type == ProcessType.UWP);
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