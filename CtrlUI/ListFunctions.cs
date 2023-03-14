using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

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

        //Refresh processes and status
        async Task RefreshProcesses()
        {
            try
            {
                //Check if already refreshing
                if (vBusyRefreshingProcesses)
                {
                    Debug.WriteLine("Processes are refreshing, cancelling.");
                    return;
                }

                //Update the refreshing status
                vBusyRefreshingProcesses = true;

                //Get all running processes
                List<ProcessMulti> processMultiList = AVProcess.Get_AllProcessesMulti();

                //Refresh the processes list
                await RefreshProcessLists(processMultiList);

                //Check app running status
                CheckAppRunningStatus(processMultiList);

                //Update list load status
                vListLoadedProcesses = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed refreshing processes: " + ex.Message);
            }
            finally
            {
                //Update the refreshing status
                vBusyRefreshingProcesses = false;
            }
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

        //Refresh process lists
        async Task RefreshProcessLists(List<ProcessMulti> processMultiList)
        {
            try
            {
                //Get all running processes
                if (processMultiList == null)
                {
                    processMultiList = AVProcess.Get_AllProcessesMulti();
                }

                //List of valid process window handles
                List<IntPtr> validProcessesWindow = new List<IntPtr>();

                //Get the currently running processes
                IEnumerable<DataBindApp> combinedAppLists = CombineAppLists(true, false, false).Where(x => x.StatusUrlProtocol == Visibility.Collapsed);

                //Update all the processes
                await ProcessListUpdate(processMultiList, combinedAppLists, validProcessesWindow);

                //Cleanup all the processes
                await ProcessListCleanupList(validProcessesWindow);
                ProcessListCleanupCombined(processMultiList, combinedAppLists);
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
                bool applicationUpdated = false;
                //Debug.WriteLine("Updating the application running time.");
                foreach (DataBindApp dataBindApp in CombineAppLists(false, false, false).Where(x => x.ProcessMulti.Any()))
                {
                    try
                    {
                        applicationUpdated = true;
                        if (dataBindApp.RunningTime < 0)
                        {
                            dataBindApp.RunningTime = 1;
                        }
                        else
                        {
                            dataBindApp.RunningTime++;
                        }
                        //Debug.WriteLine(dataBindApp.Name + " has been running for one minute, total: " + dataBindApp.RunningTime);
                    }
                    catch { }
                }

                //Save changes to Json file
                if (applicationUpdated)
                {
                    JsonSaveApplications();
                }
            }
            catch { }
        }
    }
}