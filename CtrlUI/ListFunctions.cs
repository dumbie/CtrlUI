using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppBusyWait;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Combine all the saved lists to make comparison
        IEnumerable<DataBindApp> CombineAppLists(bool includeShortcuts, bool includeProcesses, bool includeLaunchers)
        {
            try
            {
                IEnumerable<DataBindApp> combinedLists = List_Apps.ToList();
                combinedLists = combinedLists.Concat(List_Games.ToList());
                combinedLists = combinedLists.Concat(List_Emulators.ToList());
                if (includeShortcuts) { combinedLists = combinedLists.Concat(List_Shortcuts.ToList()); }
                if (includeProcesses) { combinedLists = combinedLists.Concat(List_Processes.ToList()); }
                if (includeLaunchers) { combinedLists = combinedLists.Concat(List_Launchers.ToList()); }
                return combinedLists;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed combining application lists: " + ex.Message);
                return null;
            }
        }

        //Refresh the processes and status
        async Task RefreshListProcessesWithWait(bool refreshWait)
        {
            try
            {
                //Check if already refreshing
                if (await WaitForBusyBoolCancel(() => vBusyRefreshingProcesses, refreshWait))
                {
                    Debug.WriteLine("Processes are already refreshing, cancelling.");
                    return;
                }

                //Update the refreshing status
                vBusyRefreshingProcesses = true;

                //Get all running processes
                List<ProcessMulti> processesList = AVProcess.Get_AllProcessesMulti();

                //Refresh the processes list
                await RefreshListProcesses(processesList);

                //Check app running status
                CheckAppRunningStatus(processesList);

                //Update list load status
                vListLoadedProcesses = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed refreshing processes: " + ex.Message);
            }
            //Update the refreshing status
            vBusyRefreshingProcesses = false;
        }

        //Refresh the list status
        async Task RefreshListStatus()
        {
            try
            {
                //Check if all categories loaded
                if (!vAppsLoaded())
                {
                    Debug.WriteLine("All applications not yet loaded, skipping list status update.");
                    return;
                }

                ShowHideEmptyList();
                UpdateSearchResults();
                await CategoryListCheckActive();
                CategoryListUpdateCount();
            }
            catch { }
        }

        //Refresh the processes list
        async Task RefreshListProcesses(List<ProcessMulti> processesList)
        {
            try
            {
                //Get all running processes
                if (processesList == null)
                {
                    processesList = AVProcess.Get_AllProcessesMulti();
                }

                //List all the currently running processes
                List<int> activeProcessesId = new List<int>();
                List<IntPtr> activeProcessesWindow = new List<IntPtr>();

                //Get the currently running processes
                IEnumerable<DataBindApp> currentListApps = CombineAppLists(true, false, false).Where(x => x.StatusUrlProtocol == Visibility.Collapsed);

                //Update all the processes
                await ListLoadCheckProcesses(processesList, activeProcessesId, activeProcessesWindow, currentListApps, false);

                //Update the application running count and status
                foreach (DataBindApp dataBindApp in currentListApps)
                {
                    try
                    {
                        //Remove closed processes
                        dataBindApp.ProcessMulti.RemoveAll(x => !activeProcessesId.Contains(x.Identifier));
                        dataBindApp.ProcessMulti.RemoveAll(x => !activeProcessesWindow.Contains(x.WindowHandleMain));

                        //Check the running count
                        int processCount = dataBindApp.ProcessMulti.Count();
                        if (processCount > 1)
                        {
                            //Remove invalid processes
                            dataBindApp.ProcessMulti.RemoveAll(x => x.WindowHandleMain == IntPtr.Zero);
                            processCount = dataBindApp.ProcessMulti.Count();
                        }

                        //Update the running count
                        if (processCount > 1)
                        {
                            dataBindApp.RunningProcessCount = Convert.ToString(processCount);
                        }
                        else
                        {
                            dataBindApp.RunningProcessCount = string.Empty;
                        }

                        //Update the running status
                        if (processCount == 0)
                        {
                            dataBindApp.ResetStatus();
                        }
                    }
                    catch { }
                }

                //Remove no longer running and invalid processes
                Func<DataBindApp, bool> filterProcessApp = x => x.Category == AppCategory.Process && (!x.ProcessMulti.Any() || x.ProcessMulti.Any(z => !activeProcessesWindow.Contains(z.WindowHandleMain)) || x.ProcessMulti.Any(z => z.WindowHandleMain == IntPtr.Zero));
                await ListBoxRemoveAll(lb_Processes, List_Processes, filterProcessApp);
                await ListBoxRemoveAll(lb_Search, List_Search, filterProcessApp);
            }
            catch { }
        }

        //Check for empty lists and hide them
        void ShowHideEmptyList()
        {
            try
            {
                Visibility visibilityGames = List_Games.Any() ? Visibility.Visible : Visibility.Collapsed;
                Visibility visibilityApps = List_Apps.Any() ? Visibility.Visible : Visibility.Collapsed;
                Visibility visibilityEmulators = List_Emulators.Any() ? Visibility.Visible : Visibility.Collapsed;
                Visibility visibilityLaunchers = List_Launchers.Any() ? Visibility.Visible : Visibility.Collapsed;
                Visibility visibilityShortcuts = List_Shortcuts.Any() ? Visibility.Visible : Visibility.Collapsed;
                Visibility visibilityProcesses = List_Processes.Any() ? Visibility.Visible : Visibility.Collapsed;

                AVActions.DispatcherInvoke(delegate
                {
                    button_Category_Menu_Games.Visibility = visibilityGames;
                    button_Category_Menu_Apps.Visibility = visibilityApps;
                    button_Category_Menu_Emulators.Visibility = visibilityEmulators;
                    button_Category_Menu_Launchers.Visibility = visibilityLaunchers;
                    button_Category_Menu_Shortcuts.Visibility = visibilityShortcuts;
                    button_Category_Menu_Processes.Visibility = visibilityProcesses;
                });
            }
            catch { }
        }

        //Update the app running rime
        void UpdateAppRunningTime()
        {
            try
            {
                bool ApplicationUpdated = false;
                //Debug.WriteLine("Updating the application running time.");
                foreach (DataBindApp dataBindApp in CombineAppLists(false, false, false).Where(x => x.RunningTimeLastUpdate != 0))
                {
                    try
                    {
                        if (dataBindApp.StatusRunning == Visibility.Visible && (GetSystemTicksMs() - dataBindApp.RunningTimeLastUpdate) >= 60000)
                        {
                            ApplicationUpdated = true;
                            dataBindApp.RunningTime++;
                            dataBindApp.RunningTimeLastUpdate = GetSystemTicksMs();
                            //Debug.WriteLine(UpdateApp.Name + " has been running for one minute, total: " + UpdateApp.RunningTime);
                        }
                    }
                    catch { }
                }

                //Save changes to Json file
                if (ApplicationUpdated)
                {
                    JsonSaveApplications();
                }
            }
            catch { }
        }
    }
}