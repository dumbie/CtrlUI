using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check and return the highest app number
        int GetHighestAppNumber()
        {
            int NewNumber = 0;
            try
            {
                IEnumerable<DataBindApp> CurrentApps = CombineAppLists(false, false, false);
                if (CurrentApps.Any())
                {
                    NewNumber = CurrentApps.Select(x => x.Number).Max() + 1;
                }
                else
                {
                    NewNumber = 1;
                }
            }
            catch { }
            return NewNumber;
        }

        public void SortObservableCollection<TSource>(ListBox targetListBox, ObservableCollection<TSource> targetSource, List<SortFunction<TSource>> orderBy, Func<TSource, bool> where)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Get the current selected item
                    object selectedItem = targetListBox.SelectedItem;

                    //Filter list
                    int skipCount = 0;
                    IEnumerable<TSource> whereEnumerable = null;
                    if (where != null)
                    {
                        whereEnumerable = targetSource.Where(where);
                        skipCount = targetSource.Count() - whereEnumerable.Count();
                    }
                    else
                    {
                        whereEnumerable = targetSource;
                    }

                    //Sort list
                    IOrderedEnumerable<TSource> sortEnumerable = null;
                    foreach (SortFunction<TSource> orderFunc in orderBy)
                    {
                        if (sortEnumerable == null)
                        {
                            if (orderFunc.ascending)
                            {
                                sortEnumerable = whereEnumerable.OrderBy(orderFunc.function);
                            }
                            else
                            {
                                sortEnumerable = whereEnumerable.OrderByDescending(orderFunc.function);
                            }
                        }
                        else
                        {
                            if (orderFunc.ascending)
                            {
                                sortEnumerable = sortEnumerable.ThenBy(orderFunc.function);
                            }
                            else
                            {
                                sortEnumerable = sortEnumerable.ThenByDescending(orderFunc.function);
                            }
                        }
                    }

                    //Move items
                    List<TSource> sortedList = sortEnumerable.ToList();
                    for (int i = skipCount; i < sortedList.Count(); i++)
                    {
                        targetSource.Move(targetSource.IndexOf(sortedList[i]), i);
                    }

                    //Select the focused item
                    ListBoxSelectItem(targetListBox, selectedItem);
                });
            }
            catch { }
        }

        //Sort the application lists
        async Task SortAppListsSwitch(bool silentSort)
        {
            try
            {
                if (vSortType == SortingType.Number)
                {
                    await SortAppListsByName(silentSort);
                }
                else
                {
                    await SortAppListsByNumber(silentSort);
                }
            }
            catch { }
        }

        //Sort the application lists by number
        async Task SortAppListsByNumber(bool silentSort)
        {
            try
            {
                if (!silentSort) { await Notification_Send_Status("Sorting", "Sorting by number or date"); }
                vSortType = SortingType.Number;

                SortFunction<DataBindApp> sortFuncNumber = new SortFunction<DataBindApp>();
                sortFuncNumber.function = x => x.Number;

                SortFunction<DataBindApp> sortFuncLauncher = new SortFunction<DataBindApp>();
                sortFuncLauncher.function = x => x.Launcher;

                SortFunction<DataBindApp> sortFuncTimeCreation = new SortFunction<DataBindApp>();
                sortFuncTimeCreation.function = x => x.TimeCreation;
                sortFuncTimeCreation.ascending = false;

                SortFunction<DataBindApp> sortFuncRunningTime = new SortFunction<DataBindApp>();
                sortFuncRunningTime.function = x => x.RunningTime;

                List<SortFunction<DataBindApp>> orderListGames = new List<SortFunction<DataBindApp>>();
                orderListGames.Add(sortFuncNumber);
                SortObservableCollection(lb_Games, List_Games, orderListGames, null);

                List<SortFunction<DataBindApp>> orderListApps = new List<SortFunction<DataBindApp>>();
                orderListApps.Add(sortFuncNumber);
                SortObservableCollection(lb_Apps, List_Apps, orderListApps, null);

                List<SortFunction<DataBindApp>> orderListEmulators = new List<SortFunction<DataBindApp>>();
                orderListEmulators.Add(sortFuncNumber);
                SortObservableCollection(lb_Emulators, List_Emulators, orderListEmulators, null);

                List<SortFunction<DataBindApp>> orderListLaunchers = new List<SortFunction<DataBindApp>>();
                orderListLaunchers.Add(sortFuncLauncher);
                SortObservableCollection(lb_Launchers, List_Launchers, orderListLaunchers, null);

                List<SortFunction<DataBindApp>> orderListShortcuts = new List<SortFunction<DataBindApp>>();
                orderListShortcuts.Add(sortFuncTimeCreation);
                SortObservableCollection(lb_Shortcuts, List_Shortcuts, orderListShortcuts, null);

                List<SortFunction<DataBindApp>> orderListProcesses = new List<SortFunction<DataBindApp>>();
                orderListProcesses.Add(sortFuncRunningTime);
                SortObservableCollection(lb_Processes, List_Processes, orderListProcesses, null);

                DataBindString menuSortItem = List_MainMenu.Where(x => x.Data1.ToString() == "menuButtonSorting").FirstOrDefault();
                if (menuSortItem != null)
                {
                    menuSortItem.Name = "Sort applications by name";
                }

                ToolTip newTooltip = new ToolTip() { Content = "Sort by name" };
                button_MenuSorting.ToolTip = newTooltip;
            }
            catch { }
        }

        //Sort the application lists by name
        async Task SortAppListsByName(bool silentSort)
        {
            try
            {
                if (!silentSort) { await Notification_Send_Status("Sorting", "Sorting by name"); }
                vSortType = SortingType.Name;

                SortFunction<DataBindApp> sortFuncName = new SortFunction<DataBindApp>();
                sortFuncName.function = x => x.Name;

                List<SortFunction<DataBindApp>> orderListGames = new List<SortFunction<DataBindApp>>();
                orderListGames.Add(sortFuncName);
                SortObservableCollection(lb_Games, List_Games, orderListGames, null);

                List<SortFunction<DataBindApp>> orderListApps = new List<SortFunction<DataBindApp>>();
                orderListApps.Add(sortFuncName);
                SortObservableCollection(lb_Apps, List_Apps, orderListApps, null);

                List<SortFunction<DataBindApp>> orderListEmulators = new List<SortFunction<DataBindApp>>();
                orderListEmulators.Add(sortFuncName);
                SortObservableCollection(lb_Emulators, List_Emulators, orderListEmulators, null);

                List<SortFunction<DataBindApp>> orderListLaunchers = new List<SortFunction<DataBindApp>>();
                orderListLaunchers.Add(sortFuncName);
                SortObservableCollection(lb_Launchers, List_Launchers, orderListLaunchers, null);

                List<SortFunction<DataBindApp>> orderListShortcuts = new List<SortFunction<DataBindApp>>();
                orderListShortcuts.Add(sortFuncName);
                SortObservableCollection(lb_Shortcuts, List_Shortcuts, orderListShortcuts, null);

                List<SortFunction<DataBindApp>> orderListProcesses = new List<SortFunction<DataBindApp>>();
                orderListProcesses.Add(sortFuncName);
                SortObservableCollection(lb_Processes, List_Processes, orderListProcesses, null);

                DataBindString menuSortItem = List_MainMenu.Where(x => x.Data1.ToString() == "menuButtonSorting").FirstOrDefault();
                if (menuSortItem != null)
                {
                    menuSortItem.Name = "Sort applications by number or date";
                }

                ToolTip newTooltip = new ToolTip() { Content = "Sort by number or date" };
                button_MenuSorting.ToolTip = newTooltip;
            }
            catch { }
        }
    }
}