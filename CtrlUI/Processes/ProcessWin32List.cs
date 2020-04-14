using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
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
        //Get all the active Win32 processes and update the lists
        async Task ListLoadCheckProcessesWin32(IEnumerable<Process> processesList, List<int> activeProcessesId, List<IntPtr> activeProcessesWindow, IEnumerable<DataBindApp> currentListApps, bool showStatus)
        {
            try
            {
                //Check if processes list is provided
                if (processesList == null)
                {
                    processesList = Process.GetProcesses();
                }

                //Check if there are active processes
                if (processesList.Any())
                {
                    //Show refresh status message
                    if (showStatus) { Popup_Show_Status("Refresh", "Refreshing desktop apps"); }
                    //Debug.WriteLine("Checking desktop processes.");

                    //Add new running process if needed
                    foreach (Process processApp in processesList)
                    {
                        try
                        {
                            //Process identifier
                            int processIdentifier = processApp.Id;

                            //Process window handle
                            IntPtr processWindowHandle = processApp.MainWindowHandle;

                            //Process application type
                            ProcessType processType = ProcessType.Win32;

                            //Process Windows Store Status
                            Visibility processStatusStore = Visibility.Collapsed;

                            //Get the process title
                            string processName = GetWindowTitleFromProcess(processApp);

                            //Check if application title is blacklisted
                            if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == processName.ToLower()))
                            {
                                continue;
                            }

                            //Validate the process state
                            if (!ValidateProcessState(processApp, true, true))
                            {
                                continue;
                            }

                            //Validate the window handle
                            bool windowValidation = ValidateWindowHandle(processWindowHandle);

                            //Get the executable path
                            string processPathExe = GetExecutablePathFromProcess(processApp);
                            string processPathExeLower = processPathExe.ToLower();
                            string processPathImage = processPathExe;
                            string processNameExe = Path.GetFileName(processPathExe);
                            string processNameExeLower = processNameExe.ToLower();
                            string processNameExeNoExt = Path.GetFileNameWithoutExtension(processPathExe);
                            string processNameExeNoExtLower = processNameExeNoExt.ToLower();

                            //Get the process launch arguments
                            string processArgument = GetLaunchArgumentsFromProcess(processApp, processPathExe);

                            //Check if application name is blacklisted
                            if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == processNameExeNoExtLower))
                            {
                                continue;
                            }

                            //Check explorer process
                            if (processNameExeLower == "explorer.exe")
                            {
                                processName = "File Explorer";
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

                            //Set the application search filters
                            Func<DataBindApp, bool> filterCombinedApp = x => x.PathExe != null && Path.GetFileNameWithoutExtension(x.PathExe).ToLower() == processNameExeNoExtLower;
                            Func<DataBindApp, bool> filterProcessApp = x => x.ProcessMulti.Any(z => z.WindowHandle == processWindowHandle);

                            //Check if process is a Win32Store app
                            string processAppUserModelId = GetAppUserModelIdFromProcess(processApp);
                            if (!string.IsNullOrWhiteSpace(processAppUserModelId))
                            {
                                processType = ProcessType.Win32Store;
                                processPathExe = processAppUserModelId;
                                processPathExeLower = processAppUserModelId.ToLower();
                                processStatusStore = Visibility.Visible;
                                filterCombinedApp = x => x.PathExe != null && x.PathExe.ToLower() == processPathExeLower;

                                //Validate the window handle
                                if (!windowValidation)
                                {
                                    //Debug.WriteLine(processName + " is an invalid Win32Store application.");
                                    continue;
                                }
                                else
                                {
                                    //Debug.WriteLine(processName + " is a Win32Store application.");}
                                }
                            }

                            //Convert Process To ProcessMulti
                            ProcessMulti processMultiNew = new ProcessMulti();
                            processMultiNew.Type = processType;
                            processMultiNew.Identifier = processIdentifier;
                            processMultiNew.WindowHandle = processWindowHandle;
                            processMultiNew.Argument = processArgument;

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
                                if (ConfigurationManager.AppSettings["HideAppProcesses"] == "True")
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
                                if (existingProcessApp.Name != processName) { existingProcessApp.Name = processName; }

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

                            //Validate the window handle
                            if (!windowValidation)
                            {
                                continue;
                            }

                            //Load Windows store application images
                            string storeImageSquare = string.Empty;
                            string storeImageWide = string.Empty;
                            if (processType == ProcessType.Win32Store)
                            {
                                Package appPackage = UwpGetAppPackageByAppUserModelId(processPathExe);
                                AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);
                                storeImageSquare = appxDetails.SquareLargestLogoPath;
                                storeImageWide = appxDetails.WideLargestLogoPath;
                            }

                            //Load the application image
                            BitmapImage processImageBitmap = FileToBitmapImage(new string[] { processName, processNameExeNoExt, storeImageSquare, storeImageWide, processPathImage, processPathExe }, processWindowHandle, 90, 0);

                            //Create new ProcessMulti list
                            List<ProcessMulti> listProcessMulti = new List<ProcessMulti>();
                            listProcessMulti.Add(processMultiNew);

                            //Add the process to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = processType, Category = AppCategory.Process, ProcessMulti = listProcessMulti, ImageBitmap = processImageBitmap, Name = processName, NameExe = processNameExe, PathExe = processPathExe, Argument = processArgument, StatusStore = processStatusStore, StatusSuspended = processStatusSuspended, RunningTime = processRunningTime };
                            await ListBoxAddItem(lb_Processes, List_Processes, dataBindApp, false, false);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Failed adding Win32 or Win32Store application: " + ex.Message);
                        }
                    }
                }
            }
            catch { }
        }
    }
}