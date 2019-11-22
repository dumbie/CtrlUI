using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
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
                                string ProcessExecutablePathLower = ProcessExecutablePath.ToLower();

                                //Check if already in combined list and remove it
                                if (ConfigurationManager.AppSettings["HideAppProcesses"] == "True")
                                {
                                    if (CurrentApps.Any(x => x.PathExe.ToLower() == ProcessExecutablePathLower))
                                    {
                                        //Debug.WriteLine("Process is already in other list: " + ProcessExecutablePathLower);
                                        await AVActions.ActionDispatcherInvokeAsync(async delegate
                                        {
                                            await ListBoxRemoveAll(lb_Processes, List_Processes, x => x.PathExe.ToLower() == ProcessExecutablePathLower);
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