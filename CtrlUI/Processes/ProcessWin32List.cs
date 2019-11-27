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
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get all the active Win32 processes and update the lists
        async Task ListLoadCheckProcessesWin32(IEnumerable<Process> processesList, bool showStatus)
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

                    List<int> activeProcessesId = new List<int>();
                    List<string> activeProcessesPathExe = new List<string>();
                    IEnumerable<DataBindApp> currentListApps = CombineAppLists(true, false);
                    currentListApps = currentListApps.Where(x => x.StatusLauncher == Visibility.Collapsed);
                    currentListApps = currentListApps.Where(x => x.Type == ProcessType.Win32 || x.Type == ProcessType.Win32Store);

                    //Add new running process if needed
                    foreach (Process processApp in processesList)
                    {
                        try
                        {
                            //Process application type
                            ProcessType processType = ProcessType.Win32;

                            //Process Windows Store Status
                            Visibility processStatusStore = Visibility.Collapsed;

                            //Get the process title
                            string processTitle = GetWindowTitleFromProcess(processApp);

                            //Check if application title is blacklisted
                            if (vAppsBlacklistProcess.Any(x => x.ToLower() == processTitle.ToLower()))
                            {
                                continue;
                            }

                            //Validate the process state
                            if (!ValidateProcessState(processApp, true, true))
                            {
                                continue;
                            }

                            //Validate the window handle
                            if (!ValidateWindowHandle(processApp.MainWindowHandle))
                            {
                                continue;
                            }

                            //Get the executable path
                            string processExecutablePath = GetExecutablePathFromProcess(processApp);
                            string processName = Path.GetFileNameWithoutExtension(processExecutablePath);
                            string processNameLower = processName.ToLower();

                            //Check if application process is blacklisted
                            if (vAppsBlacklistProcess.Any(x => x.ToLower() == processNameLower))
                            {
                                continue;
                            }

                            //Add active process
                            activeProcessesId.Add(processApp.Id);
                            activeProcessesPathExe.Add(processNameLower);

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

                            //Check the process launch arguments
                            string processArgument = GetLaunchArgumentsFromProcess(processApp, processExecutablePath);

                            //Check all the lists for the application
                            DataBindApp existingCombinedApp = currentListApps.Where(x => Path.GetFileNameWithoutExtension(x.PathExe.ToLower()) == processNameLower).FirstOrDefault();
                            DataBindApp existingProcessApp = List_Processes.Where(x => x.ProcessMulti.Identifier == processApp.Id).FirstOrDefault();

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
                                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => Path.GetFileNameWithoutExtension(x.PathExe.ToLower()) == processNameLower);
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
                            BitmapImage processImage = FileToBitmapImage(new string[] { processTitle, processName, processExecutablePath }, processApp.MainWindowHandle, 90);

                            //Check if the process is a Win32Store app
                            string processAppUserModelId = GetAppUserModelIdFromProcess(processApp);
                            if (!string.IsNullOrWhiteSpace(processAppUserModelId))
                            {
                                processName = Path.GetFileName(processExecutablePath);
                                processExecutablePath = processAppUserModelId;
                                processStatusStore = Visibility.Visible;
                                processType = ProcessType.Win32Store;
                                //Debug.WriteLine("Process " + processTitle + " is a Win32Store application.");
                            }

                            //Convert Process To ProcessMulti
                            ProcessMulti processMultiNew = ConvertProcessToProcessMulti(processType, processApp);

                            //Add the process to the list
                            AVActions.ActionDispatcherInvoke(delegate
                            {
                                List_Processes.Add(new DataBindApp() { Type = processType, Category = AppCategory.Process, ProcessMulti = processMultiNew, ImageBitmap = processImage, Name = processTitle, NameExe = processName, PathExe = processExecutablePath, Argument = processArgument, StatusStore = processStatusStore, StatusSuspended = processStatusSuspended, RunningTime = processRunningTime });
                            });
                        }
                        catch { }
                    }

                    //Update the application running count and status
                    foreach (DataBindApp listApp in currentListApps)
                    {
                        int processCount = activeProcessesPathExe.Count(x => x == Path.GetFileNameWithoutExtension(listApp.PathExe.ToLower()));
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
                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => !activeProcessesId.Contains(x.ProcessMulti.Identifier) && (x.Type == ProcessType.Win32 || x.Type == ProcessType.Win32Store));
                    });
                }
            }
            catch { }
        }
    }
}