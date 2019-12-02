using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                        Debug.WriteLine("Applications are already refreshing, waiting.");
                        while (vBusyRefreshingApps)
                        {
                            await Task.Delay(100);
                        }
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

                //Load all the shortcuts
                if (!skipShortcutLoading)
                {
                    await ListLoadShortcuts(false);
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
                List<IntPtr> activeProcessesWindow = new List<IntPtr>();

                //Get the currently running processes
                IEnumerable<DataBindApp> currentListApps = CombineAppLists(true, false).Where(x => x.StatusLauncher == Visibility.Collapsed);

                //Update all the processes
                await ListLoadCheckProcessesUwp(activeProcessesWindow, currentListApps, false);
                await ListLoadCheckProcessesWin32(processesList, activeProcessesWindow, currentListApps, false);

                //Update the application running count and status
                foreach (DataBindApp dataBindApp in currentListApps)
                {
                    try
                    {
                        //Remove closed processes
                        dataBindApp.ProcessMulti.RemoveAll(x => !activeProcessesWindow.Contains(x.WindowHandle));

                        //Update the running count
                        int processCount = dataBindApp.ProcessMulti.Count();
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
                            dataBindApp.RunningTimeLastUpdate = 0;
                        }
                    }
                    catch { }
                }

                //Remove no longer running and invalid processes
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    await ListBoxRemoveAll(lb_Processes, List_Processes, x => x.ProcessMulti.Any(z => !activeProcessesWindow.Contains(z.WindowHandle)));
                });
            }
            catch { }
        }

        //Bind the lists to the listbox elements
        void ListBoxBindLists()
        {
            try
            {
                lb_Games.IsTextSearchEnabled = true;
                lb_Games.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Games, "Name");
                lb_Games.ItemsSource = List_Games;

                lb_Apps.IsTextSearchEnabled = true;
                lb_Apps.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Apps, "Name");
                lb_Apps.ItemsSource = List_Apps;

                lb_Emulators.IsTextSearchEnabled = true;
                lb_Emulators.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Emulators, "Name");
                lb_Emulators.ItemsSource = List_Emulators;

                lb_Shortcuts.IsTextSearchEnabled = true;
                lb_Shortcuts.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Shortcuts, "Name");
                lb_Shortcuts.ItemsSource = List_Shortcuts;

                lb_Processes.IsTextSearchEnabled = true;
                lb_Processes.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Processes, "Name");
                lb_Processes.ItemsSource = List_Processes;

                lb_ColorPicker.ItemsSource = List_ColorPicker;

                lb_Search.IsTextSearchEnabled = true;
                lb_Search.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Search, "Name");
                lb_Search.ItemsSource = List_Search;

                lb_FilePicker.IsTextSearchEnabled = true;
                lb_FilePicker.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_FilePicker, "Name");
                lb_FilePicker.ItemsSource = List_FilePicker;
            }
            catch { }
        }

        //Select the first listbox item
        void ListBoxSelectIndexes()
        {
            try
            {
                lb_Search.SelectedIndex = 0;
                lb_Games.SelectedIndex = 0;
                lb_Apps.SelectedIndex = 0;
                lb_Emulators.SelectedIndex = 0;
                lb_Shortcuts.SelectedIndex = 0;
                lb_Processes.SelectedIndex = 0;
                lb_FilePicker.SelectedIndex = 0;
            }
            catch { }
        }

        //Check for empty lists and hide them
        void ShowHideEmptyList(bool IncludeShortcuts, bool IncludeProcesses)
        {
            try
            {
                UpdateElementVisibility(sp_Games, List_Games.Any());
                UpdateElementVisibility(sp_Apps, List_Apps.Any());
                UpdateElementVisibility(sp_Emulators, List_Emulators.Any());
                if (IncludeShortcuts)
                {
                    UpdateElementVisibility(sp_Shortcuts, List_Shortcuts.Any() && ConfigurationManager.AppSettings["ShowOtherShortcuts"] == "True");
                }
                if (IncludeProcesses)
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