using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get all active processes and update lists
        async Task ProcessListUpdate(List<ProcessMulti> processMultiList, List<IntPtr> processWindowHandles, IEnumerable<DataBindApp> combinedAppLists)
        {
            try
            {
                //Debug.WriteLine("Refreshing and updating processes.");

                //Get all running processes
                if (processMultiList == null)
                {
                    processMultiList = Get_AllProcessesMulti();
                }

                //Add new running process if needed
                foreach (ProcessMulti processMulti in processMultiList)
                {
                    try
                    {
                        //Check if the process is valid
                        if (!processMulti.Validate())
                        {
                            //Debug.WriteLine("Process has invalid paths: " + processMulti.Identifier);
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

                        //Check process running time
                        int processRunTime = (int)processMulti.RunTime.TotalMinutes;

                        //Check if process is in combined list and update it
                        Func<DataBindApp, bool> filterCombinedApp = x => (!string.IsNullOrWhiteSpace(x.PathExe) && x.PathExe.ToLower() == processPathExeLower) || (!string.IsNullOrWhiteSpace(x.PathExe) && Path.GetFileNameWithoutExtension(x.PathExe).ToLower() == processNameExeNoExtLower) || (!string.IsNullOrWhiteSpace(x.NameExe) && x.NameExe.ToLower() == processNameExeLower) || (!string.IsNullOrWhiteSpace(x.AppUserModelId) && x.AppUserModelId.ToLower() == processAppUserModelIdLower);
                        foreach (DataBindApp existingCombinedApp in combinedAppLists.Where(filterCombinedApp))
                        {
                            //Update the process running time
                            existingCombinedApp.StatusProcessRunTime = processRunTime;

                            //Update the process running status
                            existingCombinedApp.StatusRunning = processStatusRunning;

                            //Update the process suspended status
                            existingCombinedApp.StatusSuspended = processStatusSuspended;

                            //Update the process not responding status
                            existingCombinedApp.StatusNotResponding = processStatusNotResponding;

                            //Add new process multi application
                            if (!existingCombinedApp.ProcessMulti.Any(x => x.Identifier == processMulti.Identifier))
                            {
                                existingCombinedApp.ProcessMulti.Add(processMulti);
                            }
                        }

                        //Check if application name is blacklisted
                        if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == processNameExeNoExtLower))
                        {
                            continue;
                        }

                        //Get application main window handle
                        IntPtr windowHandleMain = processMulti.WindowHandleMain;

                        //Check if application has valid main window
                        if (windowHandleMain == IntPtr.Zero)
                        {
                            continue;
                        }

                        //Check if uwp application has foreground window
                        if (processMulti.Type == ProcessType.UWP)
                        {
                            if (!Check_ProcessIsForegroundUwpApp(processMulti.Identifier))
                            {
                                continue;
                            }
                        }

                        //Add valid window handle to list
                        processWindowHandles.Add(windowHandleMain);

                        //Check process name for correction
                        ProcessNameCorrection(processMulti, processNameExeLower);

                        //Check if process is in process list and update it
                        bool appUpdatedContinueLoop = false;
                        Func<DataBindApp, bool> filterProcessApp = x => x.ProcessMulti.Any(z => z.Identifier == processMulti.Identifier);
                        IEnumerable<DataBindApp> existingProcessApps = List_Processes.Where(filterProcessApp);
                        foreach (DataBindApp existingProcessApp in existingProcessApps)
                        {
                            //Update the process title
                            if (existingProcessApp.Name != processMulti.WindowTitleMain)
                            {
                                existingProcessApp.Name = processMulti.WindowTitleMain;
                            }

                            //Update the process running time
                            existingProcessApp.StatusProcessRunTime = processRunTime;

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

                        //Check if process is a Windows Store app
                        string storeImageSquare = string.Empty;
                        string storeImageWide = string.Empty;
                        Visibility processStatusStore = Visibility.Collapsed;
                        if (processMulti.Type == ProcessType.UWP || processMulti.Type == ProcessType.Win32Store)
                        {
                            try
                            {
                                processStatusStore = Visibility.Visible;
                                storeImageSquare = processMulti.AppxDetails.SquareLargestLogoPath;
                                storeImageWide = processMulti.AppxDetails.WideLargestLogoPath;
                            }
                            catch { }
                        }

                        //Load the application image
                        BitmapImage processImageBitmap = FileToBitmapImage(new string[] { processMulti.WindowTitleMain, processNameExeNoExt, storeImageSquare, storeImageWide, processPathExe }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, windowHandleMain, 0);

                        //Create new ProcessMulti list
                        List<ProcessMulti> listProcessMulti = new List<ProcessMulti>();
                        listProcessMulti.Add(processMulti);

                        //Add the process to the process list
                        DataBindApp dataBindApp = new DataBindApp() { Type = processMulti.Type, Category = AppCategory.Process, ProcessMulti = listProcessMulti, ImageBitmap = processImageBitmap, Name = processMulti.WindowTitleMain, AppUserModelId = processAppUserModelId, NameExe = processNameExe, PathExe = processPathExe, StatusStore = processStatusStore, StatusSuspended = processStatusSuspended, StatusNotResponding = processStatusNotResponding, StatusProcessRunTime = processRunTime };
                        await ListBoxAddItem(lb_Processes, List_Processes, dataBindApp, false, false);

                        //Add the process to the search list
                        await AddSearchProcess(dataBindApp);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed updating process list: " + ex.Message);
                    }
                }
            }
            catch { }
        }

        //Correct process window names
        public static void ProcessNameCorrection(ProcessMulti processMulti, string processNameExeLower)
        {
            try
            {
                if (processNameExeLower == "explorer.exe")
                {
                    processMulti.WindowTitleMain = "File Explorer";
                }
                else if (processNameExeLower == "msedge.exe")
                {
                    processMulti.WindowTitleMain = "Microsoft Edge";
                }
            }
            catch { }
        }
    }
}