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
                            string ProcessExecutablePathLower = Path.GetFileNameWithoutExtension(ProcessExecutablePath.ToLower());

                            //Check if already in combined list and remove it
                            if (ConfigurationManager.AppSettings["HideAppProcesses"] == "True")
                            {
                                if (CurrentApps.Any(x => Path.GetFileNameWithoutExtension(x.PathExe.ToLower()) == ProcessExecutablePathLower))
                                {
                                    //Debug.WriteLine("Process is already in other list: " + ProcessExecutablePathLowerName);
                                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                                    {
                                        await ListBoxRemoveAll(lb_Processes, List_Processes, x => Path.GetFileNameWithoutExtension(x.PathExe.ToLower()) == ProcessExecutablePathLower);
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

                            //Load the application image
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