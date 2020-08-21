using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get all the active UWP processes and update the lists
        async Task ListLoadCheckProcessesUwp(List<int> activeProcessesId, List<IntPtr> activeProcessesWindow, IEnumerable<DataBindApp> currentListApps, bool showStatus)
        {
            try
            {
                //Check if there are active processes
                Process frameHostProcess = GetProcessByNameOrTitle("ApplicationFrameHost", false);
                if (frameHostProcess != null)
                {
                    //Show refresh status message
                    if (showStatus) { await Notification_Send_Status("Refresh", "Refreshing store apps"); }
                    //Debug.WriteLine("Checking store processes.");

                    //Add new running process if needed
                    foreach (ProcessThread threadProcess in frameHostProcess.Threads)
                    {
                        try
                        {
                            //Process variables
                            Process processApp = null;
                            int processIdentifier = -1;
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
                                            GetWindowThreadProcessId(threadWindowHandleEx, out processIdentifier);
                                            if (processIdentifier > 0)
                                            {
                                                processApp = GetProcessById(processIdentifier);
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
                                if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == processTitle.ToLower()))
                                {
                                    continue;
                                }

                                //Get the application user model id
                                string processPathExe = GetAppUserModelIdFromWindowHandle(processWindowHandle);
                                if (string.IsNullOrWhiteSpace(processPathExe))
                                {
                                    processPathExe = GetAppUserModelIdFromProcess(processApp);
                                }
                                string processPathExeLower = processPathExe.ToLower();

                                //Get detailed application information
                                Package appPackage = UwpGetAppPackageByAppUserModelId(processPathExe);
                                AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);
                                string processNameExe = appxDetails.ExecutableName;
                                string processNameExeLower = processNameExe.ToLower();
                                string processNameExeNoExt = Path.GetFileNameWithoutExtension(processNameExe);
                                string processNameExeNoExtLower = processNameExeNoExt.ToLower();

                                //Check if application name is blacklisted
                                if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == processNameExeNoExtLower))
                                {
                                    continue;
                                }

                                //Check if the process has been found
                                if (processApp == null)
                                {
                                    processApp = GetUwpProcessByProcessNameAndAppUserModelId(processNameExeNoExt, processPathExe);
                                    processIdentifier = processApp.Id;
                                }

                                //Add active process to the list
                                activeProcessesId.Add(processIdentifier);
                                activeProcessesWindow.Add(processWindowHandle);

                                //Check the process running time
                                int processRunningTime = ProcessRuntimeMinutes(processApp);

                                //Check if the process is suspended
                                Visibility processStatusRunning = Visibility.Visible;
                                Visibility processStatusSuspended = Visibility.Collapsed;
                                ProcessThreadCollection processThreads = processApp.Threads;
                                if (CheckProcessSuspended(processThreads))
                                {
                                    processStatusRunning = Visibility.Collapsed;
                                    processStatusSuspended = Visibility.Visible;
                                }

                                //Convert Process To ProcessMulti
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Type = processType;
                                processMultiNew.Identifier = processIdentifier;
                                processMultiNew.WindowHandle = processWindowHandle;

                                //Set the application search filters
                                Func<DataBindApp, bool> filterCombinedApp = x => x.PathExe != null && x.PathExe.ToLower() == processPathExeLower;
                                Func<DataBindApp, bool> filterProcessApp = x => x.ProcessMulti.Any(z => z.WindowHandle == processWindowHandle);

                                //Check all the lists for the application
                                IEnumerable<DataBindApp> existingCombinedApps = currentListApps.Where(filterCombinedApp);
                                IEnumerable<DataBindApp> existingProcessApps = List_Processes.Where(filterProcessApp);
                                bool appUpdatedContinueLoop = false;

                                //Check if process is in combined list and update it
                                foreach (DataBindApp existingCombinedApp in existingCombinedApps)
                                {
                                    //Update the process running time
                                    existingCombinedApp.RunningTime = processRunningTime;

                                    //Update the process running status
                                    existingCombinedApp.StatusRunning = processStatusRunning;

                                    //Update the process suspended status
                                    existingCombinedApp.StatusSuspended = processStatusSuspended;

                                    //Update the application last runtime
                                    existingCombinedApp.RunningTimeLastUpdate = Environment.TickCount;

                                    //Add the new process multi application
                                    if (!existingCombinedApp.ProcessMulti.Any(x => x.WindowHandle == processWindowHandle))
                                    {
                                        existingCombinedApp.ProcessMulti.Add(processMultiNew);
                                    }

                                    //Remove app from processes list
                                    if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideAppProcesses")))
                                    {
                                        await ListBoxRemoveAll(lb_Processes, List_Processes, filterCombinedApp);
                                        appUpdatedContinueLoop = true;
                                    }
                                }
                                if (appUpdatedContinueLoop) { continue; }

                                //Check if process is in process list and update it
                                foreach (DataBindApp existingProcessApp in existingProcessApps)
                                {
                                    //Update the process title
                                    if (existingProcessApp.Name != processTitle) { existingProcessApp.Name = processTitle; }

                                    //Update the process running time
                                    existingProcessApp.RunningTime = processRunningTime;

                                    //Update the process suspended status
                                    existingProcessApp.StatusSuspended = processStatusSuspended;

                                    //Update the process multi identifier
                                    foreach (ProcessMulti processMulti in existingProcessApp.ProcessMulti.Where(x => x.Identifier <= 0))
                                    {
                                        processMulti.Identifier = processIdentifier;
                                    }

                                    //Update the process multi window handle
                                    foreach (ProcessMulti processMulti in existingProcessApp.ProcessMulti.Where(x => x.WindowHandle == IntPtr.Zero))
                                    {
                                        processMulti.WindowHandle = processWindowHandle;
                                    }

                                    appUpdatedContinueLoop = true;
                                }
                                if (appUpdatedContinueLoop) { continue; }

                                //Load the application image
                                BitmapImage processImageBitmap = FileToBitmapImage(new string[] { processTitle, processNameExeNoExt, appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, vImageSourceFolders, vImageBackupSource, processWindowHandle, 90, 0);

                                //Create new ProcessMulti list
                                List<ProcessMulti> listProcessMulti = new List<ProcessMulti>();
                                listProcessMulti.Add(processMultiNew);

                                //Add the process to the list
                                DataBindApp dataBindApp = new DataBindApp() { Type = processType, Category = AppCategory.Process, ProcessMulti = listProcessMulti, ImageBitmap = processImageBitmap, Name = processTitle, NameExe = processNameExe, PathExe = processPathExe, StatusStore = processStatusStore, StatusSuspended = processStatusSuspended, RunningTime = processRunningTime };
                                await ListBoxAddItem(lb_Processes, List_Processes, dataBindApp, false, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Failed adding UWP application: " + ex.Message);
                        }
                    }
                }
            }
            catch { }
        }

        //List all available uwp applications
        async Task ListLoadAllUwpApplications(ListBox targetListBox, ObservableCollection<DataBindFile> targetList)
        {
            try
            {
                //Set uwp application filters
                string[] whiteListFamilyName = { "Microsoft.MicrosoftEdge_8wekyb3d8bbwe" };
                string[] blackListFamilyNameId = { "Microsoft.MicrosoftEdge_8wekyb3d8bbwe!PdfReader" };

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
                        BitmapImage uwpListImage = FileToBitmapImage(new string[] { appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 50, 0);

                        //Add the application to the list
                        DataBindFile dataBindFile = new DataBindFile() { FileType = FileType.UwpApp, Name = appxDetails.DisplayName, NameExe = appxDetails.ExecutableName, PathFile = appxDetails.FamilyNameId, PathFull = appxDetails.FullPackageName, PathImage = appxDetails.SquareLargestLogoPath, ImageBitmap = uwpListImage };
                        await ListBoxAddItem(targetListBox, targetList, dataBindFile, false, false);
                    }
                    catch { }
                }

                //Sort the application list by name
                SortObservableCollection(lb_FilePicker, List_FilePicker, x => x.Name, null, true);
            }
            catch { }
        }
    }
}