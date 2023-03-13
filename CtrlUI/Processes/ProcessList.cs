using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get all the active processes and update the lists
        async Task ListLoadCheckProcesses(List<ProcessMulti> processesList, List<int> activeProcessesId, List<IntPtr> activeProcessesWindow, IEnumerable<DataBindApp> currentListApps, bool showStatus)
        {
            try
            {
                //Show refresh status message
                if (showStatus) { await Notification_Send_Status("Refresh", "Refreshing processes"); }
                //Debug.WriteLine("Refreshing and checking processes.");

                //Get all running processes
                if (processesList == null)
                {
                    processesList = Get_AllProcessesMulti();
                }

                //Add new running process if needed
                foreach (ProcessMulti processMulti in processesList)
                {
                    try
                    {
                        //Check if application has valid window
                        if (!Check_ValidWindowHandle(processMulti.WindowHandleMain))
                        {
                            continue;
                        }

                        //Check if application title is blacklisted
                        if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == processMulti.WindowTitle.ToLower()))
                        {
                            continue;
                        }

                        //Get application executable path
                        string processPathExe = processMulti.ExePath;
                        string processPathExeLower = processPathExe.ToLower();

                        //Get application executable name
                        string processNameExe = processMulti.ExeName;
                        string processNameExeLower = processNameExe.ToLower();
                        string processNameExeNoExt = processMulti.ExeNameNoExt;
                        string processNameExeNoExtLower = processNameExeNoExt.ToLower();

                        //Get application user model id
                        string processAppUserModelId = processMulti.AppUserModelId;
                        string processAppUserModelIdLower = processAppUserModelId.ToLower();

                        //Check if application name is blacklisted
                        if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == processNameExeNoExtLower))
                        {
                            continue;
                        }

                        //Check explorer process
                        ProcessNameCorrection(processMulti, processNameExeLower);

                        //Add active process to the list
                        activeProcessesId.Add(processMulti.Identifier);
                        activeProcessesWindow.Add(processMulti.WindowHandleMain);

                        //Check the process running time
                        int processRunningTime = (int)processMulti.RunTime.TotalMinutes;

                        //Check if process is a Windows Store app
                        Visibility processStatusStore = Visibility.Collapsed;
                        if (processMulti.Type == ProcessType.UWP || processMulti.Type == ProcessType.Win32Store)
                        {
                            processStatusStore = Visibility.Visible;
                        }

                        //Check process status
                        Visibility processStatusRunning = Visibility.Collapsed;
                        Visibility processStatusSuspended = Visibility.Collapsed;
                        Visibility processStatusNotResponding = Visibility.Collapsed;
                        if (!processMulti.Responding)
                        {
                            processStatusNotResponding = Visibility.Visible;
                        }
                        else if (processMulti.Suspended)
                        {
                            processStatusSuspended = Visibility.Visible;
                        }
                        else
                        {
                            processStatusRunning = Visibility.Visible;
                        }

                        //Set the application search filters
                        Func<DataBindApp, bool> filterCombinedApp = x => (!string.IsNullOrWhiteSpace(x.PathExe) && x.PathExe.ToLower() == processPathExeLower) || (!string.IsNullOrWhiteSpace(x.PathExe) && Path.GetFileNameWithoutExtension(x.PathExe).ToLower() == processNameExeNoExtLower) || (!string.IsNullOrWhiteSpace(x.NameExe) && x.NameExe.ToLower() == processNameExeLower);
                        Func<DataBindApp, bool> filterProcessApp = x => x.ProcessMulti.Any(z => z.WindowHandleMain == processMulti.WindowHandleMain);

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

                            //Update the process not responding status
                            existingCombinedApp.StatusNotResponding = processStatusNotResponding;

                            //Update the application last runtime
                            existingCombinedApp.RunningTimeLastUpdate = GetSystemTicksMs();

                            //Add the new process multi application
                            if (!existingCombinedApp.ProcessMulti.Any(x => x.WindowHandleMain == processMulti.WindowHandleMain))
                            {
                                existingCombinedApp.ProcessMulti.Add(processMulti);
                            }
                        }

                        //Check if process is in process list and update it
                        foreach (DataBindApp existingProcessApp in existingProcessApps)
                        {
                            //Update the process title
                            if (existingProcessApp.Name != processMulti.WindowTitle) { existingProcessApp.Name = processMulti.WindowTitle; }

                            //Update the process running time
                            existingProcessApp.RunningTime = processRunningTime;

                            //Update the process suspended status
                            existingProcessApp.StatusSuspended = processStatusSuspended;

                            //Update the process not responding status
                            existingProcessApp.StatusNotResponding = processStatusNotResponding;

                            appUpdatedContinueLoop = true;
                        }

                        //Check if application updated
                        if (appUpdatedContinueLoop)
                        {
                            continue;
                        }

                        //Load Windows store application images
                        string storeImageSquare = string.Empty;
                        string storeImageWide = string.Empty;
                        if (processMulti.Type == ProcessType.UWP || processMulti.Type == ProcessType.Win32Store)
                        {
                            try
                            {
                                storeImageSquare = processMulti.AppxDetails.SquareLargestLogoPath;
                                storeImageWide = processMulti.AppxDetails.WideLargestLogoPath;
                            }
                            catch { }
                        }

                        //Load the application image
                        BitmapImage processImageBitmap = FileToBitmapImage(new string[] { processMulti.WindowTitle, processNameExeNoExt, storeImageSquare, storeImageWide, processPathExe }, vImageSourceFolders, vImageBackupSource, processMulti.WindowHandleMain, vImageLoadSize, 0);

                        //Create new ProcessMulti list
                        List<ProcessMulti> listProcessMulti = new List<ProcessMulti>();
                        listProcessMulti.Add(processMulti);

                        //Add the process to the process list
                        DataBindApp dataBindApp = new DataBindApp() { Type = processMulti.Type, Category = AppCategory.Process, ProcessMulti = listProcessMulti, ImageBitmap = processImageBitmap, Name = processMulti.WindowTitle, AppUserModelId = processAppUserModelId, NameExe = processNameExe, PathExe = processPathExe, StatusStore = processStatusStore, StatusSuspended = processStatusSuspended, StatusNotResponding = processStatusNotResponding, RunningTime = processRunningTime };
                        await ListBoxAddItem(lb_Processes, List_Processes, dataBindApp, false, false);

                        //Add the process to the search list
                        await AddSearchProcess(dataBindApp);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed adding process: " + ex.Message);
                    }
                }
            }
            catch { }
        }

        //Correct process names
        public static void ProcessNameCorrection(ProcessMulti processMulti, string processNameExeLower)
        {
            try
            {
                if (processNameExeLower == "explorer.exe")
                {
                    processMulti.WindowTitle = "File Explorer";
                }
                else if (processNameExeLower == "msedge.exe")
                {
                    processMulti.WindowTitle = "Microsoft Edge";
                }
            }
            catch { }
        }
    }
}