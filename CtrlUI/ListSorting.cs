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

        public void SortObservableCollection<TSource, TKey>(ListBox targetListBox, ObservableCollection<TSource> targetSource, Func<TSource, TKey> key, Func<TSource, bool> where, bool ascending)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Get the current selected item
                    object selectedItem = targetListBox.SelectedItem;

                    int skipCount = 0;
                    List<TSource> sortedList = null;
                    if (where != null)
                    {
                        skipCount = targetSource.Count() - targetSource.Where(where).Count();
                        if (ascending)
                        {
                            sortedList = targetSource.Where(where).OrderBy(key).ToList();
                        }
                        else
                        {
                            sortedList = targetSource.Where(where).OrderByDescending(key).ToList();
                        }
                    }
                    else
                    {
                        if (ascending)
                        {
                            sortedList = targetSource.OrderBy(key).ToList();
                        }
                        else
                        {
                            sortedList = targetSource.OrderByDescending(key).ToList();
                        }
                    }

                    for (int i = skipCount; i < sortedList.Count; i++)
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

                SortObservableCollection(lb_Games, List_Games, x => x.Number, null, true);
                SortObservableCollection(lb_Apps, List_Apps, x => x.Number, null, true);
                SortObservableCollection(lb_Emulators, List_Emulators, x => x.Number, null, true);
                SortObservableCollection(lb_Launchers, List_Launchers, x => x.Launcher, null, true);
                SortObservableCollection(lb_Shortcuts, List_Shortcuts, x => x.TimeCreation, null, false);
                SortObservableCollection(lb_Processes, List_Processes, x => x.RunningTime, null, true);

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

                SortObservableCollection(lb_Games, List_Games, x => x.Name, null, true);
                SortObservableCollection(lb_Apps, List_Apps, x => x.Name, null, true);
                SortObservableCollection(lb_Emulators, List_Emulators, x => x.Name, null, true);
                SortObservableCollection(lb_Launchers, List_Launchers, x => x.Name, null, true);
                SortObservableCollection(lb_Shortcuts, List_Shortcuts, x => x.Name, null, true);
                SortObservableCollection(lb_Processes, List_Processes, x => x.Name, null, true);

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