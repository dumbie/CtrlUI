using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Combine all the saved lists to make comparison
        IEnumerable<DataBindApp> CombineAppLists(bool IncludeShortcuts, bool IncludeProcesses)
        {
            try
            {
                IEnumerable<DataBindApp> CombinedApps = List_Apps.Concat(List_Games).Concat(List_Emulators);
                if (IncludeShortcuts) { CombinedApps = CombinedApps.Concat(List_Shortcuts); }
                if (IncludeProcesses) { CombinedApps = CombinedApps.Concat(List_Processes); }
                return CombinedApps;
            }
            catch
            {
                return null;
            }
        }

        //Wait for application lists refresh
        async Task RefreshApplicationListsWait()
        {
            try
            {
                if (vBusyRefreshingApps)
                {
                    Debug.WriteLine("Applications are already refreshing, waiting.");
                    while (vBusyRefreshingApps)
                    {
                        await Task.Delay(100);
                    }
                }
            }
            catch { }
        }

        //Refresh the application lists
        async Task RefreshApplicationLists(bool skipShortcutLoading, bool skipProcessLoading, bool skipRunningStatus, bool skipListStats, bool refreshWait, bool showStatus, bool playSound)
        {
            try
            {
                //Check if applications are refreshing
                if (vBusyRefreshingApps)
                {
                    if (refreshWait)
                    {
                        await RefreshApplicationListsWait();
                    }
                    else
                    {
                        Debug.WriteLine("Applications are already refreshing, cancelling.");
                        return;
                    }
                }

                //Update the refreshing status
                vBusyRefreshingApps = true;

                //Show refresh status message
                if (showStatus)
                {
                    Popup_Show_Status("Refresh", "Refreshing applications");
                }

                //Play application refresh sound
                if (playSound)
                {
                    PlayInterfaceSound(vInterfaceSoundVolume, "Refresh", false);
                }

                //Load all the active processes
                IEnumerable<Process> processesList = null;
                if (!skipProcessLoading || !skipRunningStatus)
                {
                    processesList = Process.GetProcesses();

                    if (!skipProcessLoading)
                    {
                        await UpdateApplicationLists(processesList);
                    }

                    if (!skipRunningStatus)
                    {
                        CheckAppRunningStatus(processesList);
                    }
                }

                //Load all the shortcuts
                if (!skipShortcutLoading)
                {
                    await ListLoadShortcuts(false);
                }

                //Refresh the application list stats
                if (!skipListStats)
                {
                    ShowHideEmptyList(true, true);
                    ListsUpdateCount();
                    UpdateSearchResults();
                }
            }
            catch { }
            vBusyRefreshingApps = false;
        }

        async Task UpdateApplicationLists(IEnumerable<Process> processesList)
        {
            try
            {
                //Check if processes list is provided
                if (processesList == null)
                {
                    processesList = Process.GetProcesses();
                }

                //List all the currently running processes
                List<int> activeProcessesId = new List<int>();
                List<IntPtr> activeProcessesWindow = new List<IntPtr>();

                //Get the currently running processes
                IEnumerable<DataBindApp> currentListApps = CombineAppLists(true, false).Where(x => x.StatusLauncher == Visibility.Collapsed);

                //Update all the processes
                await ListLoadCheckProcessesUwp(activeProcessesId, activeProcessesWindow, currentListApps, false);
                await ListLoadCheckProcessesWin32(processesList, activeProcessesId, activeProcessesWindow, currentListApps, false);

                //Update the application running count and status
                foreach (DataBindApp dataBindApp in currentListApps)
                {
                    try
                    {
                        //Remove closed processes
                        dataBindApp.ProcessMulti.RemoveAll(x => !activeProcessesId.Contains(x.Identifier));
                        dataBindApp.ProcessMulti.RemoveAll(x => !activeProcessesWindow.Contains(x.WindowHandle));

                        //Check the running count
                        int processCount = dataBindApp.ProcessMulti.Count();
                        if (processCount > 1)
                        {
                            //Remove invalid processes
                            dataBindApp.ProcessMulti.RemoveAll(x => x.WindowHandle == IntPtr.Zero);
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
                            dataBindApp.StatusRunning = Visibility.Collapsed;
                            dataBindApp.StatusSuspended = Visibility.Collapsed;
                            dataBindApp.RunningProcessCount = string.Empty;
                            dataBindApp.RunningTimeLastUpdate = 0;
                            dataBindApp.ProcessMulti.Clear();
                        }
                    }
                    catch { }
                }

                //Remove no longer running and invalid processes
                await ListBoxRemoveAll(lb_Processes, List_Processes, x => !x.ProcessMulti.Any() || x.ProcessMulti.Any(z => !activeProcessesWindow.Contains(z.WindowHandle)) || x.ProcessMulti.Any(z => z.WindowHandle == IntPtr.Zero));
            }
            catch { }
        }

        //Check for empty lists and hide them
        void ShowHideEmptyList(bool includeShortcuts, bool includeProcesses)
        {
            try
            {
                UpdateElementVisibility(sp_Games, List_Games.Any());
                UpdateElementVisibility(sp_Apps, List_Apps.Any());
                UpdateElementVisibility(sp_Emulators, List_Emulators.Any());
                if (includeShortcuts)
                {
                    UpdateElementVisibility(sp_Shortcuts, List_Shortcuts.Any() && ConfigurationManager.AppSettings["ShowOtherShortcuts"] == "True");
                }
                if (includeProcesses)
                {
                    UpdateElementVisibility(sp_Processes, List_Processes.Any() && ConfigurationManager.AppSettings["ShowOtherProcesses"] == "True");
                }
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
                foreach (DataBindApp dataBindApp in CombineAppLists(false, false).Where(x => x.RunningTimeLastUpdate != 0))
                {
                    try
                    {
                        if (dataBindApp.StatusRunning == Visibility.Visible && (Environment.TickCount - dataBindApp.RunningTimeLastUpdate) >= 60000)
                        {
                            ApplicationUpdated = true;
                            dataBindApp.RunningTime++;
                            dataBindApp.RunningTimeLastUpdate = Environment.TickCount;
                            //Debug.WriteLine(UpdateApp.Name + " has been running for one minute, total: " + UpdateApp.RunningTime);
                        }
                    }
                    catch { }
                }

                //Save changes to Json file
                if (ApplicationUpdated)
                {
                    JsonSaveApps();
                }
            }
            catch { }
        }

        //Update the list items count
        void ListsUpdateCount()
        {
            try
            {
                //Debug.WriteLine("Updating the lists count.");

                string List_Games_Count = List_Games.Count.ToString();
                string List_Apps_Count = List_Apps.Count.ToString();
                string List_Emulators_Count = List_Emulators.Count.ToString();
                string List_Shortcuts_Count = List_Shortcuts.Count.ToString();
                string List_Processes_Count = List_Processes.Count.ToString();

                AVActions.ActionDispatcherInvoke(delegate
                {
                    tb_Games_Count.Text = List_Games_Count;
                    tb_Apps_Count.Text = List_Apps_Count;
                    tb_Emulators_Count.Text = List_Emulators_Count;
                    tb_Shortcuts_Count.Text = List_Shortcuts_Count;
                    tb_Processes_Count.Text = List_Processes_Count;
                });
            }
            catch { }
        }
    }
}