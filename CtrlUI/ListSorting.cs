using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

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
                IEnumerable<DataBindApp> CurrentApps = CombineAppLists(false, false);
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

        void MoveApplicationList_Left()
        {
            try
            {
                //Sort the lists by number
                SortAppLists(true, true);

                ObservableCollection<DataBindApp> AppList = vEditAppListBox.ItemsSource as ObservableCollection<DataBindApp>;
                DataBindApp TargetAppDataBind = AppList[AppList.IndexOf(vEditAppDataBind) - 1];
                int SelectedNumber = vEditAppDataBind.Number;
                int TargetNumber = TargetAppDataBind.Number;

                //Update the application number
                vEditAppDataBind.Number = TargetNumber;
                TargetAppDataBind.Number = SelectedNumber;

                //Sort the lists by number
                SortAppLists(true, true);
                Popup_Show_Status("Sorting", "Moving app left");

                //Save json applist
                JsonSaveApps();
            }
            catch { }
        }

        void MoveApplicationList_Right()
        {
            try
            {
                //Sort the lists by number
                SortAppLists(true, true);

                ObservableCollection<DataBindApp> AppList = vEditAppListBox.ItemsSource as ObservableCollection<DataBindApp>;
                DataBindApp TargetAppDataBind = AppList[AppList.IndexOf(vEditAppDataBind) + 1];
                int SelectedNumber = vEditAppDataBind.Number;
                int TargetNumber = TargetAppDataBind.Number;

                //Update the application number
                vEditAppDataBind.Number = TargetNumber;
                TargetAppDataBind.Number = SelectedNumber;

                //Sort the lists by number
                SortAppLists(true, true);
                Popup_Show_Status("Sorting", "Moving app right");

                //Save json applist
                JsonSaveApps();
            }
            catch { }
        }

        public static void SortObservableCollection<TSource, TKey>(ObservableCollection<TSource> source, Func<TSource, TKey> key, Func<TSource, bool> where, bool ascending)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    int skipCount = 0;
                    List<TSource> sortedList = null;
                    if (where != null)
                    {
                        skipCount = source.Count() - source.Where(where).Count();
                        if (ascending)
                        {
                            sortedList = source.Where(where).OrderBy(key).ToList();
                        }
                        else
                        {
                            sortedList = source.Where(where).OrderByDescending(key).ToList();
                        }
                    }
                    else
                    {
                        if (ascending)
                        {
                            sortedList = source.OrderBy(key).ToList();
                        }
                        else
                        {
                            sortedList = source.OrderByDescending(key).ToList();
                        }
                    }

                    for (int i = skipCount; i < sortedList.Count; i++)
                    {
                        source.Move(source.IndexOf(sortedList[i]), i);
                    }
                });
            }
            catch { }
        }

        //Sort the application lists
        void SortAppLists(bool ForceNumber, bool Silent)
        {
            try
            {
                if (ForceNumber || vSortType == "Name")
                {
                    if (!Silent) { Popup_Show_Status("Sorting", "Sorting by number or date"); }
                    vSortType = "Number";

                    SortObservableCollection(List_Games, x => x.Number, null, true);
                    SortObservableCollection(List_Apps, x => x.Number, null, true);
                    SortObservableCollection(List_Emulators, x => x.Number, null, true);
                    SortObservableCollection(List_Shortcuts, x => x.TimeCreation, null, false);
                    SortObservableCollection(List_Processes, x => x.RunningTime, null, true);

                    menuButtonSorting_TextBlock.Text = "Sort applications by name";
                    ToolTip newTooltip = new ToolTip() { Content = "Sort by name" };
                    menuButtonSorting.ToolTip = newTooltip;
                    button_MenuSorting.ToolTip = newTooltip;
                }
                else if (vSortType == "Number")
                {
                    if (!Silent) { Popup_Show_Status("Sorting", "Sorting by name"); }
                    vSortType = "Name";

                    SortObservableCollection(List_Games, x => x.Name, null, true);
                    SortObservableCollection(List_Apps, x => x.Name, null, true);
                    SortObservableCollection(List_Emulators, x => x.Name, null, true);
                    SortObservableCollection(List_Shortcuts, x => x.Name, null, true);
                    SortObservableCollection(List_Processes, x => x.Name, null, true);

                    menuButtonSorting_TextBlock.Text = "Sort applications by number or date";
                    ToolTip newTooltip = new ToolTip() { Content = "Sort by number or date" };
                    menuButtonSorting.ToolTip = newTooltip;
                    button_MenuSorting.ToolTip = newTooltip;
                }
            }
            catch { }
        }
    }
}