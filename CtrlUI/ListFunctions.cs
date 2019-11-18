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
            catch { return null; }
        }

        //Refresh the application lists
        async Task RefreshApplicationLists(bool SkipListLoading, bool SkipRunningStatus, bool SkipListStats, bool ShowNotification, bool PlaySound)
        {
            try
            {
                //Check if applications are refreshing
                if (vBusyRefreshingApps)
                {
                    Debug.WriteLine("Applications are already refreshing.");
                    return;
                }
                else
                {
                    vBusyRefreshingApps = true;
                }

                if (ShowNotification) { Popup_Show_Status("Refresh", "Refreshing applications"); }
                if (PlaySound) { PlayInterfaceSound("Refresh", false); }

                //Load all the active processes
                IEnumerable<Process> ProcessesList = null;
                if (!SkipListLoading || !SkipRunningStatus)
                {
                    ProcessesList = Process.GetProcesses();

                    if (!SkipListLoading)
                    {
                        await ListLoadProcessesUwp(false);
                        await ListLoadProcessesWin32(ProcessesList, false);
                        await ListLoadShortcuts(false);
                    }

                    if (!SkipRunningStatus)
                    {
                        CheckAppRunningStatus(ProcessesList);
                    }
                }

                //Refresh the application list stats
                if (!SkipListStats)
                {
                    ShowHideEmptyList(true, true);
                    ListsUpdateCount();
                    UpdateSearchResults();
                }

                vBusyRefreshingApps = false;
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
                foreach (DataBindApp UpdateApp in CombineAppLists(false, false).Where(x => x.RunningTimeLastUpdate != 0))
                {
                    try
                    {
                        if (UpdateApp.StatusRunning == Visibility.Visible && (Environment.TickCount - UpdateApp.RunningTimeLastUpdate) >= 60000)
                        {
                            ApplicationUpdated = true;
                            UpdateApp.RunningTime++;
                            UpdateApp.RunningTimeLastUpdate = Environment.TickCount;
                            //Debug.WriteLine(UpdateApp.Name + " has been running for one minute, total: " + UpdateApp.RunningTime);
                        }
                    }
                    catch { }
                }

                //Save changes to Json file
                if (ApplicationUpdated) { JsonSaveApps(); }
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