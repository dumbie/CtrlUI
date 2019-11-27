﻿using ArnoldVinkCode;
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

                    List<int> activeProcesses = new List<int>();
                    IEnumerable<DataBindApp> currentListApps = CombineAppLists(true, false);

                    //Add new running process if needed
                    foreach (Process allProcess in ProcessesList)
                    {
                        try
                        {
                            //Process application type
                            ProcessType applicationType = ProcessType.Win32;

                            //Get the process title
                            string processTitle = GetWindowTitleFromProcess(allProcess);

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

                            //Validate the process state
                            if (!ValidateProcessState(allProcess, true, true))
                            {
                                continue;
                            }

                            //Validate the window handle
                            if (!ValidateWindowHandle(allProcess.MainWindowHandle))
                            {
                                continue;
                            }

                            //Get the executable path
                            string processExecutablePath = GetExecutablePathFromProcess(allProcess);
                            string processExecutablePathLower = Path.GetFileNameWithoutExtension(processExecutablePath.ToLower());

                            //Check if already in combined list and remove it
                            if (ConfigurationManager.AppSettings["HideAppProcesses"] == "True")
                            {
                                if (currentListApps.Any(x => Path.GetFileNameWithoutExtension(x.PathExe.ToLower()) == processExecutablePathLower))
                                {
                                    //Debug.WriteLine("Process is already in other list: " + ProcessExecutablePathLowerName);
                                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                                    {
                                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => Path.GetFileNameWithoutExtension(x.PathExe.ToLower()) == processExecutablePathLower);
                                    });
                                    continue;
                                }
                            }

                            //Add active process
                            activeProcesses.Add(allProcess.Id);

                            //Check the process running time
                            int processRunningTime = ProcessRuntimeMinutes(allProcess);

                            //Check if the process is suspended
                            Visibility suspendedStatus = Visibility.Collapsed;
                            if (CheckProcessSuspended(allProcess.Threads))
                            {
                                suspendedStatus = Visibility.Visible;
                            }

                            //Check if process is already in process list and update it
                            DataBindApp processApp = List_Processes.Where(x => x.ProcessMulti.Identifier == allProcess.Id && (x.Type == ProcessType.Win32 || x.Type == ProcessType.Win32Store)).FirstOrDefault();
                            if (processApp != null)
                            {
                                if (processApp.Name != processTitle) { processApp.Name = processTitle; }
                                processApp.RunningTime = processRunningTime;
                                processApp.StatusSuspended = suspendedStatus;
                                continue;
                            }

                            //Load the application image
                            BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { processTitle, allProcess.ProcessName, processExecutablePath }, allProcess.MainWindowHandle, 90);

                            //Get process launch arguments
                            string processArgument = GetLaunchArgumentsFromProcess(allProcess, processExecutablePath);

                            //Check if the process is a Win32Store app
                            Visibility storeStatus = Visibility.Collapsed;
                            string executableName = string.Empty;
                            string appUserModelId = GetAppUserModelIdFromProcess(allProcess);
                            if (!string.IsNullOrWhiteSpace(appUserModelId))
                            {
                                executableName = Path.GetFileName(processExecutablePath);
                                processExecutablePath = appUserModelId;
                                storeStatus = Visibility.Visible;
                                applicationType = ProcessType.Win32Store;
                                //Debug.WriteLine("Process " + ProcessTitle + " is a Win32Store application.");
                            }

                            //Convert Process To ProcessMulti
                            ProcessMulti processMultiNew = ConvertProcessToProcessMulti(applicationType, allProcess);

                            //Add the process to the list
                            AVActions.ActionDispatcherInvoke(delegate
                            {
                                List_Processes.Add(new DataBindApp() { Type = applicationType, Category = AppCategory.Process, ProcessMulti = processMultiNew, ImageBitmap = iconBitmapImage, Name = processTitle, NameExe = executableName, PathExe = processExecutablePath, Argument = processArgument, StatusStore = storeStatus, StatusSuspended = suspendedStatus, RunningTime = processRunningTime });
                            });
                        }
                        catch { }
                    }

                    //Remove no longer running and invalid processes
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => !activeProcesses.Contains(x.ProcessMulti.Identifier) && (x.Type == ProcessType.Win32 || x.Type == ProcessType.Win32Store));
                    });
                }
            }
            catch { }
        }
    }
}