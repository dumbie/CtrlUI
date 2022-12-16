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
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get all the active UWP processes and update the lists
        async Task ListLoadCheckProcessesUwp(List<int> activeProcessesId, List<IntPtr> activeProcessesWindow, IEnumerable<DataBindApp> currentListApps, bool showStatus)
        {
            try
            {
                //Show refresh status message
                if (showStatus) { await Notification_Send_Status("Refresh", "Refreshing store apps"); }
                //Debug.WriteLine("Checking store processes.");

                //Get active UWP processes
                List<ProcessMulti> processesList = GetUwpAppProcesses();

                //Add new running process if needed
                foreach (ProcessMulti processMultiApp in processesList)
                {
                    try
                    {
                        //Check if application title is blacklisted
                        if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == processMultiApp.WindowTitle.ToLower()))
                        {
                            continue;
                        }

                        //Get application executable path
                        string processPathExe = processMultiApp.Path;
                        string processPathExeLower = processPathExe.ToLower();

                        //Get application executable name
                        string processNameExe = processMultiApp.AppxDetails.ExecutableAliasName;
                        string processNameExeLower = processNameExe.ToLower();
                        string processNameExeNoExt = Path.GetFileNameWithoutExtension(processNameExe);
                        string processNameExeNoExtLower = processNameExeNoExt.ToLower();

                        //Check if application name is blacklisted
                        if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == processNameExeNoExtLower))
                        {
                            continue;
                        }

                        //Add active process to the list
                        activeProcessesId.Add(processMultiApp.Identifier);
                        activeProcessesWindow.Add(processMultiApp.WindowHandle);

                        //Check the process running time
                        int processRunningTime = (int)processMultiApp.ProcessRuntime().TotalMinutes;

                        //Check if process is a Windows Store app
                        Visibility processStatusStore = Visibility.Visible;

                        //Check if the process is suspended
                        Visibility processStatusRunning = Visibility.Visible;
                        Visibility processStatusSuspended = Visibility.Collapsed;
                        ProcessThreadCollection processThreads = processMultiApp.ProcessThreads();
                        if (CheckProcessSuspended(processThreads))
                        {
                            processStatusRunning = Visibility.Collapsed;
                            processStatusSuspended = Visibility.Visible;
                        }

                        //Set the application search filters
                        Func<DataBindApp, bool> filterCombinedApp = x => !string.IsNullOrWhiteSpace(x.PathExe) && x.PathExe.ToLower() == processPathExeLower;
                        Func<DataBindApp, bool> filterProcessApp = x => x.ProcessMulti.Any(z => z.WindowHandle == processMultiApp.WindowHandle);

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
                            existingCombinedApp.RunningTimeLastUpdate = GetSystemTicksMs();

                            //Add the new process multi application
                            if (!existingCombinedApp.ProcessMulti.Any(x => x.WindowHandle == processMultiApp.WindowHandle))
                            {
                                existingCombinedApp.ProcessMulti.Add(processMultiApp);
                            }
                        }

                        //Check if process is in process list and update it
                        foreach (DataBindApp existingProcessApp in existingProcessApps)
                        {
                            //Update the process title
                            if (existingProcessApp.Name != processMultiApp.WindowTitle) { existingProcessApp.Name = processMultiApp.WindowTitle; }

                            //Update the process running time
                            existingProcessApp.RunningTime = processRunningTime;

                            //Update the process suspended status
                            existingProcessApp.StatusSuspended = processStatusSuspended;

                            //Update the process multi identifier
                            foreach (ProcessMulti processMulti in existingProcessApp.ProcessMulti.Where(x => x.Identifier <= 0))
                            {
                                processMulti.Identifier = processMultiApp.Identifier;
                            }

                            //Update the process multi window handle
                            foreach (ProcessMulti processMulti in existingProcessApp.ProcessMulti.Where(x => x.WindowHandle == IntPtr.Zero))
                            {
                                processMulti.WindowHandle = processMultiApp.WindowHandle;
                            }

                            appUpdatedContinueLoop = true;
                        }

                        //Check if application updated
                        if (appUpdatedContinueLoop)
                        {
                            continue;
                        }

                        //Load the application image
                        BitmapImage processImageBitmap = FileToBitmapImage(new string[] { processMultiApp.WindowTitle, processNameExeNoExt, processMultiApp.AppxDetails.SquareLargestLogoPath, processMultiApp.AppxDetails.WideLargestLogoPath }, vImageSourceFolders, vImageBackupSource, processMultiApp.WindowHandle, vImageLoadSize, 0);

                        //Create new ProcessMulti list
                        List<ProcessMulti> listProcessMulti = new List<ProcessMulti>();
                        listProcessMulti.Add(processMultiApp);

                        //Add the process to the process list
                        DataBindApp dataBindApp = new DataBindApp() { Type = processMultiApp.Type, Category = AppCategory.Process, ProcessMulti = listProcessMulti, ImageBitmap = processImageBitmap, Name = processMultiApp.WindowTitle, NameExe = processNameExe, PathExe = processPathExe, StatusStore = processStatusStore, StatusSuspended = processStatusSuspended, RunningTime = processRunningTime };
                        await ListBoxAddItem(lb_Processes, List_Processes, dataBindApp, false, false);

                        //Add the process to the search list
                        await AddSearchProcess(dataBindApp);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed adding UWP process: " + ex.Message);
                    }
                }
            }
            catch { }
        }
    }
}